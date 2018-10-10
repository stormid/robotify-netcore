#load "Configuration.cake"

Task("Test:DotNetCore")
    .IsDependentOn("Build")
    .IsDependeeOf("Test")
    .WithCriteria<Configuration>((ctx, config) => config.Solution.TestProjects.Any())
    .Does<Configuration>(config => 
{        
    foreach(var testProject in config.Solution.TestProjects) {
        var settings = new DotNetCoreTestSettings() {
            Configuration = config.Solution.BuildConfiguration,
            Logger = $"trx;LogFileName={config.Artifacts.Root}/test-results/{testProject.AssemblyName}.xml",
            NoBuild = true,
            NoRestore = true
        };

        DotNetCoreTest(testProject.ProjectFilePath.ToString(), settings);
    }
});
