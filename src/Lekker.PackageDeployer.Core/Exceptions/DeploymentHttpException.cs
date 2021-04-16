#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/12
// ----------------------------------------------------------------------------------
#endregion

using System.Net;

namespace Lekker.PackageDeployer.Core.Exceptions
{
    public class DeploymentHttpException : DeploymentException
    {
        public HttpStatusCode StatusCode { get; set; }

        public DeploymentHttpException(string message, HttpStatusCode code) : base(message)
        {
            
        }
    }
}