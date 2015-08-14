using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace TwitterScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Application Starting!");
            var googlePlusData = GooglePlusData.GetGooglePData();
            var fbData = FacebookData.GetFacebookInfo();
            var twitterData = TwitterData.GetTweets();
            JavaScriptSerializer jss = new JavaScriptSerializer();

            List<string> finalOut = new List<string>();
            finalOut.Add(googlePlusData);
            finalOut.Add(fbData);
            finalOut.Add(twitterData);
            string output = jss.Serialize(finalOut);
        }
    }
}
