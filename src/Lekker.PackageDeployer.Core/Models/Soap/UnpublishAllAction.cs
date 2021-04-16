#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2021 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2021/04/13
// ----------------------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.Threading.Tasks;

namespace Lekker.PackageDeployer.Core.Models.Soap
{
    public class UnpublishAllAction : ISoapAction
    {
        public UnpublishAllAction(string url)
        {
            Uri = new Uri(url);
        }

        public async Task GetRequestPayload(Stream requestStream)
        {
            throw new NotImplementedException();
        }

        public string ActionName { get; }
        public Uri Uri { get; }
    }
}