using Ductus.FluentDocker.Services;
using Pixel.Automation.Agents.Core;
using Pixel.Automation.Agents.Core.Contracts;

namespace Pixel.Automation.Docker.Agent.Handlers;

/// <summary>
/// Implementation of <see cref="ITestExecutionHandler"/> to execute test case in a docker environment 
/// which requires only pixel-run to be started in docker container
/// </summary>
internal class PixelRunHandler : DockerComposeExecutionHandler
{
    public static string Name = "docker-pixel-run";

    private readonly Dictionary<string, string> defaultParameters = new Dictionary<string, string>()
    {
        { "pixel-run-image", "pixel-test-runner:latest" }
    };  

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public PixelRunHandler(IHostService dockerHost, TemplateHandler templateHandler) : base(dockerHost, templateHandler)
    {      
       AddDefaultParameters(defaultParameters);
    }

    /// </inheritdoc> 
    protected override IEnumerable<INetworkService> CreateRequiredNetworks(string templateName, Dictionary<string, string> parameters)
    {
        return Enumerable.Empty<INetworkService>();
    }

    /// </inheritdoc> 
    protected async override Task<string> GetComposeFile(string templateName, IEnumerable<INetworkService> networks, Dictionary<string, string> parameters)
    {
        var templateToUse = Path.Combine(Environment.CurrentDirectory, "Templates", templateHandler.DockerComposeFileName);        
        if (!File.Exists(templateToUse))
        {
            throw new FileNotFoundException("File doesn't exist", templateToUse);
        }
        string templateContent = await File.ReadAllTextAsync(templateToUse);      
        string generatedContent = string.Format(templateContent, templateHandler.Parameters["pixel-run-image"], templateName);
        string templateDirectory = Path.Combine(Environment.CurrentDirectory, "Temp", templateName);
        Directory.CreateDirectory(templateDirectory);
        var saveToLocation = Path.Combine(templateDirectory, templateHandler.DockerComposeFileName);
        if (File.Exists(saveToLocation))
        {
            File.Delete(saveToLocation);
        }
        await File.WriteAllTextAsync(saveToLocation, generatedContent);
        return saveToLocation;
    }
}
