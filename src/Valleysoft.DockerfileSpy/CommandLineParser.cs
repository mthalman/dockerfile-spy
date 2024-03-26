using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Valleysoft.DockerfileSpy.Commands.Query;

namespace Valleysoft.DockerfileSpy;

public static class CommandLineParser
{
    public static Parser Create(IConsole console)
    {
        RootCommand rootCommand = new("CLI for querying Dockerfile content")
        {
            new QueryCommand(console)
        };

        CommandLineBuilder cmdLineBuilder = new(rootCommand);

        return cmdLineBuilder.Build();
    }
}
