﻿using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Pixel.Automation.Agents.Core;

namespace Pixel.Automation.Docker.Agent.Handlers.Playwright;

/// <summary>
/// Base implementation of <see cref="ITestExecutionHandler"/> to execute test cases based on playwright in docker environment
/// </summary>
internal abstract class PlaywrightHandler : DockerComposeExecutionHandler
{
    protected readonly Dictionary<string, string> defaultParameters = new Dictionary<string, string>()
    {
        { "pixel-run-image", "pixel-test-runner:latest" }
    };

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    /// <param name="templateHandler"></param>
    public PlaywrightHandler(IHostService dockerHost, TemplateHandler templateHandler) : base(dockerHost, templateHandler)
    {
        AddDefaultParameters(defaultParameters);
    }

    /// <summary>
    /// Get the template file for docker compose
    /// </summary>
    /// <returns></returns>
    protected virtual string GetDockerTemplateFile()
    {
        return templateHandler.DockerComposeFileName;
    }

    protected abstract string GetBrowserName();

    public override async Task ExecuteTestAsync(string templateName)
    {
        var networks = CreateRequiredNetworks(templateName, templateHandler.Parameters);
        try
        {
            var file = await GetComposeFile(templateName, networks, templateHandler.Parameters);

            using (var svc = new Ductus.FluentDocker.Builders.Builder()
                              .UseContainer()
                              .WithName(templateName)
                              .UseCompose()
                              .FromFile(file)                             
                              .Build().Start())
            {
                logger.Information("Docker compose file : {0} is started now", file);
                foreach (var container in svc.Containers)
                {
                    if (container.Name.Contains("pixel-test-runner"))
                    {
                        try
                        {
                            //we need xvfb if the browser is not running in headless mode. However, this command doesn't work with entrypoint.
                            //Hence, we had to override ExecuteTestAsync method for Playwright instead of using this as entrypoint
                            //Also, container needs to be configured as interactive (tty and stdin_open) to execute commands
                            container.Execute($"xvfb-run -a dotnet pixel-run.dll run template {templateName}");
                        }                        
                        finally
                        {
                            container.Stop();
                        }
                    }
                }
                logger.Information("Execution is completed now");
            }
            File.Delete(file);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "There was an error while executing template : {0}", templateName);
        }
        finally
        {
            foreach (var network in networks)
            {
                network.Dispose();
            }
        }
        await Task.CompletedTask;
    }


    protected override IEnumerable<INetworkService> CreateRequiredNetworks(string templateName, Dictionary<string, string> arguments)
    {
        return Enumerable.Empty<INetworkService>();
    }

    protected async override Task<string> GetComposeFile(string templateName, IEnumerable<INetworkService> networks, Dictionary<string, string> arguments)
    {
        var templateToUse = Path.Combine(Environment.CurrentDirectory, "Templates", GetDockerTemplateFile());
        if (!File.Exists(templateToUse))
        {
            throw new FileNotFoundException("File doesn't exist", templateToUse);
        }
        string templateContent = await File.ReadAllTextAsync(templateToUse);
        string generatedContent = string.Format(templateContent, templateHandler.Parameters["pixel-run-image"], templateName);
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
