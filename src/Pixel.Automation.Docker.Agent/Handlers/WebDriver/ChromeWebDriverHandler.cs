using Ductus.FluentDocker.Services;
using Pixel.Automation.Agents.Core;
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
    public ChromeWebDriverHandler(IHostService dockerHost, TemplateHandler templateHandler) : base(dockerHost, templateHandler)
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

    protected override void AddDefaultParameters(Dictionary<string, string> defaultParameters)
    {
        defaultParameters.Add("selenium-standalone-image", " selenium/standalone-chrome:latest");
        defaultParameters.Add("grid-address", "http://chrome-standalone:4444");
        base.AddDefaultParameters(defaultParameters);
    }
}
