using System.Collections.Generic;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.Configuration;

namespace Mobile.Solution.Infrastructure.Requests.NSI
{
    public class NsiRequest<T> : Request<T>
    {
        public virtual Task<List<T>> InitRequest(string @params)
        {
            return FetchRequest(@params);
        }

        protected override Task<List<T>> FetchRequest(string @params)
        {
            return base.FetchRequest($@"{Config.RestApiAddressString}nsi/{@params}");
        }
    }
}