using System;
using System.Threading.Tasks;

namespace Mobile.Solution.Infrastructure.Requests
{
    public static class ServerManager
    {
        public static async Task<Tuple<bool, string>> CheckForUpdates(string version)
        {
            return await Request<object>.CheckForUpdates(version);
        }

        public static async Task<bool> RegisterPhone(string uniqueId, long phoneNumber)
        {
            return await Request<object>.RegisterPhone(uniqueId, phoneNumber);
        }

        public static async Task<bool> CheckCode(string uniqueId, int code)
        {
            return await Request<object>.CheckCode(uniqueId, code);
        }

        public static async Task<bool> TestConnection(string uniqueId)
        {
            return await Request<object>.TestConnection(uniqueId);
        }

        public static async Task SendError(string uniqueId, string errorMessage)
        {
            await Request<object>.SendError(uniqueId, errorMessage);
        }

        public static async Task<Tuple<string, string>> CheckForDictUpdates(string uniqueId, string nsi, string guid)
        {
            return await Request<object>.CheckForDictUpdates(uniqueId, nsi, guid);
        }
    }
}