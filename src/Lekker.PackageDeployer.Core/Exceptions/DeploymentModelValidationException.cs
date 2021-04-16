#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/13
// ----------------------------------------------------------------------------------
#endregion

namespace Lekker.PackageDeployer.Core.Exceptions
{
    public class DeploymentModelValidationException : DeploymentException
    {
        public string Field { get; }
        public string FieldValue { get; }

        public DeploymentModelValidationException(string message, string field, string fieldValue) : base(message)
        {
            Field = field;
            FieldValue = fieldValue;
        }


    }
}