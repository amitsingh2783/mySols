/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals, V0.1
* Created by Raul Pefaur
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace TwitterScraper
{
    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// A Historical Data Feed comprised of the assets requested.
    /// </summary>
    static class FileManagement
    {
        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Fielname of the data.
        /// </summary>
        public static string GetFilename(string userName) {
             return @"../../data/scraped/" + userName + ".txt";
        }

        /// <summary>
        /// CSV Twitter Users File Reader
        /// </summary>
        public static List<string> GetUsernames()
        {
            StreamReader reader = new StreamReader(@"../../data/twitterUsernames.csv");
            List<string> userNames = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                userNames.Add(line);
            }
            return userNames;
        }

        /// <summary>
        /// Users-Dates Dictionary Builder
        /// </summary>
        public static Dictionary<string, DateTime> GetNextUpdateTime()
        {
            Dictionary<string, DateTime> userUpdateTime = new Dictionary<string, DateTime>();
            List<string> userNames = GetUsernames();
            foreach (var user in userNames)
            {
                userUpdateTime.Add(user, DateTime.Now);
            }
            return userUpdateTime;
        }

        /// <summary>
        /// Write the tweet list to a file.
        /// </summary>
        public static void Writer(List<string> TweetList, string file)
        {
            var writer = new StreamWriter(file, true);
            if (TweetList != null && TweetList.Count() > 0)
            {
                foreach (var tweet in TweetList)
                {
                    writer.WriteLine(tweet);
                }
            }
            writer.Close();
        }

        /// <summary>
        /// Read the latest 200 tweets on file
        /// </summary>
        public static List<Tweet> GetTweets(string file)
        {
            List<Tweet> tweetList = new List<Tweet>();
            try
            {
                var latestLines = File.ReadLines(file).Reverse().Take(200).ToList();
                foreach (var line in latestLines)
                {
                    Tweet decodedTweet = JsonConvert.DeserializeObject<Tweet>(line);
                    tweetList.Add(decodedTweet);
                }
            }
            catch
            {
            
            }
            return tweetList;
        }

        /// <summary>
        /// Log Activity
        /// </summary>
        public static DateTime Logger(List<string> tweetList, string userName)
        {
            DateTime date = DateTime.Now;
            if (tweetList != null && tweetList.Count() > 0)
            {
                var logWriter = new StreamWriter(@"../../data/updatesLogs.txt", true);
                logWriter.WriteLine(date + ": Updated Tweets for " + userName);
                logWriter.Close();
            }
            return date;
        }

        public static List<string> Serializer(List<Tweet> tweetList)
        {
            var encodedList = new List<string>();
            foreach (var line in tweetList)
            {
                var encodedTweet = Tweet.Serializer(line);
                encodedList.Add(encodedTweet);
            }
            return encodedList;
        }
    }
}
