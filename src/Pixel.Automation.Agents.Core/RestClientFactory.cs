using Dawn;
using Pixel.Automation.Agents.Core.Contracts;
using RestSharp;

namespace Pixel.Automation.Agents.Core;

/// <summary>
/// Default implementation of <see cref="IRestClientFactory"/>
/// </summary>
public class RestClientFactory : IRestClientFactory
{      
    private RestClient restClient;
    private readonly string serviceEndPoint;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="signInManager"></param>
    public RestClientFactory(string serviceEndPoint)
    {
        this.serviceEndPoint = Guard.Argument(serviceEndPoint, nameof(serviceEndPoint)).NotNull().NotEmpty();
    }

    ///<inheritdoc/>
    public RestClient GetOrCreateClient()
    {
        if (restClient == null)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.serviceEndPoint)
            };
            restClient = new RestClient(httpClient, new RestClientOptions()
            {
                BaseUrl = new Uri(this.serviceEndPoint)
            });
        }
        return restClient;
    }
}

