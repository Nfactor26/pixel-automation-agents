using Ductus.FluentDocker.Services;
using Pixel.Automation.Docker.Agent.Handlers.WebDriver;

namespace Pixel.Automation.Docker.Agent.Handlers;

/// <summary>
/// Implementation of <see cref="ITestExecutionHandler"/> to execute test cases based on webdriver with chrome browser in docker environment
/// </summary>
internal class ChromeWebDriverHandler : WebDriverHandler
{
    public static string Name = "docker-webdriver-chrome";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public ChromeWebDriverHandler(IHostService dockerHost) : base(dockerHost)
    {

    }

    /// </inheritdoc> 
    protected override string GetBrowserName()
    {
        return "chrome";
    }

    /// </inheritdoc> 
    protected override string GetNetworkTemplateName()
    {
        return "pixel-webdriver-chrome-{0}";
    }

    /// </inheritdoc> 
    protected override string GetDockerTemplateFile()
    {
        return "webdriver-chrome-standalone.yml";
    }
}
