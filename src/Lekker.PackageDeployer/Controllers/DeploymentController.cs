using System;
using System.Linq;
using System.Threading.Tasks;
using Lekker.PackageDeployer.Adapter;
using Lekker.PackageDeployer.Core.Exceptions;
using Lekker.PackageDeployer.Core.HostedService;
using Lekker.PackageDeployer.Models.Request;
using Lekker.PackageDeployer.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lekker.PackageDeployer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeploymentController : ControllerBase
    {
        private readonly ILogger<DeploymentController> _logger;
        private readonly DeploymentSchedulerService _deploymentService;
        private readonly DeploymentAdapter _deploymentAdapter;

        public DeploymentController(ILogger<DeploymentController> logger, 
            DeploymentSchedulerService deploymentService,
            DeploymentAdapter deploymentAdapter)
        {
            _logger = logger;
            _deploymentService = deploymentService;
            _deploymentAdapter = deploymentAdapter;
        }

        /// <summary>
        /// Add a new Deployment
        /// </summary>
        /// <param name="request">A deployment to add to the queue</param>
        /// <returns></returns>
        [HttpPost]
        [Route("new")]
        public async Task<IActionResult> AddNewDeployment([FromBody] DeploymentRequest request)
        {
            try
            {
                // Get Next ID
                //
                int nextId = await _deploymentService.GetNextDeploymentIDAsync();
                request.Id = nextId;

                // Adapt to Domain (validation occurs here)
                //
                var deployment = await _deploymentAdapter.AdaptRequestDeployment(request, nextId);

                // Set in queue
                //
                await _deploymentService.AddDeploymentAsync(deployment);
            }
            catch (DeploymentException e)
            {
                return BadRequest(new BaseResponse(false, e.Message));
            }

            return Accepted(new BaseResponse(request, true, "Deployment accepted"));
        }

        /// <summary>
        /// Reschedule an Existing Deployment
        /// </summary>
        /// <param name="request">The ID and new time that the deployment should be run</param>
        /// <returns></returns>
        [HttpPost]
        [Route("reschedule")]
        public async Task<IActionResult> RescheduleDeployment([FromBody] RescheduleRequest request)
        {
            try
            {
                // Attempt Reschedule
                //
                await _deploymentService.RescheduleDeploymentAsync(request.Id, request.NewScheduledDateTime);
            }
            catch (DeploymentNotFoundException)
            {
                return NotFound(new BaseResponse(false, "Deployment not found"));
            }
            catch (DeploymentException e)
            {
                return BadRequest(new BaseResponse(false, e.Message));
            }

            return Accepted(new BaseResponse(true, "Deployment rescheduled"));
        }

        /// <summary>
        /// Get a list of all queued deployments ordered by date
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("queued")]
        public async Task<IActionResult> GetQueuedDeployments()
        {
            // Get Deployments queued
            //
            var deployments = await _deploymentService.GetQueuedDeploymentsAsync();

            // Translate Deployments to Http Response Model
            //
            var deploymentRequests = await Task.WhenAll(deployments.Select(d=> _deploymentAdapter.AdaptDeploymentToRequest(d)).ToArray());

            return Ok(new BaseResponse(deploymentRequests, true, ""));
        }

        /// <summary>
        /// Get a list of all completed deployments ordered by date
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("completed")]
        public async Task<IActionResult> GetCompletedDeployments()
        {
            // Get Deployments queued
            //
            var deployments = await _deploymentService.GetCompletedDeploymentsAsync();

            // Translate Deployments to Http Response Model
            //
            var deploymentRequests = await Task.WhenAll(deployments.Select(d=> _deploymentAdapter.AdaptDeploymentToRequest(d)).ToArray());

            return Ok(new BaseResponse(deploymentRequests, true, ""));
        }

        /// <summary>
        /// Get a single deployment by ID
        /// </summary>
        /// <param name="id">The ID of the deployment that was returned in the 'Add Deployment' call</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetDeployment(int id)
        {
            var deployment = await _deploymentService.GetDeploymentByIdAsync(id);

            if (deployment is null)
            {
                return NotFound(new BaseResponse(false, "Deployment not found"));
            }

            var deploymentRequest = await _deploymentAdapter.AdaptDeploymentToRequest(deployment);

            return Ok(new BaseResponse(deploymentRequest, true, ""));
        }

        /// <summary>
        /// Cancels a deployment by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel/{id:int}")]
        public async Task<IActionResult> CancelDeployment(int id)
        {
            try
            {
                await _deploymentService.CancelDeploymentAsync(id);
            }
            catch (DeploymentNotFoundException)
            {
                return NotFound(new BaseResponse(false, "Deployment not found"));
            }

            return Accepted(new BaseResponse(true, "Deployment Cancelled"));
        }
    }
}
