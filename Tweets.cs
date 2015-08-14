using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Newtonsoft.Json;

namespace TwitterScraper
{
    class Tweet
    {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        public long ID = 0;
        public string Text = "";
        public DateTime Time = new DateTime();
        public int Retweets = 0;
        public int Favourites = 0;
        public string User = "";
        public int Followers = 0;

        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Null Constructor
        /// </summary>
        public Tweet() { }

        /// <summary>
        /// Create a new tweet from an original Tweetinvi object
        /// </summary>
        public Tweet(Tweetinvi.Core.Interfaces.ITweet original)
        {
            this.ID = original.Id;
            this.Text = original.Text.Replace(",", "");
            this.Time = original.CreatedAt;
            this.Retweets = original.RetweetCount;
            this.Favourites = original.FavouriteCount;
            this.User = original.Creator.Name;
            this.Followers = original.Creator.FollowersCount;
        }

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Serialize the tweet to a string:
        /// </summary>
        static public string Serializer(Tweet line)
        {
            return JsonConvert.SerializeObject(line);
        }
    }
}
