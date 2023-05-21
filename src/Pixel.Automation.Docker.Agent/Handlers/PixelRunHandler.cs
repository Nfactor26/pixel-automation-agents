using Ductus.FluentDocker.Services;
using Pixel.Automation.Agents.Core.Contracts;

namespace Pixel.Automation.Docker.Agent.Handlers;

/// <summary>
/// Implementation of <see cref="ITestExecutionHandler"/> to execute test case in a docker environment 
/// which requires only pixel-run to be started in docker container
/// </summary>
internal class PixelRunHandler : DockerComposeExecutionHandler
{
    private readonly string dockerTemplateFile = "docker-pixel-run.yml";
 
    public static string Name = "docker-pixel-run";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="dockerHost"></param>
    public PixelRunHandler(IHostService dockerHost) : base(dockerHost)
    {
    }

    /// </inheritdoc> 
    protected override IEnumerable<INetworkService> CreateRequiredNetworks(string templateName)
    {
        return Enumerable.Empty<INetworkService>();
    }

    /// </inheritdoc> 
    protected async override Task<string> GetComposeFile(string templateName, IEnumerable<INetworkService> networks)
    {
        var templateToUse = Path.Combine(Environment.CurrentDirectory, "Templates", dockerTemplateFile);
        if (!File.Exists(templateToUse))
        {
            throw new FileNotFoundException("File doesn't exist", templateToUse);
        }
        string templateContent = await File.ReadAllTextAsync(templateToUse);
        string generatedContent = string.Format(templateContent, networks.ElementAt(0).Name, templateName);
        string templateDirectory = Path.Combine(Environment.CurrentDirectory, "Temp", templateName);
        Directory.CreateDirectory(templateDirectory);
        var saveToLocation = Path.Combine(templateDirectory, dockerTemplateFile);
        if (File.Exists(saveToLocation))
        {
            File.Delete(saveToLocation);
        }
        await File.WriteAllTextAsync(saveToLocation, generatedContent);
        return saveToLocation;
    }
}
