#load "Configuration.cake"

public static Configuration IncludeArtifactCopyTarget(this Configuration configuration, DirectoryPath sourceDirectory)
{
    var copyList = new HashSet<DirectoryPath>();
    if(configuration.TaskParameters.TryGetValue("Artifacts:Copy__Items", out object value) && value is HashSet<DirectoryPath>)
    {
        copyList = value as HashSet<DirectoryPath>;
    }
    else
    {
        configuration.TaskParameters.Add("Artifacts:Copy__Items", copyList);
    }

    copyList.Add(sourceDirectory);

    return configuration;
}

public static bool HasArtifactCopyTargets(this Configuration configuration)
{
    var copyList = new HashSet<DirectoryPath>();
    if(configuration.TaskParameters.TryGetValue("Artifacts:Copy__Items", out object value) && value is HashSet<DirectoryPath>)
    {
        copyList = value as HashSet<DirectoryPath>;
    }

    return copyList.Any();
}

public static IEnumerable<DirectoryPath> GetArtifactCopyTargets(this Configuration configuration)
{
    var copyList = new HashSet<DirectoryPath>();
    if(configuration.TaskParameters.TryGetValue("Artifacts:Copy__Items", out object value) && value is HashSet<DirectoryPath>)
    {
        copyList = value as HashSet<DirectoryPath>;
    }    
    return copyList;
}

Task("Artifacts:Copy")
    .WithCriteria<Configuration>((ctx, config) => config.HasArtifactCopyTargets())
    .IsDependentOn("Build")
    .IsDependeeOf("Publish")
    .Does<Configuration>(config => 
{
    var artifacts = $"{config.Artifacts.Root}";
    EnsureDirectoryExists(artifacts);
    foreach(var directory in config.GetArtifactCopyTargets()) 
    {
        var copyFrom = directory;
        var copyTo = $"{artifacts}/{directory.GetDirectoryName()}";
        Information("{0} -> {1}", copyFrom, copyTo);
        EnsureDirectoryExists(copyTo);
        CopyDirectory(directory, copyTo);
        config.Artifacts.Add(ArtifactTypeOption.Other, directory.GetDirectoryName(), directory.FullPath);
    }
});