#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/13
// ----------------------------------------------------------------------------------
#endregion

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lekker.PackageDeployer.Models.Request
{
    /// <summary>
    /// A single customization package. Allows for the same package to be deployed to multiple sites without copying the base64 in the request
    /// </summary>
    public class Package
    {
        [JsonPropertyName("packageId")]
        [Required]
        public int PackageId { get; set; }

        [JsonPropertyName("packageName")]
        [Required]
        public string Name { get; set; }

        [JsonPropertyName("base64")]
        [Required]
        public string Base64 { get; set; }
    }
}