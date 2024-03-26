using System.CommandLine;
using System.CommandLine.IO;
using Valleysoft.DockerfileSpy.Commands.Query;

SystemConsole console = new();

RootCommand rootCommand = new("CLI for querying Dockerfile content")
{
    new QueryCommand(console)
};

return rootCommand.Invoke(args);
