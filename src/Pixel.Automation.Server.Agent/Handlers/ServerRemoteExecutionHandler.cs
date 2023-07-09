using Dawn;
using Microsoft.Extensions.Configuration;
using Pixel.Automation.Agents.Core.Contracts;
using Serilog;
using System.Diagnostics;

namespace Pixel.Automation.Server.Agent.Handlers;

/// <summary>
/// Execution handler to run test cases on server machine
/// </summary>
internal class ServerRemoteExecutionHandler : ITestExecutionHandler
{
    private readonly ILogger logger = Log.ForContext<ServerRemoteExecutionHandler>(); 
    private readonly IConfiguration config;
  
    public static string Name = "server-remote";
   
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="config"></param>
    public ServerRemoteExecutionHandler(IConfiguration config)
    {
        this.config = Guard.Argument(config, nameof(config)).NotNull().Value;
    }

    /// </inheritdoc>    
    public async Task ExecuteTestAsync(string templateName, Dictionary<string, string> arguments)
    {
        try
        {
            string executablePath = config["pixelExecutable"] ?? throw new ArgumentNullException("No value provided for 'pixel_run_exeutable' configuration setting");
            string executableName = Path.GetFileName(executablePath);
            if (Process.GetProcessesByName(executableName).Any())
            {
                throw new Exception($"{executableName} is already running");
            }

            var process = Process.Start(new ProcessStartInfo()
            {
                WorkingDirectory = Path.GetDirectoryName(executablePath),
                FileName = executablePath,
                Arguments = $"run template \"{templateName}\""
            }) ?? throw new Exception("Process failed to start");

            logger.Information("pixel-run was started with process Id : {0}", process.Id);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "There was an error while executing template : {0}", templateName);
        }
    }
}
