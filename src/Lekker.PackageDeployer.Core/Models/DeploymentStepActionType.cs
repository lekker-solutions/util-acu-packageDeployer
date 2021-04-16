#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/12
// ----------------------------------------------------------------------------------
#endregion

namespace Lekker.PackageDeployer.Core.Models
{
    public enum DeploymentStepActionType
    {
        None,
        AcumaticaSoapLogin,
        AcumaticaSoapLogout,
        UploadPackage,
        PublishPackage,
        UnpublishAllPackages
    }
}