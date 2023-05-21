using Ductus.FluentDocker.Services;
using Pixel.Automation.Docker.Agent.Handlers.Playwright;

namespace Pixel.Automation.Docker.Agent.Handlers;

/// <summary>
/// Implementation of <see cref="ITestExecutionHandler"/> to execute test cases based on playwright with edge browser in docker environment
/// </summary>
internal class EdgePlaywrightHandler : PlaywrightHandler
{  
    public static string Name = "docker-playwright-edge";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public EdgePlaywrightHandler(IHostService dockerHost) : base(dockerHost)
    {
    }

    /// </inheritdoc> 
    protected override string GetBrowserName()
    {
        return "edge";
    }

    /// </inheritdoc> 
    protected override string GetDockerTemplateFile()
    {
        return "playwright-edge.yml";
    }

}
