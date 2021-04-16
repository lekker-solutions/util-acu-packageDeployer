using Lekker.PackageDeployer.Core.Models.Soap;

namespace Lekker.PackageDeployer.Core.Models
{
    /// <summary>
    /// A step in a deployment
    /// </summary>
    public class Step
    {
        public Step(ISoapAction action)
        {
            Action = action;

            ActionType = action switch
            {
                LoginAction => DeploymentStepActionType.AcumaticaSoapLogin,
                LogoutAction => DeploymentStepActionType.AcumaticaSoapLogout,
                PublishAction => DeploymentStepActionType.PublishPackage,
                UploadPackageAction => DeploymentStepActionType.UploadPackage,
                _ => DeploymentStepActionType.None
            };
        }

        public DeploymentStepActionType ActionType { get; }

        public ISoapAction Action { get; }
    }
}