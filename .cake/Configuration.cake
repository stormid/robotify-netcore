#addin "Cake.Incubator"
#load "Configuration-Version.cake"

const string _solutionFilePathPattern = "*.sln";
const string _defaultBuildConfiguration = "Release";

Task("Noop");

Task("Default")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Publish")
    .IsDependentOn("Artifacts:List");

Task("Restore");

Task("Build")
    .IsDependentOn("Restore");

Task("Test")
    .IsDependentOn("Build");

Task("Publish")
    .IsDependentOn("Test")
    .IsDependeeOf("Artifacts:List");

Task("Artifacts:List")
    .IsDependentOn("Publish")
    .Does<Configuration>(config => {
        foreach(var artifact in config.Artifacts) {
            Information(artifact);
        }
    });    

/* ************************************* */    

public class Configuration {
    public static Configuration Create(ISetupContext context) {
        var config = new Configuration(context, _solutionFilePathPattern, _defaultBuildConfiguration);
        config.Log(context.Log);
        return config;
    }

    public static Configuration Create(ISetupContext context, Action<Configuration> configure) {
        var config = new Configuration(context, _solutionFilePathPattern, _defaultBuildConfiguration);
        configure(config);
        config.Log(context.Log);
        return config;
    }

    public static Configuration Create(ISetupContext context, Action<Configuration> configure = null, string solutionFilePathPattern = _solutionFilePathPattern, string defaultBuildConfiguration = _defaultBuildConfiguration) {
        var config = new Configuration(context, solutionFilePathPattern, _defaultBuildConfiguration);
        configure?.Invoke(config);
        config.Log(context.Log);
        return config;
    }

    public static Configuration Create(ISetupContext context, string solutionFilePathPattern = _solutionFilePathPattern, Action<Configuration> configure = null) {
        var config = new Configuration(context, solutionFilePathPattern, _defaultBuildConfiguration);
        configure?.Invoke(config);
        config.Log(context.Log);
        return config;
    }

    public static Configuration Create(ISetupContext context, string solutionFilePathPattern = _solutionFilePathPattern, string defaultBuildConfiguration = _defaultBuildConfiguration, Action<Configuration> configure = null) {
        var config = new Configuration(context, solutionFilePathPattern, defaultBuildConfiguration);
        configure?.Invoke(config);
        config.Log(context.Log);
        return config;
    }

    public Dictionary<string, string> Items { get; } = new Dictionary<string, string>();

    public BuildVersion Version { get; }

    public SolutionParameters Solution { get; }

    public ArtifactsParameters Artifacts { get; }

    private Configuration(ISetupContext context, string solutionFilePathPattern = "*.sln", string defaultBuildConfiguration = "Release", DirectoryPath artifactsRootPath = null)
    {
        var solutionPath = context.GetFiles(solutionFilePathPattern).FirstOrDefault();
        
        if(solutionPath == null || !context.FileExists(solutionPath)) 
        {
            throw new Exception("Cant continue without a valid solution file");
        }

        var buildConfiguration = context.Argument("configuration", defaultBuildConfiguration);

        Solution = new SolutionParameters(context, solutionPath, buildConfiguration);
        Version = BuildVersion.DetermineBuildVersion(context);
        Artifacts = new ArtifactsParameters(context, artifactsRootPath ?? context.Directory("artifacts"));
    }

    public void Log(ICakeLog logger) 
    {
        Solution.Log(logger);
    }

}
public enum ArtifactTypeOption {
    Zip,
    WebDeploy,
    NuGet,
    TestResults,
    Other
}

public struct Artifact {
    public ArtifactTypeOption Type { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public FilePath Path { get; set; }

    public Artifact(ArtifactTypeOption type, string name, FilePath path, string category = "none")
    {
        Type = type;
        Name = name;
        Path = path;
        Category = category;
    }

    public override string ToString() {
        return $"{Name} ({Type}/{Category}) - {Path}";
    }
}

public class ArtifactsParameters : List<Artifact> {
    
    private readonly ICakeContext context;
    public DirectoryPath Root { get; set; }

    public DirectoryPath GetRootFor(ArtifactTypeOption type) {
        var directory = $"{Root}/{type:G}/";
        context.EnsureDirectoryExists(directory);
        return context.Directory(directory);
    }

    public ArtifactsParameters(ICakeContext context, DirectoryPath artifactsRootPath) {
        this.context = context;
        Root = context.MakeAbsolute(artifactsRootPath);
        context.EnsureDirectoryExists(Root);
        context.CleanDirectory(Root);
    }

    public void Add(ArtifactTypeOption type, string name, FilePath path) {
        Add(new Artifact(type, name, path));
    }
}

public class SolutionParameters {
    private readonly ICakeContext context;
    public SolutionParserResult Solution { get; }
    public FilePath Path { get; }

    public string BuildConfiguration { get; }

    public IEnumerable<CustomProjectParserResult> Projects { get; } = Enumerable.Empty<CustomProjectParserResult>();
    public IEnumerable<CustomProjectParserResult> WebProjects => Projects.Where(IsWebProject);

    public IEnumerable<CustomProjectParserResult> TestProjects => Projects.Where(IsCliTestProject);
    public IEnumerable<CustomProjectParserResult> NuGetProjects => Projects.Where(IsNuGetPackableProject);
    public IEnumerable<CustomProjectParserResult> ConsoleProjects => Projects.Where(IsConsoleProject);

    public SolutionParameters(ICakeContext context, FilePath solutionPath, string buildConfiguration) {
        this.context = context;
        Path = solutionPath;
        BuildConfiguration = buildConfiguration;
        Solution = context.ParseSolution(Path);
        Projects = Solution.GetProjects().Select(p => context.ParseProject(p.Path, buildConfiguration)).ToList();
    }

    public ISet<Func<CustomProjectParserResult, bool>> WebProjectResolvers { get; } = new HashSet<Func<CustomProjectParserResult, bool>>() {
        p => p.IsWebApplication()
    };

    public ISet<Func<CustomProjectParserResult, bool>> TestProjectResolvers { get; } = new HashSet<Func<CustomProjectParserResult, bool>>() {
        p => p.IsDotNetCliTestProject()
    };

    public ISet<Func<CustomProjectParserResult, bool>> NuGetProjectResolvers { get; } = new HashSet<Func<CustomProjectParserResult, bool>>() {
        p => !string.IsNullOrWhiteSpace(p?.NetCore?.PackageId) && p.NetCore.IsPackable && !p.NetCore.IsWeb
    };

    public ISet<Func<CustomProjectParserResult, bool>> ConsoleProjectResolvers { get; } = new HashSet<Func<CustomProjectParserResult, bool>>() {
        p => p?.OutputType?.Equals("Exe") ?? false
    };

    public SolutionParameters IncludeAsWebProject(Func<CustomProjectParserResult, bool> includeFunc) {
        WebProjectResolvers.Add(includeFunc);
        return this;
    }

    public SolutionParameters IncludeAsTestProject(Func<CustomProjectParserResult, bool> includeFunc) {
        TestProjectResolvers.Add(includeFunc);
        return this;
    }

    public SolutionParameters IncludeAsNuGetProject(Func<CustomProjectParserResult, bool> includeFunc) {
        NuGetProjectResolvers.Add(includeFunc);
        return this;
    }

    public SolutionParameters IncludeAsConsoleProject(Func<CustomProjectParserResult, bool> includeFunc) {
        ConsoleProjectResolvers.Add(includeFunc);
        return this;
    }    

    private bool IsWebProject(CustomProjectParserResult project) {
        return WebProjectResolvers.Any(resolver => resolver(project));
    }

    private bool IsCliTestProject(CustomProjectParserResult project) {
        return TestProjectResolvers.Any(resolver => resolver(project));
    }
    
    private bool IsNuGetPackableProject(CustomProjectParserResult project) {
        return NuGetProjectResolvers.Any(resolver => resolver(project));
    }

    private bool IsConsoleProject(CustomProjectParserResult project) {
        return ConsoleProjectResolvers.Any(resolver => resolver(project));
    }

    public void Log(ICakeLog logger) {
        logger.Information("------\nProjects - Total: {0}, Web: {1}, Test: {2}, Library: {3}, Console: {4}\n------", Projects.Count(), WebProjects.Count(), TestProjects.Count(), NuGetProjects.Count(), ConsoleProjects.Count());
    }
}

const string logo = 
"                               _     _   \n" +
"      _                       (_)   | |  \n" +
"  ___| |_  ___   ____ ____     _  _ | |  \n" +
" /___)  _)/ _ \\ / ___)    \\   | |/ || |  \n" +
"|___ | |_| |_| | |   | | | |  | ( (_| |  \n" +
"(___/ \\___)___/|_|   |_|_|_|  |_|\\____|  \n" +
"                                         ";

Information(logo);