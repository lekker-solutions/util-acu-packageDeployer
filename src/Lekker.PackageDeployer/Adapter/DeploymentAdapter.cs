#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/12
// ----------------------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lekker.PackageDeployer.Controllers;
using Lekker.PackageDeployer.Core.Exceptions;
using Lekker.PackageDeployer.Core.HostedService;
using Lekker.PackageDeployer.Core.Models;
using Lekker.PackageDeployer.Core.Models.Soap;
using Lekker.PackageDeployer.Models.Request;
using DomainStep = Lekker.PackageDeployer.Core.Models.Step;
using RequestStep = Lekker.PackageDeployer.Models.Request.Step;

namespace Lekker.PackageDeployer.Adapter
{
    /// <summary>
    /// Class that adapts the Deployment Request model to the Deployment Domain Model. Also provides Request model validation
    /// </summary>
    public class DeploymentAdapter
    {
        /// <summary>
        /// Adapt a request to the domain model
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Deployment> AdaptRequestDeployment(DeploymentRequest request, int id)
        {
            Deployment deployment = new(id)
            {
                RunAt = request.RunDate,
                Status = DeploymentStatus.Queued
            };

            foreach (RequestStep step in request.Steps.OrderBy(r=>r.Order))
            {
                Site site = request.Sites.FirstOrDefault(s => s.SiteId == step.SiteId);
                if (site is null)
                    throw new DeploymentModelValidationException(
                        $"Step {step.Order} references a site that was not provided",
                        nameof(step.SiteId),
                        step.SiteId.ToString());

                DomainStep domainStep;
                switch (step.Operation)
                {
                    case Operation.Login:

                        domainStep =
                            new DomainStep(new LoginAction(site.Url, site.Username, site.Password, site.Tenant));
                        break;

                    case Operation.UploadPackage:
                        Package package = request.Packages.FirstOrDefault(p => p.PackageId == step.PackageId);
                        if (package is null)
                            throw new DeploymentModelValidationException(
                                $"Step {step.Order} references a package that was not provided",
                                nameof(step.PackageId),
                                step.PackageId.ToString());

                        domainStep = new DomainStep(
                            new UploadPackageAction(
                                site.Url,
                            package.Name, 
                            new MemoryStream(Encoding.UTF8.GetBytes(package.Base64)),
                            step.OverwriteExistingPackage));
                        break;

                    case Operation.Publish:
                        domainStep = new DomainStep(
                            new PublishAction(
                                site.Url,
                                step.PublishPackageNames, 
                                step.MergePublishWithExistingPackages));
                        break;

                    case Operation.UnpublishAll:
                        domainStep = new DomainStep(new UnpublishAllAction(site.Url));
                        break;

                    case Operation.Logout:
                        domainStep = new DomainStep(new LogoutAction(site.Url));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(step.Operation));
                }

                deployment.AddStep(domainStep);
            }

            return Task.FromResult(deployment);
        }

        /// <summary>
        /// Adapts a domain model back to be returned from a controller
        /// </summary>
        /// <param name="deployment"></param>
        /// <returns></returns>
        public Task<DeploymentRequest> AdaptDeploymentToRequest(Deployment deployment)
        {
            var request = new DeploymentRequest();


            return Task.FromResult(request);
        }
    }
}