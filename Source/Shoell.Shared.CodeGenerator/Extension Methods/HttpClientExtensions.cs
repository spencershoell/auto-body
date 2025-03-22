using System.Xml;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator
{
    public static class HttpClientExtensions
    {
        public async static Task<Stream> GetHtmlStreamAsync<TEntity>(this HttpClient httpClient, string uri, TEntity entityModel, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsJsonAsync(uri, entityModel, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }
    }
}