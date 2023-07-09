// See https://aka.ms/new-console-template for more information
using Dawn;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Pixel.Automation.Agents.Core;
using Pixel.Automation.Agents.Core.Contracts;
using Pixel.Automation.Docker.Agent.Handlers;
using Serilog;
using Serilog.Events;
using System.Runtime.InteropServices;

Log.Logger = new LoggerConfiguration()
             .WriteTo.File("logs/pixel-agent-.txt", restrictedToMinimumLevel: LogEventLevel.Verbose,
               outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [Thread:{ThreadId}] [{SourceContext:l}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
             .WriteTo.Console(LogEventLevel.Information, "{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}")
             .CreateLogger();
var logger = Log.ForContext<Program>();

using IHost host = Host.CreateDefaultBuilder(args).Build();

var switchMappings = new Dictionary<string, string>()
           {
               { "--name", "name" },
               { "--group", "group" },
               { "--hubEndPoint", "hubEndPoint" }
           };

IConfiguration config = new ConfigurationBuilder()
    .AddCommandLine(args, switchMappings)
    .AddJsonFile("appsettings.json", true, false)
    .AddEnvironmentVariables()
    .Build();

string agentName = config.GetValue<string>("name") ?? throw new Exception("name is not configured");
string agentGroup = config.GetValue<string>("group") ?? "server-agents-default-group";
string hubEndPoint = config.GetValue<string>("hubEndPoint") ?? throw new Exception("hubEndPoint is not configured");

Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Temp"));

IHostService dockerHost = new Builder().UseHost().UseNative().Build();

var agent = new
{
    Name = agentName,
    Group = agentGroup,
    MachineName = Environment.MachineName,
    OSDescription = RuntimeInformation.OSDescription,
    RegisteredHadlers = new string[]
    {
        PixelRunHandler.Name,
        ChromeWebDriverHandler.Name,
        EdgeWebDriverHandler.Name,
        FirefoxWebDriverHandler.Name,
        ChromePlaywrightHandler.Name,
        EdgePlaywrightHandler.Name,
        FirefoxPlaywrightHandler.Name
    }
};

var connection = new HubConnectionBuilder()
    .WithUrl(hubEndPoint).WithAutomaticReconnect(new ConnectionRetryPolicy())
    .Build();
await connection.StartAsync();
await connection.InvokeAsync("RegisterAgent", agent);

connection.On("CanExecuteNew", () =>
{
    bool canExecuteNew = true;
    logger.Information("Replied {0} to CanExecuteNew query", canExecuteNew);
    return canExecuteNew;
});

connection.On<string, string, string>("ExecuteTemplate", async (template, handler, arguments) =>
{
    try
    {
        Guard.Argument(template, nameof(template)).NotNull().NotEmpty();
        Guard.Argument(handler, nameof(handler)).NotNull().NotEmpty();
        logger.Information("Received request to execute template {0} with handler {1}", template, handler);
        ITestExecutionHandler executionHadler;
        switch (handler)
        {
            case "docker-pixel-run":
                executionHadler = new PixelRunHandler(dockerHost);
                break;
            case "docker-webdriver-chrome":
                executionHadler = new ChromeWebDriverHandler(dockerHost);
                break;
            case "docker-webdriver-edge":
                executionHadler = new EdgeWebDriverHandler(dockerHost);
                break;
            case "docker-webdriver-firefox":
                executionHadler = new FirefoxWebDriverHandler(dockerHost);
                break;
            case "docker-playwright-chrome":
                executionHadler = new ChromePlaywrightHandler(dockerHost);
                break;
            case "docker-playwright-edge":
                executionHadler = new EdgePlaywrightHandler(dockerHost);
                break;
            case "docker-playwright-firefox":
                executionHadler = new FirefoxPlaywrightHandler(dockerHost);
                break;
            default:
                throw new NotSupportedException($"Handler {handler} is not supported");
        }
        Dictionary<string, string> handlerArgs = new();
        if (!string.IsNullOrEmpty(arguments))
        {
            foreach (var arg in arguments.Split(','))
            {
                var keyValuePair = arg.Split('=');
                if(keyValuePair.Length != 2)
                {
                    throw new ArgumentException($"Argument {arg} could not be parsed.");
                }
                handlerArgs.Add(keyValuePair[0], keyValuePair[1]);
            }
        }
        _ = executionHadler.ExecuteTestAsync(template, handlerArgs);
        await Task.CompletedTask;
    }
    catch (Exception ex)
    {
        logger.Error(ex, "An error occured while trying to process requeste to execute template {0} with handler {1}", template, handler);
    }
});

connection.Closed += OnConnectionClosed;
connection.Reconnected += OnReconnected;

async Task OnReconnected(string arg)
{
    try
    {
        await connection.InvokeAsync("AgentReconnected", agent);
        logger.Information("Agent is reconnected now - {0}", arg);
    }
    catch (Exception ex)
    {
        logger.Error(ex, "There was an error while processing reconnected event");
    }
}

async Task OnConnectionClosed(Exception ex)
{
    logger.Error(ex, "Agent is disconencted now");
    await Task.CompletedTask;
}

await host.RunAsync();

logger.Information("Agent is shutting down");
await connection.InvokeAsync("DeRegisterAgent", agent);
