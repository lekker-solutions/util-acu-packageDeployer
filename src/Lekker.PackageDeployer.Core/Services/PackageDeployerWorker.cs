using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Lekker.PackageDeployer.Core.Extensions;
using Lekker.PackageDeployer.Core.Models;
using Lekker.PackageDeployer.Core.Models.Soap;
using Microsoft.Extensions.Logging;

namespace Lekker.PackageDeployer.Core.Services
{
    /// <summary>
    /// The class that performs the deployment from a list of steps
    /// </summary>
    public class PackageDeployerWorker
    {
        public PackageDeployerWorker(ILogger<PackageDeployerWorker> logger)
        {
            _logger = logger;
            _container = new CookieContainer();
        }

        /// <summary>
        /// Runs the steps specified in the Deployment
        /// </summary>
        /// <param name="deployment"></param>
        public async Task Deploy(Deployment deployment)
        {
            Uri uri = default;
            bool logout = default;
            try
            {
                // Run each action in the deployment
                //
                foreach (Step step in deployment.Steps)
                {
                    _logger.LogInformation($"Performing Action: {step.ActionType}");

                    uri = step.Action.Uri;

                    var request = await step.Action.MakeSOAPWebRequestAsync();
                    request.CookieContainer = _container;

                    var response = await request.GetResponseAsync() as HttpWebResponse;
                    await response.HandleSOAPResponse();

                    // Set if we have a session on a site
                    logout = step.ActionType switch
                    {
                        DeploymentStepActionType.AcumaticaSoapLogin => true,
                        DeploymentStepActionType.AcumaticaSoapLogout => false,
                        _ => logout
                    };

                    _logger.LogInformation($"Action: {step.ActionType} Success");
                }
            }
            catch (WebException e)
            {
                StreamReader reader = new StreamReader(e.Response.GetResponseStream());

                _logger.LogError(e, "Action Failed: " + await reader.ReadToEndAsync());
            }
            finally
            {
                if (logout)
                {
                    _logger.LogInformation($"Logging out of {uri}");

                    ISoapAction logoutAction = new LogoutAction(uri);
                    var logoutReq = await logoutAction.MakeSOAPWebRequestAsync();
                    var logoutResponse = await logoutReq.GetResponseAsync() as HttpWebResponse;
                    await logoutResponse.HandleSOAPResponse();

                    _logger.LogInformation("Logout Successful");
                }
            }
        }

        private readonly ILogger _logger;
        private readonly CookieContainer _container;
    }
}