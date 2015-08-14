using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Configuration;
using Facebook;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;


namespace TwitterScraper
{
    static class FacebookData
    {
        public static string GetFacebookInfo()
        { 
            
            var client = new FacebookClient(ConfigurationManager.AppSettings["accessTokedFB"]);
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            
            var retVal = client.Get("me");
            string result  = retVal.ToString();
            return result;
        }
    }
}
