using Microsoft.Extensions.Configuration;

namespace Pixel.Automation.Server.Agent.Handlers;

/// <summary>
/// Test execution handler for execution in Windows environment
/// </summary>
internal class WindowsExecutionHandler : ExecutionHandler
{
    public static string Name = "windows-server";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="config"></param>
    public WindowsExecutionHandler(IConfiguration config) : base(config)
    {
    }
}
