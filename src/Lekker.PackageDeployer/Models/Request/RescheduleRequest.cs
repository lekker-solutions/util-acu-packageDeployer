using System;
using System.Text.Json.Serialization;

namespace Lekker.PackageDeployer.Models.Request
{
    public class RescheduleRequest
    {
        /// <summary>
        /// The ID of the Generated Deployment Request
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// The new time that you want to run the deployment at
        /// </summary>
        [JsonPropertyName("newRunAt")]
        public DateTime NewScheduledDateTime { get; set; }
    }
}