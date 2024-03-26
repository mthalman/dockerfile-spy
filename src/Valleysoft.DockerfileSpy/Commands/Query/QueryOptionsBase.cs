using System.CommandLine;

namespace Valleysoft.DockerfileSpy.Commands.Query;

public class QueryOptionsBase : OptionsBase
{
    private const string DefaultDockerfilePath = "Dockerfile";
    private readonly Option<string> _dockerfilePathOption;

    public string DockerfilePath { get; set; } = DefaultDockerfilePath;

    public QueryOptionsBase()
    {
        _dockerfilePathOption = Add(new Option<string>(
            ["--dockerfile", "-f"], () => DefaultDockerfilePath, "The path to the Dockerfile to query"));
    }

    protected override void GetValues()
    {
        DockerfilePath = GetValue(_dockerfilePathOption)!;
    }
}
