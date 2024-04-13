using System.CommandLine;
using System.Text.Json;
using System.Text.Json.Serialization;
using Valleysoft.DockerfileModel;

namespace Valleysoft.DockerfileSpy.Commands.Query;

public class FromCommand(IConsole console) : CommandWithOptions<FromCommandOptions>("from", "Queries for FROM instructions")
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly IConsole _console = console;

    protected override Task ExecuteAsync() => ExecuteCoreAsync();

    public Task ExecuteCoreAsync()
    {
        Dockerfile dockerfile = Dockerfile.Parse(File.ReadAllText(Options.DockerfilePath));
        dockerfile.ResolveVariables(options: new ResolutionOptions { RemoveEscapeCharacters = true, UpdateInline = true });
        IEnumerable<FromInstruction> fromInstructions = dockerfile.Items.OfType<FromInstruction>();

        List<Models.FromInstruction> fromInstructionModels = fromInstructions
            .Select(from => new Models.FromInstruction
            {
                ImageName = from.ImageName,
                StageName = from.StageName
            })
            .ToList();

        object result;
        if (Options.LayoutType == FromLayoutType.Graph)
        {
            Models.FromInstructionGraphNode[] leafGraphNodes = GetFromInstructionGraph(fromInstructionModels);
            result = leafGraphNodes.ToArray();
        }
        else
        {
            result = fromInstructionModels;
        }

        _console.WriteLine(JsonSerializer.Serialize(result, s_jsonSerializerOptions));

        return Task.CompletedTask;
    }

    private static Models.FromInstructionGraphNode[] GetFromInstructionGraph(List<Models.FromInstruction> fromInstructionModels)
    {
        Dictionary<string, Models.FromInstructionGraphNode> nodes = [];

        foreach (Models.FromInstruction fromInstruction in fromInstructionModels.Where(from => from.StageName is not null))
        {
            Models.FromInstructionGraphNode node = new(fromInstruction);
            nodes[fromInstruction.StageName!] = node;

            // If the FROM references a previous stage, add it as a child of that node
            if (nodes.TryGetValue(fromInstruction.ImageName, out Models.FromInstructionGraphNode? parentGraphNode))
            {
                node.Parent = parentGraphNode;
                parentGraphNode.Children.Add(node);
            }
        }

        List<Models.FromInstructionGraphNode> nodesWithNoStageName = [];

        foreach (Models.FromInstruction fromInstruction in fromInstructionModels.Where(from => from.StageName is null))
        {
            Models.FromInstructionGraphNode node = new(fromInstruction);
            nodesWithNoStageName.Add(node);

            // If the FROM references a previous stage, add it as a child of that node
            if (nodes.TryGetValue(fromInstruction.ImageName, out Models.FromInstructionGraphNode? parentGraphNode))
            {
                node.Parent = parentGraphNode;
                parentGraphNode.Children.Add(node);
            }
        }

        // It is possible for a leaf stage to have a stage name, so we need to walk the graph to find all nodes with no children
        return nodes.Values
            .Where(node => !node.Children.Any())
            .Concat(nodesWithNoStageName)
            .ToArray();
    }
}
