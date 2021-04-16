#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/12
// ----------------------------------------------------------------------------------
#endregion

using System;

namespace Lekker.PackageDeployer.Core.Exceptions
{
    public class DeploymentException : ApplicationException
    {
        public DeploymentException(string message) : base(message)
        {
            
        }   

        public DeploymentException() : base()
        {
            
        }
    }
}