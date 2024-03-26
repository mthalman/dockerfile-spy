using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using Moq;

namespace Valleysoft.DockerfileSpy.Tests
{
    public class FromCommandTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            DirectoryInfo workingDir = new(Path.Combine(Environment.CurrentDirectory, "TestData", "FromCommand"));
            return workingDir.GetDirectories()
                .Select(dir => new TestScenario(
                        dir.Name,
                        Path.Combine(dir.FullName, "command.txt"),
                        Path.Combine(dir.FullName, "expected-output.txt"),
                        dir.FullName))
                .Select(scenario => new object[] { scenario });
        }

        public class TestScenario(string name, string command, string expectedOutputPath, string workingDirectory)
        {
            public string Name { get; } = name;
            public string Command { get; } = command;
            public string ExpectedOutputPath { get; } = expectedOutputPath;
            public string WorkingDirectory { get; } = workingDirectory;
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Test(TestScenario testScenario)
        {
            Mock<IConsole> consoleMock = new();
            consoleMock.Setup(o => o.Out.Write(It.IsAny<string>()));

            string command = File.ReadAllText(testScenario.Command);

            Parser parser = CommandLineParser.Create(consoleMock.Object);

            string savedCurrentDirectory = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = testScenario.WorkingDirectory;
                await parser.InvokeAsync(command);

                string expectedOutput = File.ReadAllText(testScenario.ExpectedOutputPath);
                Mock.Get(consoleMock.Object.Out).Verify(o => o.Write(expectedOutput));
                //consoleMock.Verify(o => o.Out.Write(expectedOutput), Times.Once);
            }
            finally
            {
                Environment.CurrentDirectory = savedCurrentDirectory;
            }


        }
    }
}
