using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Project_Methane
{
    class HarmonicKernel
    {
        public class Core
        {


            public static ObservableCollection<HarmonicKernel.Core.ArticleItem> articles;
            public static ObservableCollection<HarmonicKernel.Core.PodcastItem> podcasts;
            public static ObservableCollection<HarmonicKernel.Core.VideoItem> videos;
            static YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "702277838158-lgoieg35bdrtg08al8puj296l5i2qeuf.apps.googleusercontent.com",
                ApplicationName = "WinBeta"
            });
            public static int currentAticleId = 0;

            public class ArticleItem
            {

                public string title { get; set; }
                public string permalink { get; set; }
                public long id { get; set; }
                public string content { get; set; }
                public string excerpt { get; set; }
                public string date { get; set; }

                public string thumbnail { get; set; }
                public bool isFormatted = false;
            }
            public class PodcastItem
            {
                public string title { get; set; }
                public string created_at { get; set; }

                public string stream_url { get; set; }
                public int duration { get; set; }
                public string video_url { get; set; }
                public int podcast_number { get; set; }

            }
            public class VideoItem
            {
                public string Title { get; set; }
                public string VideoId { get; set; }
                public string Thumbnail { get; set; }
                public string Date { get; set; }
                public string ViewCount { get; set; }
            }

            public static async Task<bool> GetArticleFeed()
            {
                // Create a client to retrieve the feed from OnMSFT,
                // we also tell the client to use automatic decompression
                // to speed up the process.
                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }))
                {
                    using (var response = await client.GetAsync(new Uri("https://www.onmsft.com/feed/json")))
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        
                        responseString = responseString.Replace("&#8217;", "'"); responseString = responseString.Replace("&#8216;", "'"); responseString = responseString.Replace("&amp;", "&");
                        responseString = responseString.Replace("&#8220;", "'"); responseString = responseString.Replace("&#8221;", "'");

                        articles = JsonConvert.DeserializeObject<ObservableCollection<ArticleItem>>(responseString);

                        foreach (var article in articles)
                        {
                            // Image logic. Images returned from the feed are in a 
                            // 'URL size,' pattern. For now we can just use the first
                            // image. First we split the group of images (,).
                            var images = article.thumbnail.Split(',');
                            // Grab a single image, this code needs to have error checking
                            // You can also mess around with images[] to get better image
                            // quality.
                            var singleImage = images[0].Trim().Split(' ')[0].Trim();
                            // Set the thumbnail image to this new image
                            article.thumbnail = singleImage;

                            // Clean the excerpt string
                            article.excerpt = article.excerpt.Replace("&#46;", "");

                            // Clean and sort the date object. 
                            var articleDateTime = DateTime.Parse(article.date);
                            var localDateTime = articleDateTime.ToLocalTime();

                            // Create a clean time string with correct AM / PM (12 hour time)
                            var localTime = string.Format("{0:t}", localDateTime);

                            // How many days ago was this post
                            var daysAgo = DateTime.Now.Day - localDateTime.Day;

                            switch (daysAgo)
                            {
                                case 0:
                                    article.date = "Today at " + localTime;
                                    break;
                                case 1:
                                    article.date = "Yesterday at " + localTime;
                                    break;
                                default:
                                    article.date = daysAgo + " days ago at " + localTime;
                                    break;
                            }
                        }
                    }
                }

                return true;

            }
            public static async Task<bool> RetrivePodcast()
            {

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri("https://api.soundcloud.com/users/137345051/tracks?client_id=c6838ff2dfb87782907b83c01067eac8"));


                string responseString = await response.Content.ReadAsStringAsync();


                podcasts = JsonConvert.DeserializeObject<ObservableCollection<PodcastItem>>(responseString);
                int podcastNumber = podcasts.Count;
                foreach (var podcast in podcasts)
                {
                    podcast.podcast_number = podcastNumber;
                    podcastNumber -= 1;
                }
                return true;

            }
            public static async Task<bool> RetriveVideos()
            {
                // Show Progress Ring

                // Reset DataContext and Lists


                // Send a request to the YouTube API to search for channels
                var searchChannelRequest = youtubeService.Channels.List("contentDetails");
                searchChannelRequest.Id = "UC70UzaroFf5GcyecHOGw-tw"; // The WinBeta Channel ID
                searchChannelRequest.MaxResults = 1; // Only get one result (Only can be one channel with the same channel ID)

                // API response
                var searchChannelResponse = await searchChannelRequest.ExecuteAsync();
                var channel = searchChannelResponse.Items.First(); // Choose the first (and only) item

                // Send a request to the YouTube API to search for playlists on youtube channel 
                // (for now only grab upload playlist, later on will grab all playlists and let user filter)
                var playlistRequest = youtubeService.PlaylistItems.List("snippet");
                playlistRequest.PlaylistId = channel.ContentDetails.RelatedPlaylists.Uploads; // Get the uploads playlist
                playlistRequest.MaxResults = 50; // Max of 50 results

                // API response
                var playlistResponse = await playlistRequest.ExecuteAsync();

                // Loop through all items in upload playlist
                foreach (var playlistItem in playlistResponse.Items)
                {
                    // Create a video object to pass into the data context
                    VideoItem video = new VideoItem()
                    {
                        Title = playlistItem.Snippet.Title, // Video Title
                        Thumbnail = ExtraFunctions.GetVideoThumbnail(playlistItem), // Video thumbnail <- This function gets the highest quality avaliable
                        Date = ExtraFunctions.ConvertVideoDateTime(playlistItem.Snippet.PublishedAt) // Get the published date (formated and converted to correct time zone)
                    };

                    // Add video to data context list
                    videos.Add(video);
                }

                return true;
            }

            public static string LoadArticle(int id, double webViewWidth, double webViewHeight)
            {

                string unformattedArticle = articles[id].content;
                StringBuilder formattedArticle = new StringBuilder();
                for (int i = 0; i < unformattedArticle.Length;)
                {
                    string upcomingSubsting = "";
                    try
                    {
                        upcomingSubsting = unformattedArticle.Substring(i, 7);
                    }
                    catch { }
                    if (upcomingSubsting == "width=" + '"')
                    {
                        i = ExtraFunctions.AlterDimensions(i, unformattedArticle, formattedArticle, webViewWidth, webViewHeight);
                    }
                    else if (upcomingSubsting == "[appbox")
                    {
                        i += 21;
                        string storeUrl = "https://www.microsoft.com/en-us/store/apps/app/";
                        StringBuilder appId = new StringBuilder();

                        while (unformattedArticle[i] != ']')
                        {
                            appId.Append(unformattedArticle[i]);
                            i += 1;
                        }

                        i += 1;
                        storeUrl = storeUrl + appId.ToString();
                        formattedArticle.Append("<a href=" + '"' + storeUrl + '"' + ">Download app</a>");
                    }
                    else if (upcomingSubsting == "[captio")
                    {
                        int j = i;
                        StringBuilder s = new StringBuilder();
                        while (unformattedArticle[j] != ']')
                        {
                            j += 1;
                        }
                        j += 1;
                        i = j;
                        formattedArticle.Append("<Center>");

                        while (unformattedArticle.Substring(j, 2) != "/>")
                        {
                            if (unformattedArticle.Substring(j, 7) == "width=" + '"')
                            {
                                j = ExtraFunctions.AlterDimensions(j, unformattedArticle, formattedArticle, webViewWidth, webViewHeight);
                            }
                            formattedArticle.Append(unformattedArticle[j]);

                            j += 1;
                        }
                        formattedArticle.Append("/></a>");

                        j += 2;
                        //
                        // formattedArticle.Append("<br><br>Image Description : ");
                        while (unformattedArticle[j] != '[')
                        {
                            //  formattedArticle.Append(unformattedArticle[j]);
                            j += 1;
                        }
                        i = j + 10;
                        formattedArticle.Append("</Center>");


                    }
                    else if (upcomingSubsting == "https:/")
                    {
                        StringBuilder url = new StringBuilder();
                        while (unformattedArticle[i] != '\r')
                        {
                            url.Append(unformattedArticle[i]);
                            i += 1;
                        }
                        i += 4;
                        if (url.ToString().Contains("youtube"))
                        {
                            url.Replace("watch?v=", "embed/");
                            formattedArticle.Append("<iframe width=" + '"' + webViewWidth + '"' + " height=" + '"' + "300" + '"' + " src=" + '"' + url + '"' + "></iframe>");

                        }
                    }
                    else
                    {
                        formattedArticle.Append(unformattedArticle[i]);
                        i += 1;
                    }

                }


                formattedArticle.Replace("\n", "</P><P>");
                string style = "<style> body{zoom:100%;} iframe{margin-bottom:10px} a{text-decoration:none} p { margin-right: 15px; margin-left: 15px} body {  font-family: " + '"' + "Segoe UI" + '"' + "; font-size: 46px; margin-right: 15px; margin-left: 15px } li{ font-family: " + '"' + "Segoe UI" + '"' + "; font-size: 46px; margin-right: 20px; margin-left: 20px } img{align: middle} blockquote{ font-family: " + '"' + "Segoe UI" + '"' + "; font-size: 46px; margin-right: 20px; margin-left: 20px; font-style: italic} </style>";
                return "<html><head>" + style + "</head><body  link = " + '"' + Settings.accentColor + '"' + ">" + "<P>" + formattedArticle.ToString() + "</Font></body>";
            }


        }

        public class ExtraFunctions
        {


            public static String HexConverter(Color c)
            {
                return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            }

            public static string GetVideoThumbnail(PlaylistItem playlistItem)
            {
                // Check if max res image is avaliable (not all videos have them)
                if (playlistItem.Snippet.Thumbnails.Maxres != null)
                    return playlistItem.Snippet.Thumbnails.Maxres.Url; // Max Res
                else // If not get next best thing
                    return playlistItem.Snippet.Thumbnails.Medium.Url; // Medium Res
            }
            public static string ConvertVideoDateTime(DateTime? dt)
            {
                // Check if DateTime is not Null
                if (dt != null)
                {
                    // Convert to local timezone
                    DateTime date = dt.Value.ToLocalTime();

                    // Format the Date section
                    var startDate = date.DayOfWeek + " " + date.Day + " " + GetMonthFromString(date.Month.ToString());

                    // Total Minutes
                    var minutes = date.TimeOfDay.Minutes.ToString();

                    // Add the zero infront of the minutes
                    if (minutes.Length == 1) { minutes = "0" + minutes; }

                    // Basic time
                    var time = date.TimeOfDay.Hours + ":" + minutes + " AM";

                    // Convert to 12 hour time
                    if (date.TimeOfDay.Hours >= 13)
                    {
                        time = (date.TimeOfDay.Hours - 12) + ":" + minutes + " PM";
                    }

                    // Return nice formated date
                    return "Uploaded: " + startDate + " at " + time;
                }
                else
                {
                    return "No Upload Date";
                }
            }
            public static string GetMonthFromString(string month)
            {
                switch (month)
                {
                    case "1":
                        month = "Janurary";
                        break;
                    case "2":
                        month = "Feburary";
                        break;
                    case "3":
                        month = "March";
                        break;
                    case "4":
                        month = "April";
                        break;
                    case "5":
                        month = "May";
                        break;
                    case "6":
                        month = "June";
                        break;
                    case "7":
                        month = "July";
                        break;
                    case "8":
                        month = "August";
                        break;
                    case "9":
                        month = "September";
                        break;
                    case "10":
                        month = "October";
                        break;
                    case "11":
                        month = "November";
                        break;
                    case "12":
                        month = "December";
                        break;
                }
                return month;
            }

            public static int AlterDimensions(int i, string unformattedArticle, StringBuilder formattedArticle, double webViewWidth, double webViewHeight)
            {
                webViewWidth = 1000;
                int j = i + 7;
                string widthStr = ""; int width;
                while (unformattedArticle[j] != '"')
                {
                    widthStr += unformattedArticle[j];
                    j += 1;
                }
                width = Convert.ToInt16(widthStr);
                if (width < webViewWidth)
                {
                    formattedArticle.Append(unformattedArticle[i]);
                    i += 1;

                }
                else
                {
                    formattedArticle.Append("width=" + '"' + (webViewWidth - 30).ToString() + "px" + '"');
                    if (unformattedArticle[j + 2] == 'h')
                    {
                        string heightStr = ""; int height;
                        i = j + 10;
                        while (unformattedArticle[i] != '"')
                        {
                            heightStr += unformattedArticle[i];
                            i += 1;
                        }
                        height = Convert.ToInt16(heightStr);
                        height = Convert.ToInt16(height * (webViewWidth / width));
                        formattedArticle.Append("height=" + '"' + (height).ToString() + '"' + " align =" + '"' + "middle" + '"');
                        i = j + 1;
                    }
                    else
                    {
                        i = j + 1;
                    }
                }
                return i;
            } //.Alters image dimensions


        }

        public static class Settings
        {

            public static string accentColor;

        }
    }

}
