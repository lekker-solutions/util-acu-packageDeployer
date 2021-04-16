#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/13
// ----------------------------------------------------------------------------------
#endregion

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Lekker.PackageDeployer.Models.Request
{
    public enum Operation
    {
        Login,
        UploadPackage,
        Publish,
        UnpublishAll,
        Logout
    }
}