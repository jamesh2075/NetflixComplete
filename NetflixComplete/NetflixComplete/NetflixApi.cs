using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using HtmlAgilityPack;
using System.Xml.Linq;

namespace NetflixComplete
{
    [JsonObject]
    public class Video : Java.Lang.Object
    {
        [JsonProperty]
        public int Id { get; set; }
        [JsonProperty]
        public string Type { get; set; }
        [JsonProperty]
        public string Title { get; set; }
        [JsonProperty]
        public string Synopsis { get; set; }
        [JsonProperty]
        public string Rating { get; set; }
        [JsonProperty]
        public short ReleaseYear { get; set; }
        [JsonProperty]
        public string CardUrl { get; set; }
        [JsonProperty]
        public List<string> BackgroundImageUrls { get; } = new List<string>();
        [JsonProperty]
        public string VideoUrl { get; set; }
        [JsonProperty]
        public List<string> Starring { get; } = new List<string>();
        [JsonProperty]
        public List<string> Genres { get; } = new List<string>();
        [JsonProperty]
        public bool IsInMyList { get; set; }
    }
    public static class NetflixApi
    {
        public async static System.Threading.Tasks.Task<bool> AuthenticateAsync(string userName, string password)
        {
            var task = System.Threading.Tasks.Task.Factory.StartNew<bool>(() =>
            {
                return Authenticate(userName, password);
            });
            return await task;
        }
        public static bool Authenticate(string userName, string password)
        {
            bool isAuthenticated = false;

            var time = GetTime(DateTime.Now);
            System.Threading.Thread.Sleep(100);
            var sessionId = GetId();
            System.Threading.Thread.Sleep(100);
            var appId = GetId();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            // Browse to the Login page
            // Retrieve the authUrl, ownerToken, and flowendpoint values from the page
            // Currently, Netflix performs a couple redirects to ensure that the
            // user's browser supports cookies.
            // HttpWebRequest handles these redirects automatically by default.
            var step1 = SendRequest("https://www.netflix.com/Login", null);
            doc.LoadHtml(step1);
            authUrl = doc.DocumentNode.Descendants("input").Where(i => i.GetAttributeValue("name", "") == "authURL").FirstOrDefault()?.GetAttributeValue("value", "");
            authUrl = FixAuthUrl(authUrl);

            string tokenValue = string.Empty;
            string flowEndpoint = string.Empty;
            string lolomoCookieName = string.Empty;

            var match = System.Text.RegularExpressions.Regex.Match(step1, "\"ownerToken\":\"(?<tokenValue>\\w+)\"");
            if (match.Success)
                tokenValue = match.Groups["tokenValue"].Value;
            match = System.Text.RegularExpressions.Regex.Match(step1, "flowendpoint\":\"(?<flowEndpoint>\\w+)\"");
            if (match.Success)
                flowEndpoint = match.Groups["flowEndpoint"].Value;


            // Generate the cL cookie
            // Send POST requests to log on the user
            var sequence = 8;
            string postString = string.Empty;
            string clCookie = $"{time}|{appId}|{sessionId}||{sequence}|{tokenValue}";
            cookies.Add(new Cookie("cL", clCookie));
            try
            {
                postString = $"{{'action':'loginAction','fields':{{'email':'{userName}','flow':'websiteSignUp','mode':'loginEmail','action':'loginAction','withFields':'email,nextPage','authURL':'{authUrl}','nextPage':'','password':'','rememberMe':'true'}},'authURL':'{authUrl}'}}";
                var step2 = SendRequest($"https://www.netflix.com/api/shakti/flowendpoint/{flowEndpoint}?mode=loginEmail&flow=websiteSignUp", postString);
                postString = $"{{'action':'loginAction','fields':{{'email':'{userName}','flow':'websiteSignUp','mode':'loginEmail','action':'loginAction','withFields':'email,nextPage','authURL':'{authUrl}','nextPage':'','password':'{password}','rememberMe':'true'}},'authURL':'{authUrl}'}}";
                var step3 = SendRequest($"https://www.netflix.com/api/shakti/flowendpoint/{flowEndpoint}?mode=loginPassword&flow=websiteSignUp", postString);

                var errorObj = JObject.Parse(step3)["fields"]?["errorCode"];
                if (errorObj != null)
                    return false;

            }
            catch (WebException)
            {
                var step2 = SendRequest("https://www.netflix.com/Login", $"email={userName}&password={password}&rememberMe=true&flow=websiteSignUp&mode=login&action=loginAction&withFields=email,password,rememberMe,nextPage&authURL={authUrl}&nextPage=", "application/x-www-form-urlencoded; charset=UTF-8");

            }

            // After the user logs on, a list of profiles is typically shown
            // Redirect to the Browse page to see the list of profiles
            // Grab the authUrl, switch guid, and the first profile token from the page
            // Use these values and navigate to that profile page
            Cookie[] savedCookies = new Cookie[cookies.Count];
            cookies.CopyTo(savedCookies, 0);
            var profiles = SendRequest("https://www.netflix.com/browse", null, useResponseCookies:true);
            savedCookies.ToList().ForEach(c => cookies.Add(c));
            match = System.Text.RegularExpressions.Regex.Match(profiles, "\"authURL\":\"(?<authUrl>.*?)\"");
            if (match.Success)
                authUrl = FixAuthUrl(match.Groups["authUrl"].Value);
            match = System.Text.RegularExpressions.Regex.Match(profiles, "switch\":\"(?<switchGuid>.*?)\"");
            string switchGuid = string.Empty;
            string switchProfileGuid = string.Empty;
            if (match.Success)
                switchGuid = match.Groups["switchGuid"].Value;
            //match = System.Text.RegularExpressions.Regex.Match(profiles, "\"profiles\":{\"(?<switchProfileGuid>.*?)\"");
            match = System.Text.RegularExpressions.Regex.Match(profiles, "\"isAccountOwner\":true,\"guid\":\"(?<switchProfileGuid>.*?)\",\"profileName\"");
            if (match.Success)
                switchProfileGuid = match.Groups["switchProfileGuid"].Value;
            match = System.Text.RegularExpressions.Regex.Match(profiles, "\"lolomoCookieName\":\"(?<lolomoCookieName>.*?)\"");
            if (match.Success)
                lolomoCookieName = match.Groups["lolomoCookieName"].Value;
            savedCookies = new Cookie[cookies.Count];
            cookies.CopyTo(savedCookies, 0);
            string url = $"https://www.netflix.com/api/shakti/profiles/switch/{switchGuid}?switchProfileGuid={switchProfileGuid}&_={GetTime(DateTime.Now)}&authURL={authUrl}";
            var changeProfile = SendRequest(url, null, useResponseCookies: true);
            savedCookies.ToList().ForEach(c => cookies.Add(c));

            pathEvaluator = string.Empty;
            match = System.Text.RegularExpressions.Regex.Match(profiles, "pathEvaluator\":\"(?<pathEvaluator>.*?)\"");
            if (match.Success)
                pathEvaluator = match.Groups["pathEvaluator"].Value;
            match = System.Text.RegularExpressions.Regex.Match(profiles, "\"authURL\":\"(?<authUrl>.*?)\"");
            if (match.Success)
                authUrl = FixAuthUrl(match.Groups["authUrl"].Value);

            cookies.Add(new Cookie("profilesNewSession", "0"));

            savedCookies = new Cookie[cookies.Count];
            cookies.CopyTo(savedCookies, 0);
            var browse = SendRequest($"https://www.netflix.com/browse", null, useResponseCookies: true);
            savedCookies.ToList().ForEach(c => cookies.Add(c));
            var lolomoCookieValue = cookies[lolomoCookieName].Value;
            match = System.Text.RegularExpressions.Regex.Match(browse, "\"mylist\":\\[\"lolomos\",\"(?<lolomos>.*?)\"");
            if (match.Success)
                lolomos = FixAuthUrl(match.Groups["lolomos"]?.Value);

            isAuthenticated = true;

            return isAuthenticated;
        }
        private static string authUrl, pathEvaluator, lolomos;

        public static async System.Threading.Tasks.Task<List<Video>> GetVideosByCategoryAsync(long categoryId)
        {
            List<Video> videos = null;
            var task = System.Threading.Tasks.Task.Factory.StartNew<List<Video>>(() =>
            {
                videos = GetVideosByCategory(categoryId);
                return videos;
            });
            return await task;
        }

        private static Dictionary<long, List<Video>> cache = new Dictionary<long, List<Video>>();
        public static List<Video> GetVideosByCategory(long categoryId)
        {
            List<Video> videos = null;
            if (cache.ContainsKey(categoryId))
            {
                videos = cache[categoryId];
            }
            if (videos != null && videos.Count > 0)
                return videos;

            videos = new List<Video>();
            var postString = $"{{'paths':[['genres',{categoryId},'su',{{'from':0,'to':200}},['summary','title']],['genres',{categoryId},'su',{{'from':0,'to':200}},'boxarts','_342x192','webp']],'authURL':'{authUrl}'}}";
            var t = JObject.Parse(postString);
            var categoriesJson = SendRequest($"https://www.netflix.com/api/shakti/pathEvaluator/{pathEvaluator}?withSize=true&materialize=true&model=harris", postString);
            JObject jobject = JObject.Parse(categoriesJson);
            var videosObj = jobject["value"]?["videos"];
            if (videosObj != null)
            {
                videosObj.Children().OfType<JProperty>().Where(p => p.Name != "$size" && p.Name != "size").ToList().ForEach(p =>
                {
                    var title = p.Value["title"];
                    var imageUrl = p.Value["boxarts"]["_342x192"]["webp"]["url"];
                    var summary = p.Value["summary"];
                    var type = summary["type"].Value<string>();
                    string id = p.Name;

                    //string videoUrl = $"https://www.netflix.com/title/{id}";
                    string videoUrl = $"android-app://com.netflix.mediaclient/http/www.netflix.com/title/{id}";
                    
                    Video video = new Video { Title = title.Value<string>(), CardUrl = imageUrl.Value<string>(), Id = int.Parse(id), Type= System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.ToTitleCase(type), VideoUrl=videoUrl };
                    videos.Add(video);
                });
            }
            cache[categoryId] = videos;
            return videos;
        }

        public static async System.Threading.Tasks.Task AddVideoToMyListAsync(Video video)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(() => AddVideoToMyList(video));
        }

        public static void AddVideoToMyList(Video video)
        {
            var url = $"https://www.netflix.com/api/shakti/pathEvaluator/{pathEvaluator}?withSize=true&materialize=true&model=harris&method=call";
            var postString = $"{{'callPath':['lolomos','{lolomos}','addToList'],'params':['{lolomos}_8437759320454',1,['videos',{video.Id}],14170286,null,null],'paths':[],'pathSuffixes':[[['length','trackIds','context','displayName']],[{{'to':18}}]],'authURL':'{authUrl}'}}";
            var post = JObject.Parse(postString);
            var result = SendRequest(url, postString);
            video.IsInMyList = true;
            videoDetailCache[video.Id] = video;
        }

        public static async System.Threading.Tasks.Task RemoveVideoFromMyListAsync(Video video)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(() => RemoveVideoFromMyList(video));
        }

        public static void RemoveVideoFromMyList(Video video)
        {
            var url = $"https://www.netflix.com/api/shakti/pathEvaluator/{pathEvaluator}?withSize=true&materialize=true&model=harris&method=call";
            var postString = $"{{'callPath':['lolomos','{lolomos}','removeFromList'],'params':['{lolomos}_8437759320454',1,['videos',{video.Id}],14170286,null,null],'paths':[],'pathSuffixes':[[['length','trackIds','context','displayName']],[{{'to':18}}]],'authURL':'{authUrl}'}}";
            var post = JObject.Parse(postString);
            var result = SendRequest(url, postString);
            video.IsInMyList = false;
            videoDetailCache[video.Id] = video;
        }

        public static async System.Threading.Tasks.Task<Video> GetDetailedVideoInfoAsync(Video video)
        {
            return await System.Threading.Tasks.Task.Factory.StartNew(() => GetDetailedVideoInfo(video));
        }

        static Dictionary<int, Video> videoDetailCache = new Dictionary<int, Video>();
        public static Video GetDetailedVideoInfo(Video video)
        {
            Video cachedVideo = null;
            if (videoDetailCache.ContainsKey(video.Id))
            {
                cachedVideo = videoDetailCache[video.Id];
            }
            if (cachedVideo != null)
            {
                return cachedVideo;
            }

            video.Starring.Clear();
            video.Genres.Clear();
            video.BackgroundImageUrls.Clear();

            var url = $"https://www.netflix.com/api/shakti/pathEvaluator/{pathEvaluator}?withSize=true&materialize=true&model=harris";
            var postString = $"{{'paths':[['videos',{video.Id},['requestId','regularSynopsis','queue','releaseYear','maturity','hookEvidence','titleEvidence']],['videos',{video.Id},['BGImages'],470,'webp','trailers','summary'],['videos',{video.Id},'bb2OGLogo','_400x90','webp'],['videos',{video.Id},'genres',{{'from':0,'to':2}},['id','name']],['videos',{video.Id},'genres','summary'],['videos',{video.Id},'tags',{{'from':0,'to':9}},['id','name']],['videos',{video.Id},'tags','summary'],['videos',{video.Id},'cast',{{'from':0,'to':5}},['id','name']],['videos',{video.Id},'cast','summary'],['videos',{video.Id},'artWorkByType','BILLBOARD','_1280x720','webp'],['videos',{video.Id},'interestingMoment','_665x375','webp']],'authURL':'{authUrl}'}}";
            var result = SendRequest(url, postString);
            JObject jobject = JObject.Parse(result);

            var value = jobject["value"];
            var videos = value?["videos"];
            var videoObj = videos[$"{video.Id}"];

            // Load the cast
            var cast = videoObj?["cast"];
            if (cast != null)
            {
                // Get the top three cast members ordered by their IDs
                cast.Children().OfType<JProperty>().Where(c => c.Name != "$size" && c.Name != "size" && c.Name != "summary").OrderBy(o => o.Name).Take(3).ToList().ForEach(m =>
                {
                    if (m.Value.Type == JTokenType.Array && m.Value.ToArray().Length > 1)
                    {
                        var memberId = m.Value.ToArray()[1].Value<string>();

                        var castMemberObj = value?["person"]?.Children<JProperty>().Where(p => p.Name == memberId).FirstOrDefault();
                        if (castMemberObj != null)
                        {
                            string castMember = castMemberObj.Value["name"].Value<string>();
                            video.Starring.Add(castMember);
                        }
                    }
                });
            }
            //var cast = jobject["value"]?["person"];
            //if (cast != null)
            //{
            //    // Get the top three cast members ordered by their IDs
            //    cast.Children().OfType<JProperty>().Where(p => p.Name != "$size" && p.Name != "size").OrderBy(o => o.Name).Take(3).ToList().ForEach(p =>
            //    {
            //        string castMember = p.Value["name"].Value<string>();
            //        video.Starring.Add(castMember);
            //    });
            //}

            // Load the Genres
            var genres = value["genres"];
            if (genres != null)
            {
                // Get the top three genres associated with this video ordered by their IDs
                genres.Children().OfType<JProperty>().Where(p => p.Name != "$size" && p.Name != "size").OrderBy(o => o.Name).Take(3).ToList().ForEach(p =>
                {
                    string genre = p.Value["name"].Value<string>();
                    video.Genres.Add(genre);
                });
            }

            // Load the background images
            var backgroundImages = videoObj?["BGImages"]?["470"]?["webp"];
            if (backgroundImages != null)
            {
                backgroundImages.ToArray().ToList().ForEach(b =>
                {
                    var backgroundImageUrl = b["url"].Value<string>();
                    video.BackgroundImageUrls.Add(backgroundImageUrl);
                });
            }

            // Load common video details
            if (videoObj != null)
            {
                video.Synopsis = videoObj["regularSynopsis"].Value<string>();
                video.ReleaseYear = videoObj["releaseYear"].Value<short>();
                video.IsInMyList = bool.Parse(videoObj["queue"]["inQueue"].Value<string>());
                video.Rating = videoObj["maturity"]["rating"]["value"].Value<string>();
            }

            videoDetailCache[video.Id] = video;
            return video;
        }

        private static string FixAuthUrl(string authUrl)
        {
            Regex regex = new Regex(@"\\x[0-9][A-F]");
            var matches = regex.Matches(authUrl);
            foreach (Match match in matches)
            {
                authUrl = authUrl.Replace(match.Value, ((char)Convert.ToByte(match.Value.Replace(@"\x", ""), 16)).ToString());
            }
            return authUrl;
        }

        // This functionality was taken from the Netflix JavaScript.
        private static Int64 GetTime(DateTime date)
        {
            Int64 retval = 0;
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (date.ToUniversalTime() - st);
            retval = (Int64)(t.TotalMilliseconds + 0.5);
            return retval;
        }

        // This functionality was taken from the Netflix JavaScript.
        private static string GetId()
        {
            var e = Math.Floor((decimal)(GetTime(DateTime.Now) / 1000)).ToString();
            var t = Math.Floor(100000000 * new Random().NextDouble()).ToString();
            return e + t;
        }

        private static CookieCollection cookies;
        private static string SendRequest(string url, string postString, string contentType = "application/json", Dictionary<string, string> additionalHeaders = null, bool useResponseCookies = false)
        {
            Uri requestUri = new Uri(url);
            HttpWebRequest req = HttpWebRequest.Create(requestUri) as HttpWebRequest;

            req.CookieContainer = new CookieContainer();
            if (cookies != null && cookies.Count > 0) req.CookieContainer.Add(new Uri("https://www.netflix.com"), cookies);

            byte[] postData = new byte[0];
            if (postString != null)
            {
                req.Method = "POST";
                req.ContentType = contentType;

                postData = Encoding.UTF8.GetBytes(postString);
            }

            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
            req.KeepAlive = true;
            req.Headers["Upgrade-Insecure-Requests"] = "1";
            req.Headers["Cache-Control"] = "max-age=0";

            if (additionalHeaders != null)
            {
                foreach (KeyValuePair<string, string> pair in additionalHeaders)
                {
                    req.Headers[pair.Key] = pair.Value;
                }
            }

            var str = "";
            try
            {
                if (postString != null)
                {
                    using (System.IO.Stream requestStream = req.GetRequestStreamAsync().Result)
                    {
                        requestStream.Write(postData, 0, postData.Length);
                    }
                }
                using (HttpWebResponse res = req.GetResponseAsync().Result as HttpWebResponse)
                {
                    using (System.IO.Stream stream = res.GetResponseStream())
                    {
                        System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                        str = reader.ReadToEnd();
                    }

                    cookies = useResponseCookies ? res.Cookies : req.CookieContainer.GetCookies(new Uri("https://www.netflix.com"));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return str;
        }
    }
}