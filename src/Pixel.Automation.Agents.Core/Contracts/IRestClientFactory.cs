using RestSharp;

namespace Pixel.Automation.Agents.Core.Contracts
{
    /// <summary>
    /// Factory for creating a <see cref="RestClient"/>
    /// </summary>
    public interface IRestClientFactory
    {
        /// <summary>
        /// Get an instance of <see cref="RestClient"/>.
        /// A new instance will be created if it doesn't already exist.
        /// </summary>
        /// <returns></returns>
        RestClient GetOrCreateClient();
    }
}
