using System.CommandLine;

namespace Valleysoft.DockerfileSpy.Commands.Query;

internal class QueryCommand : Command
{
    public QueryCommand(IConsole console) : base("query", "Queries for elements of a Dockerfile")
    {
        Add(new FromCommand(console));
    }
}
