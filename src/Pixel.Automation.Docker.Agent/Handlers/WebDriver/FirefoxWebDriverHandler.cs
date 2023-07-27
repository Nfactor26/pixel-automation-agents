using Ductus.FluentDocker.Services;
using Pixel.Automation.Agents.Core;
using Pixel.Automation.Docker.Agent.Handlers.WebDriver;

namespace Pixel.Automation.Docker.Agent.Handlers;

/// <summary>
/// Implementation of <see cref="ITestExecutionHandler"/> to execute test cases based on webdriver with firefox browser in docker environment
/// </summary>
internal class FirefoxWebDriverHandler : WebDriverHandler
{
    public static string Name = "docker-webdriver-firefox";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public FirefoxWebDriverHandler(IHostService dockerHost, TemplateHandler templateHandler) : base(dockerHost, templateHandler)
    { 
    }

    /// </inheritdoc> 
    protected override string GetBrowserName()
    {
        return "firefox";
    }

    /// </inheritdoc> 
    protected override string GetNetworkTemplateName()
    {
        return "pixel-webdriver-firefox-{0}";
    }

    /// </inheritdoc>
    protected override void AddDefaultParameters(Dictionary<string, string> defaultParameters)
    {
        defaultParameters.Add("selenium-standalone-image", " selenium/standalone-firefox:latest");
        defaultParameters.Add("grid-address", "http://firefox-standalone:4444");
        base.AddDefaultParameters(defaultParameters);
    }
}
