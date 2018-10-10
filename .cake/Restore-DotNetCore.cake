#load "Configuration.cake"

Task("Restore:DotNetCore")
    .IsDependeeOf("Restore")
    .Does<Configuration>(config => DotNetCoreRestore(config.Solution.Path.ToString(), new DotNetCoreRestoreSettings { Verbosity = DotNetCoreVerbosity.Minimal }));