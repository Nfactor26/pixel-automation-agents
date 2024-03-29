﻿using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Services;
using Pixel.Automation.Agents.Core;

namespace Pixel.Automation.Docker.Agent.Handlers.WebDriver;

/// <summary>
/// Base implementation of <see cref="ITestExecutionHandler"/> to execute test cases based on webdriver in docker environment
/// </summary>
internal abstract class WebDriverHandler : DockerComposeExecutionHandler
{
    protected readonly Dictionary<string, string> defaultParameters = new()
    {
        { "pixel-run-image", "pixel-test-runner:latest" }
    };


    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public WebDriverHandler(IHostService dockerHost, TemplateHandler templateHandler) : base(dockerHost, templateHandler)
    {
       AddDefaultParameters(defaultParameters);
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
    protected virtual string GetDockerTemplateFile()
    {
        return templateHandler.DockerComposeFileName;
    }

    /// </inheritdoc> 
    protected override IEnumerable<INetworkService> CreateRequiredNetworks(string templateName, Dictionary<string, string> parameters)
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
    protected override async Task<string> GetComposeFile(string templateName, IEnumerable<INetworkService> networks, Dictionary<string, string> parameters)
    {
        var templateToUse = Path.Combine(Environment.CurrentDirectory, "Templates", GetDockerTemplateFile());
        if (!File.Exists(templateToUse))
        {
            throw new FileNotFoundException("File doesn't exist", templateToUse);
        }
        string templateContent = await File.ReadAllTextAsync(templateToUse);
        
        string generatedContent = string.Format(templateContent,  templateHandler.Parameters["selenium-standalone-image"], templateHandler.Parameters["pixel-run-image"],
            networks.ElementAt(0).Name, templateHandler.Parameters["grid-address"], templateName);
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
