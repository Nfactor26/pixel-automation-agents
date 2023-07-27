using Dawn;
using Pixel.Automation.Agents.Core.Contracts;
using RestSharp;

namespace Pixel.Automation.Agents.Core;

/// <summary>
/// Default implementation of <see cref="IHandlersService"/>
/// </summary>
public class HandlersService : IHandlersService
{
    private readonly IRestClient restClient;
    private readonly string templatesFolder = Path.Combine(Environment.CurrentDirectory, "Templates");

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="restClientFactory"></param>
    public HandlersService(IRestClientFactory restClientFactory)
    {
        Guard.Argument(restClientFactory, nameof(restClientFactory)).NotNull();
        this.restClient = restClientFactory.GetOrCreateClient();
        if(!Directory.Exists(templatesFolder))
        {
            Directory.CreateDirectory(templatesFolder);
        }
    }

    /// <inheritdoc/>   
    public async Task DownloadComposeFileAsync(string fileName)
    {
        RestRequest restRequest = new RestRequest($"api/composefiles/name/{fileName}");
        var dataFile = await restClient.GetAsync<DataFile>(restRequest, CancellationToken.None);
        using (MemoryStream ms = new MemoryStream(dataFile.Bytes))
        {
            using (FileStream fs = new FileStream(Path.Combine(templatesFolder, fileName), FileMode.Create))
            {
                ms.Seek(0, SeekOrigin.Begin);
                ms.CopyTo(fs);
            }
        }
    }

    /// <inheritdoc/> 
    public async Task<IEnumerable<TemplateHandler>> GetAllHandlersAsync()
    {
        RestRequest restRequest = new RestRequest($"api/handlers");
        var templateHandlers = await restClient.GetAsync<IEnumerable<TemplateHandler>>(restRequest, CancellationToken.None);    
        return templateHandlers;
    }

    /// <inheritdoc/> 
    public async Task<TemplateHandler> GetHandlerByNameAsync(string name)
    {
        RestRequest restRequest = new RestRequest($"api/handlers/name/{name}");
        var handler = await restClient.GetAsync<TemplateHandler>(restRequest, CancellationToken.None);
        return handler;
    }
}
