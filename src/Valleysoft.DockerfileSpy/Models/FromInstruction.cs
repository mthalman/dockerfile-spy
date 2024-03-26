namespace Valleysoft.DockerfileSpy.Models;

internal class FromInstruction
{
    public string ImageName { get; set; } = string.Empty;

    public string? StageName { get; set; }
}
