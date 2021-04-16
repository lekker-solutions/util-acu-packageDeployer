using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Lekker.PackageDeployer.Core.Exceptions;
using Lekker.PackageDeployer.Core.Models.Soap;
using Lekker.PackageDeployer.Core.Services;

namespace Lekker.PackageDeployer.Core.Extensions
{
    public static class HttpExtensions
    {
        public static async Task<HttpWebRequest> MakeSOAPWebRequestAsync(this ISoapAction action)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(action.Uri);
            request.Method = "POST";
            request.Headers.Add("SOAPAction", action.ActionName);
            request.ContentType = "text/xml; charset=utf-8";

            var stream = await request.GetRequestStreamAsync();
            await action.GetRequestPayload(stream);
            await stream.FlushAsync();
            stream.Close();

            return request;
        }

        public static async Task HandleSOAPResponse(this HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK) return;

            XmlDocument doc = new XmlDocument();
            using StreamReader reader = new StreamReader(response.GetResponseStream());
            doc.LoadXml(await reader.ReadToEndAsync());

            string msg = doc.GetElementById("faultstring")?.InnerText;

            throw new DeploymentHttpException(msg, response.StatusCode);
        }
    }
}