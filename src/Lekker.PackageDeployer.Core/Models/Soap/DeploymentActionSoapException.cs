using System;

namespace Lekker.PackageDeployer.Core.Models.Soap
{
    public class DeploymentActionSoapException : ApplicationException
    {
        public DeploymentActionSoapException(string message) : base(message)
        {
            
        }
    }
}