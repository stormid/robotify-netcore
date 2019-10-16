#load "Configuration.cake"

Task("Publish:Pack:DotNetCore")
    .IsDependeeOf("Publish")
    .WithCriteria<Configuration>((ctx, config) => config.Solution.NuGetProjects.Any())
    .Does<Configuration>(config => 
{
    var projectArtifactDirectory = config.Artifacts.GetRootFor(ArtifactTypeOption.NuGet);
    var settings = new DotNetCorePackSettings
    {
        NoBuild = true,
        NoRestore = true,
        IncludeSymbols = true,
        Configuration = config.Solution.BuildConfiguration,
        OutputDirectory = projectArtifactDirectory
    };
    settings.MSBuildSettings = new DotNetCoreMSBuildSettings();
    settings.MSBuildSettings
        .SetVersion(config.Version.FullSemVersion)
        .SetConfiguration(config.Solution.BuildConfiguration)
        .WithProperty("PackageVersion", config.Version.SemVersion);
    settings.MSBuildSettings.NoLogo = true;

    foreach(var nugetProject in config.Solution.NuGetProjects) {
        DotNetCorePack(nugetProject.ProjectFilePath.ToString(), settings);
    }

    foreach(var package in GetFiles($"{projectArtifactDirectory}/*.nupkg")) 
    {    
        config.Artifacts.Add(ArtifactTypeOption.NuGet, package.GetFilename().ToString(), package.FullPath);
    }
});
