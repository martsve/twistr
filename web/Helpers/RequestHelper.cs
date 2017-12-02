using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Twistr_web.Helper
{
    public static class RequestHelper 
    {
        private static string APIUSER_KEY = "key";
        private static string URL_APIUSER_KEY = "key";

        public static string ApiUser(HttpRequest request)
        {
            var key = request.Headers.ContainsKey(APIUSER_KEY) ? (string)request.Headers[APIUSER_KEY] : (string)request.Query[URL_APIUSER_KEY];
            if (key == null)
            {
                Respons("AuthenticationFailed", "No key was found for the request", 403);
            }
            return key;
        }

        public static void Respons(string title, string message, int errorCode)
        {
            throw new Exception($"{errorCode} {title}: {message}");
        }
    }
}