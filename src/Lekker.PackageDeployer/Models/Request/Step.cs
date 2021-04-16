using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lekker.PackageDeployer.Models.Request
{
    public class Step
    {
        /// <summary>
        /// The Operation to be performed
        /// </summary>
        [JsonPropertyName("operation")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [EnumDataType(typeof(Operation))]
        [Required]
        public Operation Operation { get; set; }

        /// <summary>
        /// The order of operation within a site
        /// </summary>
        [JsonPropertyName("order")]
        [Required]
        public int Order { get; set; }

        // ------- Site Login Step Properties -------

        /// <summary>
        /// The site in which a cluster of steps is associated with
        /// </summary>
        [JsonPropertyName("siteId")]
        [Required]
        public int SiteId { get; set; }


        // ------- Publish Package Step Properties -------

        /// <summary>
        /// The names of the Packages to be published
        /// </summary>
        [JsonPropertyName("publishPackageNames")]
        public string[] PublishPackageNames { get; set; }

        /// <summary>
        /// Whether to publish the specified packages along with existing packages on the system
        /// </summary>
        [JsonPropertyName("mergePublish")]
        public bool MergePublishWithExistingPackages { get; set; }

        // ------- Upload Package Steps Properties -------

        /// <summary>
        /// The id of the package to be uploaded
        /// </summary>
        [JsonPropertyName("packageId")]
        public int PackageId { get; set; }

        /// <summary>
        /// Whether to overwrite an existing customization project with the same name as the specified name in 'PackageName'
        /// </summary>
        [JsonPropertyName("overwriteIfExists")]
        public bool OverwriteExistingPackage { get; set; }
    }
}