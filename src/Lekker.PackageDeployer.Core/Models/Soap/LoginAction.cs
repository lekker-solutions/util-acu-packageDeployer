using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lekker.PackageDeployer.Core.Models.Soap
{
    public class LoginAction : ISoapAction
    {
        public Uri Uri { get; }

        public LoginAction(string url, string name, string password, string tenant)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DeploymentActionSoapException("No Username Specified");
            if (string.IsNullOrWhiteSpace(password)) throw new DeploymentActionSoapException("No Password Specified");
            Uri = new Uri(url);

            _username = name;
            _password = password;
            _tenant = tenant;
        }

        public async Task GetRequestPayload(Stream requestStream)
        {
            string payload = 
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                "<soap:Body>" +
                "<Login xmlns=\"http://acumatica.com\">" +
                $"<name>{_combineUsername}</name>" +
                $"<password>{_password}</password>" +
                "</Login>" +
                "</soap:Body>" +
                "</soap:Envelope>";

            await requestStream.WriteAsync(Encoding.UTF8.GetBytes(payload));
        }

        public string ActionName =>  "\"http://acumatica.com/Login\"";


        private readonly string _username;
        private readonly string _password;
        private readonly string _tenant;


        private string _combineUsername => string.IsNullOrWhiteSpace(_tenant) ? _username : $"{_username}@{_tenant}";
    }
}