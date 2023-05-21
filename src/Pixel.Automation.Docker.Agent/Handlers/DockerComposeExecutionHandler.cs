using Dawn;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services;
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

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public DockerComposeExecutionHandler(IHostService dockerHost)
    {
        this.dockerHost = Guard.Argument(dockerHost, nameof(dockerHost)).NotNull().Value;  
    }
   
    /// </inheritdoc>  
    public virtual async Task ExecuteTestAsync(string templateName)
    {       
        var networks = CreateRequiredNetworks(templateName);
        try
        {
            var file = await GetComposeFile(templateName, networks);

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
    protected abstract Task<string> GetComposeFile(string templateName, IEnumerable<INetworkService> networks);

    /// <summary>
    /// Create networks required for the execution
    /// </summary>
    /// <param name="templateName"></param>
    /// <returns></returns>
    protected abstract IEnumerable<INetworkService> CreateRequiredNetworks(string templateName);

}
