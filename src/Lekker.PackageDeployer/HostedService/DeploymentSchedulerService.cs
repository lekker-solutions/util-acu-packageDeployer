using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lekker.PackageDeployer.Core.Exceptions;
using Lekker.PackageDeployer.Core.Models;
using Lekker.PackageDeployer.Core.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lekker.PackageDeployer.Core.HostedService
{
    /// <summary>
    /// The hosted service that runs the scheduled deployments
    /// </summary>
    public class DeploymentSchedulerService : IHostedService, IDisposable
    {
        public DeploymentSchedulerService(ILogger<DeploymentSchedulerService> logger, PackageDeployerWorker worker)
        {
            _logger = logger;
            _worker = worker;
            _timer = new Timer(DeployPackage);
            _queuedDeployments = new ConcurrentDictionary<int, Deployment>();
            _completedDeployments = new ConcurrentDictionary<int, Deployment>();
        }

        public Task<ICollection<Deployment>> GetQueuedDeploymentsAsync() => 
            Task.FromResult(
                _queuedDeployments.Select(i=>i.Value)
                    .OrderByDescending(d=>d.RunAt)
                    .ToArray() as ICollection<Deployment>);

        public Task<ICollection<Deployment>> GetCompletedDeploymentsAsync() => 
            Task.FromResult(
                _completedDeployments.Select(i=>i.Value)
                    .OrderByDescending(d=>d.RunAt)
                    .ToArray() as ICollection<Deployment>);

        public Task<Deployment> GetDeploymentByIdAsync(int id)
        {
            if (_completedDeployments.TryGetValue(id, out Deployment deployment))
            {
                return Task.FromResult(deployment);
            }

            if (_queuedDeployments.TryGetValue(id, out deployment))
            {
                return Task.FromResult(deployment);
            }

            return Task.FromResult<Deployment>(default);
        }

        /// <summary>
        /// Adds a new deployment to the stack and potentially resets the next deployment fields
        /// </summary>
        /// <param name="deployment"></param>
        public Task AddDeploymentAsync(Deployment deployment)
        {
            if (_queuedDeployments.ContainsKey(deployment.Id))
                throw new DeploymentScheduleException("Deployment already exists in queue");

            _queuedDeployments[deployment.Id] = deployment;
            ResetTimer();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Changes the RunAt time for a deployment
        /// </summary>
        /// <param name="deploymentId">The ID of the deployment</param>
        /// <param name="newRunat">The new time to run this</param>
        /// <returns></returns>
        public Task RescheduleDeploymentAsync(int deploymentId, DateTime newRunat)
        {
            if (!_queuedDeployments.TryGetValue(deploymentId, out Deployment deployment)) 
                throw new DeploymentNotFoundException();

            deployment.RunAt = newRunat;
            ResetTimer();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Cancels deployment and removes it from the queue
        /// </summary>
        /// <param name="deploymentId">The ID of the deployment</param>
        /// <returns></returns>
        public Task CancelDeploymentAsync(int deploymentId)
        {
            if (!_queuedDeployments.TryGetValue(deploymentId, out Deployment deployment))
                throw new DeploymentNotFoundException();

            _queuedDeployments[deploymentId] = default;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the next deploymentID
        /// </summary>
        /// <returns></returns>
        public Task<int> GetNextDeploymentIDAsync()
        {
            int id = _nextId;
            Interlocked.Increment(ref _nextId);
            return Task.FromResult(id);
        }

        /// <summary>
        /// Used by the ASP.Net Framework only
        /// </summary>
        [Obsolete]
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Deployment Scheduler Service starting.");


            PauseTimer(); // We do not want the timer to start until there is something to deploy

            return Task.CompletedTask;
        }

        /// <summary>
        /// Used by the ASP.Net Framework only
        /// </summary>
        [Obsolete]
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
            PauseTimer();

            return Task.CompletedTask;
        }

        private void ResetTimer()
        {
            _logger.LogInformation("Deployment Service Timer is being Reset");

            // Get the next Deployment in the list
            //
            var nextDeployment = _queuedDeployments.OrderBy(d=>d.Value?.RunAt ?? DateTime.MaxValue).FirstOrDefault().Value;
            _nextDeployment = nextDeployment;

            if (_nextDeployment is null)
            {
                // Wait for the next reset
                PauseTimer();
                return;
            }
                

            // Set the timer time
            //
            TimeSpan spanToDeployment = nextDeployment.RunAt - DateTime.Now;

            // If the timespan is in the past then deploy immediately
            if (spanToDeployment.Negate() > spanToDeployment) spanToDeployment = TimeSpan.Zero;
            
            _timer.Change(spanToDeployment, Timeout.InfiniteTimeSpan);

            _logger.LogInformation("Next Deployment: ID:{Id} RunAt:{RunAt}", _nextDeployment.Id, _nextDeployment.RunAt);
        }

        // The Deployment task that is scheduled
        //
        private async void DeployPackage(object state)
        {
            var nextDeployment = _nextDeployment;
            try
            {
                var count = Interlocked.Increment(ref _executionCount);

                _logger.LogInformation("Timed Deployment Service is working. Count: {Count}", count);
                await _worker.Deploy(nextDeployment);

                nextDeployment.Status = DeploymentStatus.Deployed;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception when attempting to deploy package");
                nextDeployment.Status = DeploymentStatus.Failed;
            }
            finally
            {
                // Remove deployment from Queued List
                //
                _queuedDeployments.Remove(_nextDeployment.Id, out Deployment removed);

                // Add completed deployment to completed list
                //
                _completedDeployments.GetOrAdd(_nextDeployment.Id, _nextDeployment);

                ResetTimer();
            }
        }

        private void PauseTimer()
        {
            _timer.Change(Timeout.Infinite, 0);
            _logger.LogInformation("Deployment Timer Paused");
        }

        public void Dispose()
        {
            _ = _timer?.DisposeAsync();
        }

        private int _executionCount;
        private int _nextId;
        private Timer _timer;
        private Deployment _nextDeployment;

        private readonly ILogger _logger;
        private readonly PackageDeployerWorker _worker;
        private readonly ConcurrentDictionary<int, Deployment> _queuedDeployments;
        private readonly ConcurrentDictionary<int, Deployment> _completedDeployments;
    }
}