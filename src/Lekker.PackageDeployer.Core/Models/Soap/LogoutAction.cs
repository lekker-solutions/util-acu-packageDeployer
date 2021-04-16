using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lekker.PackageDeployer.Core.Models.Soap
{
    public class LogoutAction : ISoapAction
    {
        public LogoutAction(string url)
        {
            Uri = new Uri(url);
        }

        public LogoutAction(Uri uri)
        {
            Uri = uri;
        }

        public async Task GetRequestPayload(Stream requestStream)
        {
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                "<soap:Body>" +
                "<Logout xmlns=\"http://acumatica.com\" />" +
                "</soap:Body>" +
                "</soap:Envelope>";

            await requestStream.WriteAsync(Encoding.UTF8.GetBytes(payload));
        }

        public string ActionName => "\"http://acumatica.com/Logout\"";
        public Uri Uri { get; }
    }
}