using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lekker.PackageDeployer.Models.Request
{
    /// <summary>
    /// Class that represents Deserialized Json from the web request
    /// </summary>
    public class DeploymentRequest
    {
        /// <summary>
        /// The ID of the deployment, returned by the server
        /// </summary>
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        /// <summary>
        /// The steps in the deployment: Upload, publish, unpublish
        /// </summary>
        [JsonPropertyName("steps")]
        public List<Step> Steps { get; set; }

        /// <summary>
        /// The websites associated with the steps
        /// </summary>
        [JsonPropertyName("sites")]
        public List<Site> Sites { get; set; }

        /// <summary>
        /// The packages associated with the upload steps
        /// </summary>
        [JsonPropertyName("packages")]
        public List<Package> Packages { get; set; }

        /// <summary>
        /// The Date/Time in which the deployment will be run
        /// </summary>
        [JsonPropertyName("runAt")]
        [Required]
        public DateTime RunDate { get; set; }

    }
}