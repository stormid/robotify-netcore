#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0&prerelease"

public class BuildVersion {
    public FilePath VersionAssemblyInfo { get; private set; }
    public string Version { get; private set; }
    public string SemVersion { get; private set; }
    public string CakeVersion { get; private set; } = typeof(ICakeContext).Assembly.GetName().Version.ToString();
    public string InformationalVersion { get; private set; }
    public string FullSemVersion { get; private set; }

    public static BuildVersion DetermineBuildVersion(ICakeContext context) {
        return new BuildVersion(context);
    }

    private BuildVersion(ICakeContext context) {
        VersionAssemblyInfo = context.Argument("versionAssemblyInfo", "VersionAssemblyInfo.cs");

        if(context.DirectoryExists(".git")) {
            UseGitVersion(context);
        }
        else 
        {
            UseVersionAssemblyInfo(context);
        }

        LogVersionInformation(context);

        EnsureVersionInformation();
    }

    private void LogVersionInformation(ICakeContext context) {
        if(context.Log.Verbosity == Verbosity.Diagnostic) {
            context.Verbose("--- ENVIRONMENT ---");
            foreach(var s in context.EnvironmentVariables()) {
                context.Verbose("{0} = {1}", s.Key, s.Value);
            } 
            context.Verbose("-------------------");
        }

        context.Information($"{nameof(Version)} = {Version}");
        context.Information($"{nameof(SemVersion)} = {SemVersion}");
        context.Information($"{nameof(CakeVersion)} = {CakeVersion}");
        context.Information($"{nameof(InformationalVersion)} = {InformationalVersion}");
        context.Information($"{nameof(FullSemVersion)} = {FullSemVersion}");        
    }

    private void UseVersionAssemblyInfo(ICakeContext context) {
        if(context.FileExists(VersionAssemblyInfo)) {
            context.Verbose($"Looking up semantic version from {VersionAssemblyInfo}");

            var assemblyInfo = context.ParseAssemblyInfo(VersionAssemblyInfo);
            Version = assemblyInfo.AssemblyVersion;
            SemVersion = assemblyInfo.AssemblyInformationalVersion;
            InformationalVersion = assemblyInfo.AssemblyInformationalVersion;
            FullSemVersion = assemblyInfo.AssemblyVersion;
        }
        else
        {
            context.Error("Unable to calculate or retrieve version information");
        }        
    }

    private void UseGitVersion(ICakeContext context) {
        var settings = new GitVersionSettings {
            UpdateAssemblyInfo = true,
            UpdateAssemblyInfoFilePath = VersionAssemblyInfo,         
            NoFetch = true
        };
        settings.ArgumentCustomization = args => args.Append("-ensureassemblyinfo");
        context.Verbose($"Calculating semantic version....");

        if(context.BuildSystem().IsLocalBuild || !context.HasArgument("GitVersionFromBuildServer"))
        {
            context.Verbose("Outputting semantic version as JSON");
            settings.OutputType = GitVersionOutput.Json;
            var gitVersion = context.GitVersion(settings);
            Version = gitVersion.MajorMinorPatch;
            SemVersion = gitVersion.LegacySemVerPadded;
            InformationalVersion = gitVersion.InformationalVersion;
            FullSemVersion = gitVersion.FullSemVer;
        }
        else
        {
            // not working properly
            context.Verbose("Outputting semantic version for BUILDSERVER");
            settings.OutputType = GitVersionOutput.BuildServer;
            context.GitVersion(settings);
            Version = context.EnvironmentVariable("GITVERSION_MAJORMINORPATCH");
            SemVersion = context.EnvironmentVariable("GITVERSION_LEGACYSEMVERPADDED");
            InformationalVersion = context.EnvironmentVariable("GITVERSION_INFORMATIONALVERSION");
            FullSemVersion = context.EnvironmentVariable("GITVERSION_FULLSEMVER");
        }        
    }

    private void EnsureVersionInformation() {
        if(new [] { Version, SemVersion, FullSemVersion, InformationalVersion }.Any(string.IsNullOrWhiteSpace)) {
            throw new Exception("Version information has not been determined, build failed!");
        }
    }
}
