using Microsoft.Extensions.Configuration;

namespace Pixel.Automation.Server.Agent.Handlers;

/// <summary>
/// Test Execution handler for execution in Linux environment
/// </summary>
internal class LinuxExecutionHandler : ExecutionHandler
{
    public static string Name = "linux-server";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="config"></param>
    public LinuxExecutionHandler(IConfiguration config) : base(config)
    {
    }
}
