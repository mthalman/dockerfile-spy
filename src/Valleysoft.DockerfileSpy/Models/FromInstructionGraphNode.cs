namespace Valleysoft.DockerfileSpy.Models;

internal class FromInstructionGraphNode(FromInstruction fromInstruction)
{
    public FromInstruction FromInstruction { get; init; } = fromInstruction;
    public FromInstructionGraphNode? Parent { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public List<FromInstructionGraphNode> Children { get; } = [];
}
