using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Services;

namespace Pixel.Automation.Docker.Agent.Handlers.WebDriver;

/// <summary>
/// Base implementation of <see cref="ITestExecutionHandler"/> to execute test cases based on webdriver in docker environment
/// </summary>
internal abstract class WebDriverHandler : DockerComposeExecutionHandler
{
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public WebDriverHandler(IHostService dockerHost): base(dockerHost)
    {

    }

    /// <summary>
    /// Get the name of the browser for the handler
    /// </summary>
    /// <returns></returns>
    protected abstract string GetBrowserName();

    /// <summary>
    /// Get the template for the network name
    /// </summary>
    /// <returns></returns>
    protected abstract string GetNetworkTemplateName();

    /// <summary>
    /// Get the template file for docker compose
    /// </summary>
    /// <returns></returns>
    protected abstract string GetDockerTemplateFile();

    /// </inheritdoc> 
    protected override IEnumerable<INetworkService> CreateRequiredNetworks(string templateName)
    {
        var networkName = string.Format(GetNetworkTemplateName(), templateName);
        if (dockerHost.GetNetworks().Any(n => n.Name.Equals(networkName)))
        {
            logger.Information("Network {0} already exists", networkName);
            return dockerHost.GetNetworks().Where(n => n.Name.Equals(networkName));  
        }
        var network = dockerHost.CreateNetwork(networkName, new NetworkCreateParams(), false);
        logger.Information("Network {0} was created", network.Name);
        return new[] { network };
    }

    /// </inheritdoc> 
    protected override async Task<string> GetComposeFile(string templateName, IEnumerable<INetworkService> networks)
    {
        var templateToUse = Path.Combine(Environment.CurrentDirectory, "Templates", GetDockerTemplateFile());
        if (!File.Exists(templateToUse))
        {
            throw new FileNotFoundException("File doesn't exist", templateToUse);
        }
        string templateContent = await File.ReadAllTextAsync(templateToUse);
        string generatedContent = string.Format(templateContent, networks.ElementAt(0).Name, templateName);
        string templateDirectory = Path.Combine(Environment.CurrentDirectory, "Temp", $"{templateName}-with-{GetBrowserName()}");
        Directory.CreateDirectory(templateDirectory);
        var saveToLocation = Path.Combine(templateDirectory, GetDockerTemplateFile());
        if (File.Exists(saveToLocation))
        {
            File.Delete(saveToLocation);
        }
        await File.WriteAllTextAsync(saveToLocation, generatedContent);
        return saveToLocation;
    }
}
