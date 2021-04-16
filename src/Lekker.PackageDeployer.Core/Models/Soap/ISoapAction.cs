using System;
using System.IO;
using System.Threading.Tasks;

namespace Lekker.PackageDeployer.Core.Models.Soap
{
    public interface ISoapAction
    {
        Task GetRequestPayload(Stream requestStream);
        string ActionName { get; }
        Uri Uri { get; }
    }
}