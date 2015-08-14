using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;
using System.Web.Script.Serialization;  

namespace TwitterScraper
{
    static class GooglePlusData
    {
        public static string GetGooglePData()
        {
            string _profileId = "115200425973230417813";
            string _apiKey = ConfigurationManager.AppSettings["GooglePlusAPIKey"];
            string requestString = "https://www.googleapis.com/plus/v1/people/" + _profileId + "?key=" + _apiKey;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = GetWebData(requestString);
            return result;
        }


        private static string GetWebData(string requestString)
        {
            WebRequest objWebRequest = WebRequest.Create(requestString);
            WebResponse objWebResponse = objWebRequest.GetResponse();

            Stream objWebStream = objWebResponse.GetResponseStream();

            using (StreamReader objStreamReader = new StreamReader(objWebStream))
            {
                string result = objStreamReader.ReadToEnd();
                return result;
            }
        }

    }
}
