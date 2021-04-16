using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lekker.PackageDeployer.Core.Models.Soap
{
    public class PublishAction : ISoapAction
    {
        public PublishAction(string url, IEnumerable<string> packageNames, bool mergeWithExisting = true)
        {
            if (!packageNames.Any()) throw new DeploymentActionSoapException("No packages specified");

            Uri = new Uri(url);

            _packageNames = packageNames;
            _mergeWithExisting = mergeWithExisting;
        }

        public async Task GetRequestPayload(Stream requestStream)
        {
            string payload =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                "<soap:Body>" +
                "<PublishPackages xmlns=\"http://acumatica.com\">" +
                "<packageNames>";

            foreach (string s in _packageNames)
            {
                payload += $"<string>{s}</string>";
            }

            payload +=
                "</packageNames>" +
                $"<mergeWithExistingPackages>{_mergeWithExisting.ToString().ToLower()}</mergeWithExistingPackages>" +
                "</PublishPackages>" +
                "</soap:Body>" +
                "</soap:Envelope>";

            await requestStream.WriteAsync(Encoding.UTF8.GetBytes(payload));
        }

        public string ActionName => "\"http://acumatica.com/PublishPackages\"";
        public Uri Uri { get; }

        private readonly IEnumerable<string> _packageNames;
        private readonly bool _mergeWithExisting;
    }
}