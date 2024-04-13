using System.CommandLine;

namespace Valleysoft.DockerfileSpy.Commands.Query;

public class QueryOptionsBase : OptionsBase
{
    private const string DefaultDockerfilePath = "Dockerfile";
    private readonly Option<string> _dockerfilePathOption;
    private readonly Option<bool> _resolveVariablesOption;
    private readonly Option<Dictionary<string, string?>> _variableOverridesOption;

    public string DockerfilePath { get; private set; } = DefaultDockerfilePath;
    public bool ResolveVariables { get; private set; }
    public Dictionary<string, string?> VariableOverrides { get; private set; } = new();

    public QueryOptionsBase()
    {
        _dockerfilePathOption = Add(new Option<string>(
            ["--dockerfile", "-f"], () => DefaultDockerfilePath, "The path to the Dockerfile to query"));

        _resolveVariablesOption = Add(new Option<bool>(
            "--resolve-vars", "Indicates that variables referenced in the queried values should be resolved"));

        _variableOverridesOption = Add(new Option<Dictionary<string, string?>>(
            "--var",
            description: "Override for variable referenced in the queried values (<var>=<value>)",
            parseArgument: argResult =>
                argResult.Tokens
                    .ToList()
                    .Select(token => ParseKeyValuePair(token.Value, '='))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            ));
    }

    protected override void GetValues()
    {
        DockerfilePath = GetValue(_dockerfilePathOption)!;
        ResolveVariables = GetValue(_resolveVariablesOption);
        VariableOverrides = GetValue(_variableOverridesOption)!;
    }
}
