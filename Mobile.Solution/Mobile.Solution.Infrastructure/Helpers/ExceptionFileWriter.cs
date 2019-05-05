using System;
using System.Threading.Tasks;
using Mobile.Solution.Infrastructure.Dependencies;
using Mobile.Solution.Infrastructure.Requests;
using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.Helpers
{
    public static class ExceptionFileWriter
    {
        public static string FileName { get; } = "Fatal.txt";

        public static async Task ToLogUnhandledException(this Exception exception)
        {
            try
            {
                var errorMessage = string.Format("Time: {0}\r\nError: Unhandled User Exception\r\n{1}\n\n",
                    DateTime.Now,
                    string.IsNullOrEmpty(exception.StackTrace) ? exception.ToString() : exception.StackTrace);
                var appInfo = DependencyService.Get<IAppInfo>();
                try
                {
                    await ServerManager.SendError(appInfo.UniqueId, errorMessage);
                }
                catch
                {
                    var fp = DependencyService.Get<IFilePreview>();
                    fp.SaveStringToFile(errorMessage, FileName);
                }
            }
            catch
            {
            }
        }
    }
}