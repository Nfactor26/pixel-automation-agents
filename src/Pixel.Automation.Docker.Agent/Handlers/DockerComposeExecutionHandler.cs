using Dawn;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services;
using Pixel.Automation.Agents.Core;
using Pixel.Automation.Agents.Core.Contracts;
using Serilog;

namespace Pixel.Automation.Docker.Agent.Handlers;

/// <summary>
/// Base implementation of <see cref="ITestExecutionHandler"/> to execute test cases in docker environment
/// </summary>
internal abstract class DockerComposeExecutionHandler : ITestExecutionHandler
{
    protected readonly ILogger logger = Log.ForContext<DockerComposeExecutionHandler>();
    protected readonly IHostService dockerHost;
    protected readonly DockerTemplateHandler templateHandler;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public DockerComposeExecutionHandler(IHostService dockerHost, TemplateHandler templateHandler)
    {
        Guard.Argument(templateHandler, nameof(templateHandler)).NotNull().Compatible<DockerTemplateHandler>();
        this.dockerHost = Guard.Argument(dockerHost, nameof(dockerHost)).NotNull().Value;
        this.templateHandler = templateHandler as DockerTemplateHandler;
    }
   
    /// </inheritdoc>  
    public virtual async Task ExecuteTestAsync(string templateName)
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
                        container.WaitForStopped();
                    }
                }
                logger.Information("Execution is completed now");
            }
            File.Delete(file);
        }
        catch(Exception ex)
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

    /// <summary>
    /// Get the path to the docker compose file
    /// </summary>
    /// <param name="templateName"></param>
    /// <param name="networks"></param>
    /// <returns></returns>
    protected abstract Task<string> GetComposeFile(string templateName, IEnumerable<INetworkService> networks, Dictionary<string, string> parameters);

    /// <summary>
    /// Create networks required for the execution
    /// </summary>
    /// <param name="templateName"></param>
    /// <returns></returns>
    protected abstract IEnumerable<INetworkService> CreateRequiredNetworks(string templateName, Dictionary<string, string> parameters);

    /// <summary>
    /// Add default values to template handler -> parameters if missing
    /// </summary>
    /// <param name="defaultParameters"></param>
    protected virtual void AddDefaultParameters(Dictionary<string, string> defaultParameters)
    {
        foreach(var parameter in defaultParameters)
        {
            if(!templateHandler.Parameters.ContainsKey(parameter.Key))
            {
                templateHandler.Parameters.Add(parameter.Key, parameter.Value);
            }
        }
    }

}
