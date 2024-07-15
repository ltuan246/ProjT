//////////////////////////////////////////////////////////////////////
// INSTALL CAKE
// dotnet new tool-manifest
// dotnet tool install Cake.Tool --version 4.0.0
//////////////////////////////////////////////////////////////////////

const string defaultTarget = "Default";
var target = Argument("target", defaultTarget);
var configuration = Argument("configuration", "Debug");

var solutionDirectory = MakeAbsolute(Directory("./"));
var srcDirectory = solutionDirectory.Combine("src");
var testsDirectory = solutionDirectory.Combine("tests");
var outputDirectory = solutionDirectory.Combine("build");

var solutionFile = solutionDirectory.CombineWithFilePath("KISS.sln");
var solutionFullPath = solutionFile.FullPath;
var srcProjectNames = new[]
{
    "KISS.GuardClauses",
    "KISS.Misc",
    "KISS.QueryPredicateBuilder"
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
    {
        var dirsToClean =
                 GetDirectories("./**/bin");
        dirsToClean.Add(GetDirectories("./**/obj"));

        CleanDirectories(dirsToClean);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        // disable parallel restore to work around apparent bugs in restore
        var restoreSettings = new DotNetRestoreSettings
        {
            DisableParallel = true
        };
        DotNetRestore(solutionFullPath, restoreSettings);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var settings = new DotNetBuildSettings
        {
            NoRestore = true,
            Configuration = configuration
        };

        DotNetBuild(solutionFullPath, settings);
    });


Task("Test")
    .IsDependentOn("Build")
    .DoesForEach(
        items: GetFiles("./**/*.Tests.csproj"),
        action: (testProject) =>
    {
        var settings = new DotNetTestSettings
        {
            NoBuild = true,
            NoRestore = true,
            Configuration = configuration,
        };
        DotNetTest(testProject.FullPath, settings);
    })
    .DeferOnError();

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);