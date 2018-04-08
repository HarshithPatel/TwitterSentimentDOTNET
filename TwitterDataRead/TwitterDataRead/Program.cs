using System;
using LinqToTwitter;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Xml.Linq;
using System.Xml;
namespace TwitterDataRead
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Get Tweets and thier score");

            //var tweets = GetTweets();
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = "cRha74sMORRB10UKbF0pTqDyn",
                    ConsumerSecret = "8cLvmwQqATGmkdIZ2j5ipCOv8Gkp8U1o7yjZsRCJdCBAdqu6JP",
                    OAuthToken = "1858043162-Qb41Zc4PRX7vLwMKOh17oahtQAKIlBx5Lnv8P4F",
                    OAuthTokenSecret = "QU30u7DuowkkGC4XnwpQFvWlA0z4lVHwURcIcyEvzW3SZ"
                }
            };

            var twitterCtx = new TwitterContext(auth);

            var tweets =
                (from tweet in twitterCtx.Status  
                 where tweet.Type == StatusType.User  
                 && tweet.ScreenName == "barackobama"  
                 && tweet.Count == 10
                 select tweet).ToList();
            
            double totalscore = 0.0;
            int i = 1;
            //StorePersistentTweets();
            //XmlNodeList tweetsFromXML = readPersistenTweetsDonald();
            //foreach(XmlNode tweet in tweetsFromXML)
            //{
            //    Console.WriteLine("======================");
            //    Console.WriteLine(" ");
            //    Console.WriteLine("Tweet No.: {0}",i);
            //    i += 1;
            //    Console.WriteLine(tweet.InnerText);
            //    Thread.Sleep(2000);
            //    Task<double> score = MakeRequest(tweet.InnerText);
            //    Console.WriteLine("======================");
            //    Console.WriteLine("Tweet Sentiment - Trump: {0}%",score.Result*100);
            //    totalscore += Convert.ToDouble(score.Result);
            //}
            //Console.WriteLine("======================");
            //Console.WriteLine("Overall Tweet Sentiment for Trump: {0}%", (totalscore / tweetsFromXML.Count) * 100);
            //Thread.Sleep(5000);
            // totalscore = 0.0;
            // i = 1;
            //XmlNodeList tweetsFromXML2 = readPersistenTweetsObama();
            //foreach (XmlNode tweet in tweetsFromXML2)
            //{
            //    Console.WriteLine("======================");
            //    Console.WriteLine(" ");
            //    Console.WriteLine("Tweet No.: {0}", i);
            //    i += 1;
            //    Console.WriteLine(tweet.InnerText);
            //    Thread.Sleep(2000);
            //    Task<double> score = MakeRequest(tweet.InnerText);
            //    Console.WriteLine("======================");
            //    Console.WriteLine("Tweet Sentiment - Obama: {0}%", score.Result * 100);
            //    totalscore += Convert.ToDouble(score.Result);
            //}
            //Console.WriteLine("======================");
            //Console.WriteLine("Overall Tweet Sentiment for Obama: {0}%", (totalscore / tweetsFromXML2.Count) * 100);

            foreach(var tweet in tweets)
            {
                Console.WriteLine("======================");
                Console.WriteLine(" ");
                Console.WriteLine("Tweet No.: {0}",i);
                i += 1;
                Console.WriteLine(tweet.Text);
                Thread.Sleep(1000);
                Task<double> score = MakeRequest(tweet.Text);
                Console.WriteLine("Tweet Sentiment Score: {0}%",score.Result*100);
                totalscore += Convert.ToDouble(score.Result);
            }

            //Console.ReadLine();
            double averageScore = totalscore / tweets.Count();
            Console.WriteLine(" ");
            Console.WriteLine("======================");
            Console.WriteLine("Overall Sentiment : {0}%",averageScore*100);
            Console.WriteLine("End of Execution");

            Console.ReadLine();
        }

        static async Task<double> MakeRequest(string tweet)
        {
            try
            {
                var client = new HttpClient();
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                // Request headers
                 template temp = new template
                {
                    id = "0",
                    language = "en",
                    text = tweet
                };
                string postData = JsonConvert.SerializeObject(temp); 

                string requestString = "{\"documents\":[";
                requestString += postData;
                requestString += "]}";



                HttpWebRequest request =  (HttpWebRequest)WebRequest.Create("https://westcentralus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment");
                request.Method = "POST";
                request.ContentType = "application/json";

                byte[] bytes = Encoding.UTF8.GetBytes(requestString);
                request.ContentLength = bytes.Length;
                request.Headers.Add("Ocp-Apim-Subscription-Key", "2f7b39b799a34ebb86b24017b6d8aa4e");

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response1 = await request.GetResponseAsync();
                Stream stream = response1.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string result = reader.ReadToEnd();
                double score = Convert.ToDouble(result.Substring(23,result.IndexOf('i')-25));
               
                stream.Dispose();
                reader.Dispose();
                return score;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0.0;
            }


        }

        static XmlNodeList readPersistenTweetsDonald()
        {
            string path = Directory.GetCurrentDirectory() + "/DonaldTrump.xml";

            //create new instance of XmlDocument
            XmlDocument doc = new XmlDocument();

            //load from file
            doc.Load(path);
            XmlNodeList nodeList = doc.SelectNodes("//Tweet");
            //foreach (XmlNode no in nodeList)
            //{
            //    Console.WriteLine(no.InnerText);
            //    //XmlNode node = doc.SelectSingleNode("//Tweets/Tweet/");
            //    //string str = node.InnerText;

               
            //}
            return nodeList;
        }

        static XmlNodeList readPersistenTweetsObama()
        {
            string path = Directory.GetCurrentDirectory() + "/BarackObama.xml";

            //create new instance of XmlDocument
            XmlDocument doc = new XmlDocument();

            //load from file
            doc.Load(path);
            XmlNodeList nodeList = doc.SelectNodes("//Tweet");
            //foreach (XmlNode no in nodeList)
            //{
            //    Console.WriteLine(no.InnerText);
            //    //XmlNode node = doc.SelectSingleNode("//Tweets/Tweet/");
            //    //string str = node.InnerText;


            //}
            return nodeList;
        }

        static void StorePersistentTweets()
        {
            //var tweets = GetTweets();
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = "cRha74sMORRB10UKbF0pTqDyn",
                    ConsumerSecret = "8cLvmwQqATGmkdIZ2j5ipCOv8Gkp8U1o7yjZsRCJdCBAdqu6JP",
                    OAuthToken = "1858043162-Qb41Zc4PRX7vLwMKOh17oahtQAKIlBx5Lnv8P4F",
                    OAuthTokenSecret = "QU30u7DuowkkGC4XnwpQFvWlA0z4lVHwURcIcyEvzW3SZ"
                }
            };

            var twitterCtx = new TwitterContext(auth);
            ulong lastid = 0;



            string path = Directory.GetCurrentDirectory() + "/BarackObama.xml";

            //create new instance of XmlDocument
            XmlDocument doc = new XmlDocument();

            //load from file
            doc.Load(path);

            //create node and add value
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Tweets", null);
            //node.InnerText = "this is new node";
            int i = 1900;


            for (int j = 0; j < 11; j++)
            {
                if (lastid != 0)
                {
                    var tweets =
                        (from tweet in twitterCtx.Status
                         where tweet.Type == StatusType.User
                         && tweet.ScreenName == "barackobama"
                         && tweet.Count == 200
                         && tweet.MaxID == lastid
                         select tweet).ToList();
                    lastid = ulong.Parse(tweets.Last().StatusID.ToString());
                    foreach (var tweet in tweets)
                    {
                        Console.WriteLine(i);
                        i += 1;
                        XmlNode nodeTweet = doc.CreateElement("Tweet");
                        nodeTweet.InnerText = tweet.Text;
                        node.AppendChild(nodeTweet);
                        doc.DocumentElement.AppendChild(node);
                        doc.Save(path);

                    }
                }
                else
                {
                    var tweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.User
                 && tweet.ScreenName == "barackobama"
                 && tweet.Count == 200
                 select tweet).ToList();               
                    lastid = ulong.Parse(tweets.Last().StatusID.ToString());
                    foreach (var tweet in tweets)
                    {
                        Console.WriteLine(i);
                        i += 1;
                        XmlNode nodeTweet = doc.CreateElement("Tweet");
                        nodeTweet.InnerText = tweet.Text;
                        node.AppendChild(nodeTweet);
                        doc.DocumentElement.AppendChild(node);
                        doc.Save(path);

                    }
                }
               
            }
        }
    }
    public class template
    {
        public string id { get; set; }
        public string language { get; set; }
        public string text { get; set; }
    }
}
