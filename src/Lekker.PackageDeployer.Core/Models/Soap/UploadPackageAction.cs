using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lekker.PackageDeployer.Core.Models.Soap
{
    public class UploadPackageAction : ISoapAction
    {
        public string ActionName => "\"http://acumatica.com/UploadPackage\"";
        public Uri Uri { get; }

        public UploadPackageAction(string url, string packageName, MemoryStream packageContents, bool replaceIfPackageExists)
        {
            Uri = new Uri(url);
            if (string.IsNullOrWhiteSpace(packageName)) throw new DeploymentActionSoapException("No Username Specified");
            if ((packageContents?.Length ?? 0) == 0) throw new DeploymentActionSoapException("No package contents defined");

            _packageName = packageName;
            _packageContents = packageContents;
            _replaceIfPackageExists = replaceIfPackageExists;
        }

        public async Task GetRequestPayload(Stream requestStream)
        {
            string payload1 = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                              "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                              "<soap:Body>" +
                              "<UploadPackage xmlns=\"http://acumatica.com\">" +
                              $"<packageName>{_packageName}</packageName>" +
                              "<packageContents>";

            string payload2 = "</packageContents>" +
                              $"<replaceIfPackageExists>{_replaceIfPackageExists.ToString().ToLower()}</replaceIfPackageExists>" +
                              "</UploadPackage>" +
                              "</soap:Body>" +
                              "</soap:Envelope>";

            var encoding = new UTF8Encoding(false);

            await requestStream.WriteAsync(encoding.GetBytes(payload1));
            await requestStream.WriteAsync(_packageContents.ToArray());
            await requestStream.WriteAsync(encoding.GetBytes(payload2));

            await requestStream.FlushAsync();

            await _packageContents.DisposeAsync();
        }


        private readonly string _packageName;
        private readonly MemoryStream _packageContents;
        private readonly bool _replaceIfPackageExists;
    }
}