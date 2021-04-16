using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lekker.PackageDeployer.Core.Models
{
    /// <summary>
    /// Model for a deployment operation, code leaving a machine to an Acumatica instance
    /// </summary>
    [DebuggerDisplay("Id: {Id}")]
    public class Deployment
    {
        /// <summary>
        /// The ID of the deployment, set sequentially from app startup
        /// </summary>
        public int Id { get; }

        public DeploymentStatus Status { get; set; }

        /// <summary>
        /// The date/time in which to run the deployment
        /// </summary>
        public DateTime RunAt { get; set; }

        // ----- Post Deployment -----

        /// <summary>
        /// The exception message if it failed
        /// </summary>
        public Exception DeploymentException { get; set; }

        /// <summary>
        /// The list of Steps in the deployment
        /// </summary>
        public ICollection<Step> Steps => _steps.ToArray();

        public void AddStep(Step step) => _steps.Add(step);

        /// <summary>
        /// Create a deployment with a pre initialized list of steps
        /// </summary>
        /// <param name="steps"></param>
        public Deployment(int id, IEnumerable<Step> steps) : this(id)
        {
            _steps.AddRange(steps);
        }

        public Deployment(int id)
        {
            Id = id;
            _steps = new List<Step>();
        }

        
        private readonly List<Step> _steps;
    }
}