using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ApiConnect
    {
        private static string ApiUrl { get; set; }

        public static void SetApiUrl(string apiUrl)
        {
            if (ApiUrl != null && ApiUrl != "") return;
            ApiUrl = apiUrl;
        }

        public static string FromApi<T>(string api, T model) {
            return "";
        }

    }
}
