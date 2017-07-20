#tool "GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target                  = Argument("target", AppVeyor.IsRunningOnAppVeyor ? "CI" : "Default");
var configuration           = Argument("configuration", "Release");
var solutionPath            = MakeAbsolute(File(Argument("solutionPath", "./Robotify.AspNetCore.sln")));

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var testAssemblies              = "./tests/**/bin/" +configuration +"/*.Tests.dll";
var coverageReportXmlFilePath   = "";

var artifacts               = MakeAbsolute(Directory(Argument("artifactPath", "./artifacts")));
var versionAssemblyInfo     = MakeAbsolute(File(Argument("versionAssemblyInfo", "VersionAssemblyInfo.cs")));

IEnumerable<FilePath> nugetProjectPaths     = null;
SolutionParserResult solution               = null;
GitVersion versionInfo                      = null;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(ctx => {
    if(!FileExists(solutionPath)) throw new Exception(string.Format("Solution file not found - {0}", solutionPath.ToString()));
    solution = ParseSolution(solutionPath.ToString());

    Information("[Setup] Using Solution '{0}'", solutionPath.ToString());

    if(DirectoryExists(artifacts)) 
    {
        DeleteDirectory(artifacts, true);
    }
    
    EnsureDirectoryExists(artifacts);
    
    var binDirs = GetDirectories(solutionPath.GetDirectory() +@"\src\**\bin");
    var objDirs = GetDirectories(solutionPath.GetDirectory() +@"\src\**\obj");
    DeleteDirectories(binDirs, true);
    DeleteDirectories(objDirs, true);
});

Task("Update-Version-Info")
    .IsDependentOn("CreateVersionAssemblyInfo")
    .Does(() => 
{
        versionInfo = GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true,
            UpdateAssemblyInfoFilePath = versionAssemblyInfo
        });

    if(versionInfo != null) {
        Information("Version: {0}", versionInfo.FullSemVer);
    } else {
        throw new Exception("Unable to determine version");
    }
});

Task("CreateVersionAssemblyInfo")
    .WithCriteria(() => !FileExists(versionAssemblyInfo))
    .Does(() =>
{
    Information("Creating version assembly info");
    CreateAssemblyInfo(versionAssemblyInfo, new AssemblyInfoSettings {
        Version = "0.0.0.0",
        FileVersion = "0.0.0.0",
        InformationalVersion = "",
    });
});

Task("DotNet-MsBuild-Restore")
    .IsDependentOn("Update-Version-Info")
    .Does(() => {

        MSBuild(solutionPath, c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithTarget("Restore")
        );
});

Task("DotNet-MsBuild")
    .IsDependentOn("Restore")
    .Does(() => {

        MSBuild(solutionPath, c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("TreatWarningsAsErrors", "true")
            .WithTarget("Build")
        );

});

Task("DotNet-MsBuild-Pack")
    .IsDependentOn("Build")
    .Does(() => {

        MSBuild("src/Robotify.AspNetCore.csproj", c => c
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Normal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithProperty("PackageVersion", versionInfo.NuGetVersionV2)
            .WithProperty("NoBuild", "true")
            .WithTarget("Pack")
    );
});

Task("DotNet-MsBuild-CopyToArtifacts")
    .IsDependentOn("DotNet-MsBuild-Pack")
    .Does(() => {

        EnsureDirectoryExists(artifacts);
        CopyFiles("src/bin/" +configuration +"/*.nupkg", artifacts);
});

Task("DotNet-Test")
    .IsDependentOn("Build")
    .WithCriteria(() => FileExists("test/Robotify.AspNetCore.Tests.csproj"))
    .Does(() => {

    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true
    };

    DotNetCoreTest("test/Robotify.AspNetCore.Tests.csproj", settings);
});

Task("Print-AppVeyor-Environment-Variables")
    .WithCriteria(AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
{
    Information("CI: {0}", EnvironmentVariable("CI"));
    Information("APPVEYOR_API_URL: {0}", EnvironmentVariable("APPVEYOR_API_URL"));
    Information("APPVEYOR_PROJECT_ID: {0}", EnvironmentVariable("APPVEYOR_PROJECT_ID"));
    Information("APPVEYOR_PROJECT_NAME: {0}", EnvironmentVariable("APPVEYOR_PROJECT_NAME"));
    Information("APPVEYOR_PROJECT_SLUG: {0}", EnvironmentVariable("APPVEYOR_PROJECT_SLUG"));
    Information("APPVEYOR_BUILD_FOLDER: {0}", EnvironmentVariable("APPVEYOR_BUILD_FOLDER"));
    Information("APPVEYOR_BUILD_ID: {0}", EnvironmentVariable("APPVEYOR_BUILD_ID"));
    Information("APPVEYOR_BUILD_NUMBER: {0}", EnvironmentVariable("APPVEYOR_BUILD_NUMBER"));
    Information("APPVEYOR_BUILD_VERSION: {0}", EnvironmentVariable("APPVEYOR_BUILD_VERSION"));
    Information("APPVEYOR_PULL_REQUEST_NUMBER: {0}", EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER"));
    Information("APPVEYOR_PULL_REQUEST_TITLE: {0}", EnvironmentVariable("APPVEYOR_PULL_REQUEST_TITLE"));
    Information("APPVEYOR_JOB_ID: {0}", EnvironmentVariable("APPVEYOR_JOB_ID"));
    Information("APPVEYOR_REPO_PROVIDER: {0}", EnvironmentVariable("APPVEYOR_REPO_PROVIDER"));
    Information("APPVEYOR_REPO_SCM: {0}", EnvironmentVariable("APPVEYOR_REPO_SCM"));
    Information("APPVEYOR_REPO_NAME: {0}", EnvironmentVariable("APPVEYOR_REPO_NAME"));
    Information("APPVEYOR_REPO_BRANCH: {0}", EnvironmentVariable("APPVEYOR_REPO_BRANCH"));
    Information("APPVEYOR_REPO_TAG: {0}", EnvironmentVariable("APPVEYOR_REPO_TAG"));
    Information("APPVEYOR_REPO_TAG_NAME: {0}", EnvironmentVariable("APPVEYOR_REPO_TAG_NAME"));
    Information("APPVEYOR_REPO_COMMIT: {0}", EnvironmentVariable("APPVEYOR_REPO_COMMIT"));
    Information("APPVEYOR_REPO_COMMIT_AUTHOR: {0}", EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"));
    Information("APPVEYOR_REPO_COMMIT_TIMESTAMP: {0}", EnvironmentVariable("APPVEYOR_REPO_COMMIT_TIMESTAMP"));
    Information("APPVEYOR_SCHEDULED_BUILD: {0}", EnvironmentVariable("APPVEYOR_SCHEDULED_BUILD"));
    Information("APPVEYOR_FORCED_BUILD: {0}", EnvironmentVariable("APPVEYOR_FORCED_BUILD"));
    Information("APPVEYOR_RE_BUILD: {0}", EnvironmentVariable("APPVEYOR_RE_BUILD"));
    Information("PLATFORM: {0}", EnvironmentVariable("PLATFORM"));
    Information("CONFIGURATION: {0}", EnvironmentVariable("CONFIGURATION"));

});

Task("AppVeyor-Update-Build-Number")
    .IsDependentOn("Update-Version-Info")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(versionInfo.FullSemVer +"|" +AppVeyor.Environment.Build.Number);
});

Task("Appveyor-Upload-Artifacts")
    .IsDependentOn("Package")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
{
    foreach(var nupkg in GetFiles(artifacts +"/*.nupkg")) {
        AppVeyor.UploadArtifact(nupkg);
    }
});

Task("Appveyor")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .IsDependentOn("Print-AppVeyor-Environment-Variables")    
    .IsDependentOn("AppVeyor-Update-Build-Number")
    .IsDependentOn("AppVeyor-Upload-Artifacts");

// ************************** //

Task("Restore")
    .IsDependentOn("DotNet-MsBuild-Restore");

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("DotNet-MsBuild");

Task("Test")
    .IsDependentOn("Build")
    .IsDependentOn("DotNet-Test");

Task("Package")
    .IsDependentOn("Build")
    .IsDependentOn("DotNet-MsBuild-CopyToArtifacts")
    .IsDependentOn("DotNet-MsBuild-Pack");

Task("CI")
    .IsDependentOn("AppVeyor")
    .IsDependentOn("Default");

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
