using Ductus.FluentDocker.Services;
using Pixel.Automation.Agents.Core;
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
    public EdgeWebDriverHandler(IHostService dockerHost, TemplateHandler templateHandler) : base(dockerHost, templateHandler)
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

    protected override void AddDefaultParameters(Dictionary<string, string> defaultParameters)
    {
        defaultParameters.Add("selenium-standalone-image", " selenium/standalone-edge:latest");
        defaultParameters.Add("grid-address", "http://edge-standalone:4444");
        base.AddDefaultParameters(defaultParameters);
    }
}
