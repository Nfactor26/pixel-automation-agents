using Ductus.FluentDocker.Services;
using Pixel.Automation.Docker.Agent.Handlers.Playwright;

namespace Pixel.Automation.Docker.Agent.Handlers;

/// <summary>
/// Implementation of <see cref="ITestExecutionHandler"/> to execute test cases based on playwright with chrome browser in docker environment
/// </summary>
internal class ChromePlaywrightHandler : PlaywrightHandler
{
    public static string Name = "docker-playwright-chrome";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public ChromePlaywrightHandler(IHostService dockerHost) : base(dockerHost)
    {
    }

    /// </inheritdoc> 
    protected override string GetBrowserName()
    {
        return "chrome";
    }

    /// </inheritdoc> 
    protected override string GetDockerTemplateFile()
    {
        return "playwright-chrome.yml";
    }
}
