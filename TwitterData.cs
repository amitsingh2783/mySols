using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace TwitterScraper
{
    public static class TwitterData
    {
        public static string GetTweets()
        {
            Twitter.SetCredentials(accessToken: ConfigurationManager.AppSettings["accessToken"], accessTokenSectet: ConfigurationManager.AppSettings["accessTokenSectet"],
                                   consumerKey: ConfigurationManager.AppSettings["consumerKey"], consumerSecret: ConfigurationManager.AppSettings["consumerSecret"]);
            
            Console.WriteLine("Twitter Ready!");

            string userName = ConfigurationManager.AppSettings["twitterUserName"];
            var sentiment= new List<string>(ConfigurationManager.AppSettings["sentiment"].Split(new char[] { ';' })).ToArray();
            var objects = new List<string>(ConfigurationManager.AppSettings["objects"].Split(new char[] { ';' })).ToArray();
            var reviewWords = new List<string>(ConfigurationManager.AppSettings["reviewWords"].Split(new char[] { ';' })).ToArray();
            var hashTags = new List<string>(ConfigurationManager.AppSettings["hashTags"].Split(new char[] { ';' })).ToArray();

            var startDate = DateTime.Now;

            var usernames = FileManagement.GetUsernames();
            var nextUpdateTime = FileManagement.GetNextUpdateTime();

            var file = FileManagement.GetFilename(userName);
            int tweetCount = 0;

            var tweets = FileManagement.GetTweets(file);
            var timeSpan = Twitter.GetAverageTimeSpan(tweets);
            
            var tweetList = Twitter.GetTimeline(userName, tweets, ref tweetCount);
            List<Tweet> filteredTweets = new List<Tweet>();
            foreach (var tweet in tweetList)
            {
                bool flagOne = false;
                bool flagTwo = false;
                var splitTweet = tweet.Text.ToLower().Split();

                var sentimentMatch = findMatches(splitTweet, sentiment);
                var hashTagMatch = findMatches(splitTweet, hashTags);
                var objectMatch = findMatches(splitTweet, objects);
                var reviewWordsMatch = findMatches(splitTweet, reviewWords);
                
                if (reviewWordsMatch && hashTagMatch || objectMatch && sentimentMatch)
                {
                    filteredTweets.Add(tweet);
                }
            }
            var twitterJson = JsonConvert.SerializeObject(filteredTweets);
            

            return twitterJson;
        }
        private static bool findMatches(string[] arr1, string[] arr2)
        {
            foreach(var item in arr1){
                foreach (var word in arr2)
                    if (item.Trim() == word.Trim())
                        return true;
            }
            return false;
        }
    }
}
