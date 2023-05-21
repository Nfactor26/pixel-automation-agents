using Ductus.FluentDocker.Services;
using Pixel.Automation.Docker.Agent.Handlers.WebDriver;

namespace Pixel.Automation.Docker.Agent.Handlers;

/// <summary>
/// Implementation of <see cref="ITestExecutionHandler"/> to execute test cases based on webdriver with edge browser in docker environment
/// </summary>
internal class EdgeWebDriverHandler : WebDriverHandler
{       
    public static string Name = "docker-webdriver-edge";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public EdgeWebDriverHandler(IHostService dockerHost) : base(dockerHost)
    { 
    }

    /// </inheritdoc> 
    protected override string GetBrowserName()
    {
        return "edge";
    }

    /// </inheritdoc> 
    protected override string GetNetworkTemplateName()
    {
        return "pixel-webdriver-edge-{0}";
    }

    /// </inheritdoc> 
    protected override string GetDockerTemplateFile()
    {
        return "webdriver-edge-standalone.yml";
    }
}
