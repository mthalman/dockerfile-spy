using System.CommandLine;

namespace Valleysoft.DockerfileSpy.Commands.Query;

public class FromCommandOptions : QueryOptionsBase
{
    private readonly Option<FromLayoutType> _fromLayoutTypeOption;

    public FromLayoutType LayoutType { get; set; }

    public FromCommandOptions()
    {
        _fromLayoutTypeOption = Add(new Option<FromLayoutType>(
            "--layout", () => FromLayoutType.Flat, "The layout type of the results"));
    }

    protected override void GetValues()
    {
        base.GetValues();

        LayoutType = GetValue(_fromLayoutTypeOption);
    }
}

public enum FromLayoutType
{
    Flat,
    Graph
}
