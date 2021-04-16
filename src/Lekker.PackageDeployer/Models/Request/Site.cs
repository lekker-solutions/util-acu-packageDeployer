#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/13
// ----------------------------------------------------------------------------------
#endregion

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Lekker.PackageDeployer.Models.Request
{
    [DebuggerDisplay("SiteId: {SiteId} Url:{Url}")]
    public class Site
    {
        [JsonPropertyName("siteId")]
        public int SiteId { get; set; }

        /// <summary>
        /// The base instance URL of the Acumatica instance to deploy to
        /// </summary>
        ///         [JsonPropertyName("url")]
        public string Url
        {
            get => _acuUrl;
            set
            {
                if (value.Contains("api/servicegate.asmx")) _acuUrl = value;
                else _acuUrl = value.TrimEnd('/') + "/api/servicegate.asmx";
            }
        }



        [JsonPropertyName("username")]
        [Required]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }

        [JsonPropertyName("tenant")]
        public string Tenant { get; set; }

        private string _acuUrl;
    }
}