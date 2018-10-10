#load "Configuration.cake"

Task("Restore:NuGet")
    .IsDependeeOf("Restore")
    .Does<Configuration>(config => 
{
    NuGetRestore(config.Solution.Path.ToString(), new NuGetRestoreSettings());
});