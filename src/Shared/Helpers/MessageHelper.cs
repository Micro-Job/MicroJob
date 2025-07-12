using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Helpers
{
    public static class MessageHelper
    {
        private static readonly ResourceManager resourceManager =
            new ResourceManager("SharedLibrary.Resources.Messages", typeof(MessageHelper).Assembly);

        public static string GetMessage(string errorKey)
        {
            return resourceManager.GetString(errorKey, CultureInfo.CurrentUICulture) ?? "error";
        }

        public static string GetMessage(string errorKey, params object[] args)
        {
            string formatString = resourceManager.GetString(errorKey, CultureInfo.CurrentUICulture) ?? "error";

            return string.Format(formatString, args);
        }
    }
}
