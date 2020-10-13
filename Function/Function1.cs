using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Models;

namespace TwitterToCosmos
{
    public static class Function1
    {
        private static TwitterClient client;

        [FunctionName("Function1")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, [CosmosDB(databaseName: "Twitter", collectionName: "Tweets", ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<Tweet> twitterItemsOut, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                client = new TwitterClient("", "", "");
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw;
            }
            
            string userName = "Alice_Weidel";

            var tweets = await GetUserTimelineTweets(userName, log).ConfigureAwait(false);

            foreach (var tweet in tweets)
            {
                Tweet item = new Tweet() 
                { 
                    username = userName,
                    content = tweet.FullText,
                    id = Convert.ToString(tweet.Id),
                    createdAt = tweet.CreatedAt,
                    isRetweet = tweet.IsRetweet
                };

                await twitterItemsOut.AddAsync(item).ConfigureAwait(false);
            }

            log.LogInformation($"Added {tweets.Length} entries to the database");
        }

        private static async Task<ITweet[]> GetUserTimelineTweets(string userName, ILogger log)
        {
            try
            {
                var tweets = new List<ITweet>();
                var receivedTweets = (await client.Timelines.GetUserTimelineAsync(userName).ConfigureAwait(false)).ToArray();
                tweets.AddRange(receivedTweets);

                return tweets.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An unexpected error occured");
                throw;
            }
        }
    }
}
