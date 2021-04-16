#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/12
// ----------------------------------------------------------------------------------
#endregion

namespace Lekker.PackageDeployer.Core.Exceptions
{
    public class DeploymentScheduleException : DeploymentException
    {
        public DeploymentScheduleException(string message) : base(message)
        {
            
        }
    }
}