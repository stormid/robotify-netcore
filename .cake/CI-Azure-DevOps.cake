#load "Configuration.cake"

Task("CI:VSTS:UploadArtifacts")
    .WithCriteria<Configuration>((ctx, config) => BuildSystem.IsRunningOnAzurePipelinesHosted || TFBuild.IsRunningOnAzurePipelines)
    .IsDependentOn("Publish")
    .IsDependeeOf("CI:UploadArtifacts")
    .Does<Configuration>(config => 
{
    Information("Uploading artifacts from {0}", config.Artifacts.Root);
    TFBuild.Commands.UploadArtifact("artifacts", config.Artifacts.Root.ToString(), "artifacts");
});

Task("CI:VSTS:UpdateBuildNumber")
    .IsDependeeOf("CI:UpdateBuildNumber")
    .WithCriteria<Configuration>((ctx, config) => BuildSystem.IsRunningOnAzurePipelinesHosted || TFBuild.IsRunningOnAzurePipelines)
    .Does<Configuration>(config =>
{
    Information(
        @"Repository:
        Branch: {0}
        SourceVersion: {1}
        Shelveset: {2}",
        BuildSystem.TFBuild.Environment.Repository.Branch,
        BuildSystem.TFBuild.Environment.Repository.SourceVersion,
        BuildSystem.TFBuild.Environment.Repository.Shelveset
        );    

    TFBuild.Commands.UpdateBuildNumber(config.Version.FullSemVersion);
    TFBuild.Commands.SetVariable("GitVersion.Version", config.Version.Version);
    TFBuild.Commands.SetVariable("GitVersion.SemVer", config.Version.SemVersion);
    TFBuild.Commands.SetVariable("GitVersion.InformationalVersion", config.Version.InformationalVersion);
    TFBuild.Commands.SetVariable("GitVersion.FullSemVer", config.Version.FullSemVersion);
    TFBuild.Commands.SetVariable("Cake.Version", config.Version.CakeVersion);
});