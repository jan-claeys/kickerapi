
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV1s;

var adoNetClient = new ADotNetClient();

var githubPipeline = new GithubPipeline
{
    Name = "Mengblad Build",

    OnEvents = new Events
    {
        Push = new PushEvent
        {
            Branches = new string[] { "master" }
        },
        PullRequest = new PullRequestEvent
        {
            Branches = new string[] { "master" }
        }
    },

    Jobs = new Jobs
    {
        Build = new BuildJob
        {
            RunsOn = BuildMachines.WindowsLatest,

            Steps = new List<GithubTask>
            {
                new CheckoutTaskV2
                {
                    Name = "Checkout Code",
                },
                new SetupDotNetTaskV1
                {
                    Name = "Installing .NET",
                    TargetDotNetVersion = new TargetDotNetVersion
                    {
                        DotNetVersion = "6.0.408"
                    }
                },
                new RestoreTask
                {
                    Name = "Restore .NET Packages",
                },
                new DotNetBuildTask
                {
                    Name = "Building Solution"
                },
                new TestTask
                {
                    Name = "Run Tests"
                }

            }
        }
    }
};

adoNetClient.SerializeAndWriteToFile(githubPipeline, "../../../../.github/workflows/dotnet.yml");