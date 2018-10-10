#load "CI-VSTS.cake"
#load "CI-AppVeyor.cake"

Task("CI")
    .IsDependentOn("CI:UpdateBuildNumber")
    .IsDependeeOf("Default")
    .IsDependentOn("CI:UploadArtifacts");

Task("CI:UpdateBuildNumber")
    .IsDependeeOf("CI").Does<Configuration>(config => Information("Build Number: {0}", config.Version.SemVersion));

Task("CI:UploadArtifacts")
    .IsDependeeOf("CI")
    .IsDependentOn("Publish");