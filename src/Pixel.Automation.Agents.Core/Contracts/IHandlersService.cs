namespace Pixel.Automation.Agents.Core.Contracts;

/// <summary>
/// Service contract for <see cref="TemplateHandler"/>
/// </summary>
public interface IHandlersService
{
    /// <summary>
    /// Get handler with a given name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<TemplateHandler> GetHandlerByNameAsync(string name);

    /// <summary>
    /// Get all the available template handlers async
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<TemplateHandler>> GetAllHandlersAsync();

    /// <summary>
    /// Download the docker compose template file with a given name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task DownloadComposeFileAsync(string fileName);
}
