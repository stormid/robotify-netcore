#load "Configuration.cake"

Task("CI:AppVeyor:UploadArtifacts")
    .WithCriteria<Configuration>((ctx, config) => BuildSystem.IsRunningOnAppVeyor)
    .IsDependentOn("Publish")
    .IsDependeeOf("CI:UploadArtifacts")
    .Does<Configuration>(config => 
{
    foreach(var artifact in config.Artifacts) {
        if(FileExists(artifact.Path)) {
            BuildSystem.AppVeyor.UploadArtifact(artifact.Path, new AppVeyorUploadArtifactsSettings {
                ArtifactType = artifact.Type == ArtifactTypeOption.WebDeploy ? AppVeyorUploadArtifactType.WebDeployPackage : AppVeyorUploadArtifactType.Auto
            });
        }
    }
});

Task("CI:AppVeyor:UpdateBuildNumber")
    .IsDependeeOf("CI:UpdateBuildNumber")
    .WithCriteria<Configuration>((ctx, config) => AppVeyor.IsRunningOnAppVeyor)
    .Does<Configuration>(config =>
{
    AppVeyor.UpdateBuildVersion(config.Version.FullSemVersion +"." +AppVeyor.Environment.Build.Number);
});