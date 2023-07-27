using Ductus.FluentDocker.Services;
using Pixel.Automation.Agents.Core;
using Pixel.Automation.Docker.Agent.Handlers.Playwright;

namespace Pixel.Automation.Docker.Agent.Handlers;

/// <summary>
/// Implementation of <see cref="ITestExecutionHandler"/> to execute test cases based on playwright with firefox browser in docker environment
/// </summary>
internal class FirefoxPlaywrightHandler : PlaywrightHandler
{  
    public static string Name = "docker-playwright-firefox";

    /// <summary>
    /// contructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public FirefoxPlaywrightHandler(IHostService dockerHost, TemplateHandler templateHandler) : base(dockerHost, templateHandler)
    {
    }

    /// </inheritdoc> 
    protected override string GetBrowserName()
    {
        return "firefox";
    }
}
