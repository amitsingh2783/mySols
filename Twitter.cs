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
using Tweetinvi;
using Tweetinvi.Json;
using Newtonsoft.Json;
using Stream = Tweetinvi.Stream;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Events;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.oAuth;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Core.Interfaces.DTO;
using Tweetinvi.Core.Interfaces.Models;
using Tweetinvi.Core.Exceptions;

namespace TwitterScraper
{
    static class Twitter
    {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        private static string _accessToken = "";
        private static string _accessTokenSecret = "";
        private static string _consumerKey = "";
        private static string _consumerSecret = "";
        private static List<DateTime> timeStamps = new List<DateTime>();

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/

        /// <summary>
        /// Set Twitter Dev Credentials
        /// </summary>
        public static void SetCredentials(string accessToken, string accessTokenSectet, string consumerKey, string consumerSecret)
        {
            _accessToken = accessToken;
            _accessTokenSecret = accessTokenSectet;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            TwitterCredentials.SetCredentials(accessToken, accessTokenSectet, consumerKey, consumerSecret);
        }

        /// <summary>
        /// Download user's timeline, and receive a list with Tweets ordered from oldest to newest
        /// </summary>
        public static List<Tweet> GetTimeline(string userName, List<Tweet> getTweets, ref int tweetCount)
        {
            List<Tweet> tweets;
            if (getTweets.Count == 0)
            {
               Console.WriteLine(" First time downloading " + userName + ", creating new file.");
               tweets = TweetsDownload(true, userName, getTweets, ref tweetCount);
            }
            else
            {
                tweets = TweetsDownload(false, userName, getTweets, ref tweetCount);
            }
            //Sort the tweets from oldest to newest
            var sortedTweets = (from tweet in tweets
                               orderby tweet.Time ascending
                               select tweet).ToList();
            return sortedTweets;
        }

        /// <summary>
        /// Download Historical Tweets
        /// </summary>
        private static List<Tweet> TweetsDownload(bool historical, string ticker, List<Tweet> getTweets, ref int tweetCount)
        {
            var tweets = default(IEnumerable<ITweet>);
            var downloadedTweets = new List<Tweet>();
            long maxID = 0;
            long sinceID = 0; 
            
            //Get the last tweet downloaded:
            if (!historical) sinceID = LastSavedTweetID(getTweets);

            do
            {
                WaitForAPIReady();
                var request = Timeline.CreateUserTimelineRequestParameter(ticker);
                request.MaximumNumberOfTweetsToRetrieve = 32000;

                if (historical) {
                    request.MaxId = maxID - 1;
                } else {
                    request.SinceId = sinceID + 1;
                }
                
                try
                {
                    AddTimeStamps(timeStamps);
                    tweets = Timeline.GetUserTimeline(request);
                    if (tweets != null && tweets.Count() > 0)
                    {
                        tweetCount += tweets.Count();
                    }
                }
                catch
                {
                    Console.WriteLine(" Couldn't download tweets, trying again...");
                    Thread.Sleep(4000);
                    continue;
                }
                if (tweets != null && tweets.Count() > 0)
                {
                    foreach (var tweet in tweets)
                    {
                        var qcTweet = new Tweet(tweet);
                        downloadedTweets.Add(qcTweet);
                    }
                    if (historical) {
                        maxID = (from tweet in tweets
                                 select tweet.Id).Min();
                    } else {
                        sinceID = (from tweet in tweets
                                   select tweet.Id).Max();
                    }
                }
            }
            while (tweets != null && tweets.Count() > 0);
            return downloadedTweets;
        }

        /// <summary>
        /// Get the ID of the latest saved tweet
        /// </summary>
        public static long LastSavedTweetID(List<Tweet> getTweets)
        {
            var lastLine = getTweets.First();
            long lastTweetID = lastLine.ID;
            return lastTweetID;
        }

        /// <summary>
        /// User's average posting frecuency
        /// </summary>
        public static TimeSpan GetAverageTimeSpan(List<Tweet> tweets)
        {
            if (tweets.Count == 0)
            {
                return TimeSpan.FromSeconds(500);
            }
            else
            {
                List<DateTime> dates = new List<DateTime>();
                foreach (var line in tweets)
                {
                    dates.Add(line.Time);
                }
                var difference = dates.Max().Subtract(dates.Min());
                var averageTimes = TimeSpan.FromMilliseconds(difference.TotalMilliseconds / (dates.Count()));
                return averageTimes;
            }
        }

        /// <summary>
        /// Add Time Stamps to list
        /// </summary>
        private static void AddTimeStamps(List<DateTime> timeStamps)
        {
            if (timeStamps.Count < 300)
            {
                timeStamps.Add(DateTime.Now);
            }
            else
            {
                timeStamps.RemoveAt(0);
                timeStamps.Add(DateTime.Now);
            }
        }

        /// <summary>
        /// Check if API is ready for new request
        /// </summary>
        private static int WaitForAPIReady()
        {
            int count = 0;
            do
            {
                DateTime currentTime = DateTime.Now;
                currentTime = currentTime.AddMinutes(-15);

                count = (from time in timeStamps
                         where time > currentTime
                         select time).Count();
                if (count > 290)
                {
                    Console.WriteLine(" Twitter downloading limit reached. Waiting...");
                    Thread.Sleep(50000);
                }
            } while (count > 290);
            return (300 - count);
        }

        /// <summary>
        /// Statistics of current download and API status
        /// </summary>
        public static void Statistics(int tweetsDownloaded, string userName)
        {
            int requestsLeft = WaitForAPIReady();
            Console.WriteLine(String.Format(" {2} |  Requests left: {0}  |  Tweets Downloaded: {1}\n", requestsLeft, tweetsDownloaded, userName));
        }     
    }
}
