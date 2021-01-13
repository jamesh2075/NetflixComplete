using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using HtmlAgilityPack;
using System.Xml.Linq;

/// <summary>
/// This API allows apps to communicate with Netflix for the purposes
/// of browsing videos and adding them to a user's My List.
/// Author: James Henry
/// Date Started: August 26, 2016
/// </summary>

namespace NetflixComplete.Portable
{
    [JsonObject]
    public class Video
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

    public class UserProfile
    {
        public string AvatarUrl { get; set; }
        public string FirstName { get; set; }
        public bool IsPrimary { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string UserId { get; set; }
    }

    public class NetflixApi
    {
        #region Authentication
        public bool Authenticate(string userName, string password)
        {
            return AuthenticateByScreenScraping2(userName, password);
        }

        public async System.Threading.Tasks.Task<bool> AuthenticateAsync(string userName, string password)
        {
            var task = System.Threading.Tasks.Task.Factory.StartNew<bool>(() =>
            {
                return AuthenticateByUsingApiGlobal(userName, password);
            });
            return await task;
        }

        private bool AuthenticateByUsingApiGlobal(string userName, string password)
        {
            // Reset the cookies so new ones are generated.
            //cookies = null;
            _authToken = null;

            bool isAuthenticated = false;

            string url = "https://api-global.netflix.com/android/samurai/config?responseFormat=json&progressive=false&ffbc=phone&appVersion=5.2.0&dlEnabled=true&languages=en&pathFormat=hierarchical&method=get&res=high&imgpref=webp&path=['signInVerify']";
            url = "https://api-global.netflix.com/android/samurai/config?responseFormat=json&progressive=false&pathFormat=hierarchical&routing=reject&appType=samurai&dbg=false&qlty=sd&ffbc=phone&osBoard=universal7420&osDevice=zeroflte&osDisplay=PIXEL&appVer=7816&appVersion=4.6.0&mId=NULL&api=24&mnf=samsung&store=google&memLevel=high&lackLocale=false&deviceLocale=en&method=get&path=%5B%27signInVerify%27%5D";
            url = "https://api-global.netflix.com/android/samurai/config?path=['signInVerify']&appVersion=6.0.0";
            string flwssn = Guid.NewGuid().ToString();
            //url = "https://api-global.netflix.com/android/5.0/api/account/auth";
            string postData = $"email={userName}&password={password}&flwssn={flwssn}";
            //postData = $"{{'email':'{userName}','password':'{password}','secureNetflixId':'v=2','netflixId':'v=2'}}";
            var result = SendRequest(url, postData, "application/x-www-form-urlencoded; charset=UTF-8", useResponseCookies:true);
            //var result = SendRequest(url, postData, "application/json; charset=UTF-8", useResponseCookies: true);
            var jobject = JObject.Parse(result);
            var signInVerifyObject = jobject?["value"]?["signInVerify"];
            isAuthenticated = signInVerifyObject?["mode"]?.Value<string>() == "memberHome";

            if (isAuthenticated)
            {
                GenerateAuthToken();
            }

            return isAuthenticated;
        }

        private bool AuthenticateByScreenScraping2(string userName, string password)
        {
            cookies = null;
            _authToken = null;
            bool isAuthenticated = false;

            var time = GetTime(DateTime.Now);
            //System.Threading.Tasks.Task.Delay(100).RunSynchronously();
            var sessionId = GetId();
            //System.Threading.Tasks.Task.Delay(100).RunSynchronously();
            var appId = GetId();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            // Go to the login page
            var step1 = SendRequest("https://www.netflix.com/login", null);
            doc.LoadHtml(step1);
            authUrl = doc.DocumentNode.Descendants("input").Where(i => i.GetAttributeValue("name", "") == "authURL").FirstOrDefault()?.GetAttributeValue("value", "");
            authUrl = FixAuthUrl(authUrl);

            string tokenValue = string.Empty;
            var match = System.Text.RegularExpressions.Regex.Match(step1, "\"ownerToken\":(?<tokenValue>\\w+),");
            if (match.Success)
                tokenValue = match.Groups["tokenValue"].Value;


            // Log in with the email address and password
            var sequence = 4;
            string postString = string.Empty;
            string clCookie = $"{time}|{appId}|{sessionId}||{sequence}|{tokenValue}";
            cookies.Add(new Cookie("cL", clCookie));

            postString = $"userLoginId={WebUtility.UrlEncode(userName)}&password={WebUtility.UrlEncode(password)}&rememberMe=true&flow=websiteSignUp&mode=login&action=loginAction&withFields=rememberMe%2CnextPage%2CuserLoginId%2Cpassword%2CcountryCode%2CcountryIsoCode&authURL={WebUtility.UrlEncode(authUrl)}&nextPage=&showPassword=&countryCode=%2B1&countryIsoCode=US";
            //postString = $"userLoginId={userName}&password={password}&rememberMe=false&flow=websiteSignUp&mode=login&action=loginAction&withFields=rememberMe,nextPage,userLoginId,password,countryCode,countryIsoCode&authUrl={authUrl}&nextPage=&showPassword=&countryCode=+1&countryIsoCode=US";
            var step2 = SendRequest($"https://www.netflix.com/login", postString, "application/x-www-form-urlencoded");

            Cookie[] savedCookies = new Cookie[cookies.Count];
            cookies.CopyTo(savedCookies, 0);
            step2 = SendRequest($"https://www.netflix.com", null, useResponseCookies:true);
            savedCookies.ToList().ForEach(c => cookies.Add(c));


            // Go to the Profiles page to select a profile
            savedCookies = new Cookie[cookies.Count];
            cookies.CopyTo(savedCookies, 0);
            var profiles = SendRequest("https://www.netflix.com/browse", null, useResponseCookies: true);
            savedCookies.ToList().ForEach(c => cookies.Add(c));
            match = System.Text.RegularExpressions.Regex.Match(profiles, "\"authURL\":\"(?<authUrl>.*?)\"");
            if (match.Success)
                authUrl = FixAuthUrl(match.Groups["authUrl"].Value);

            // Location the owner profile
            string profileGuid = "";
            match = System.Text.RegularExpressions.Regex.Match(profiles, "\"profileName\":\"(?<profileName>.*?)\",\"guid\":\"(?<switchProfileGuid>.*?)\",\"isAccountOwner\":true");
            if (match.Success)
                profileGuid = match.Groups["switchProfileGuid"].Value;

            clCookie = $"{GetTime(DateTime.Now)}|{appId}|{sessionId}||{sequence}|{tokenValue}";
            cookies.Add(new Cookie("cL", clCookie));

            
            string url = $"https://www.netflix.com/SwitchProfile?tkn={profileGuid}";
            var changeProfile = SendRequest(url, null, useResponseCookies: false);
            

            //cookies.Add(new Cookie("profilesNewSession", "0"));

            //savedCookies = new Cookie[cookies.Count];
            //cookies.CopyTo(savedCookies, 0);
            //var browse = SendRequest("https://www.netflix.com/browse", null, useResponseCookies: true);
            //savedCookies.ToList().ForEach(c => cookies.Add(c));

            isAuthenticated = true;
            GenerateAuthToken();

            
            return isAuthenticated;
        }

        private bool AuthenticateByScreenScraping(string userName, string password)
        {
            // Reset the cookies so new ones are generated.
            cookies = null;
            _authToken = null;

            bool isAuthenticated = false;
            ;
            var time = GetTime(DateTime.Now);
            //System.Threading.Tasks.Task.Delay(100).RunSynchronously();
            var sessionId = GetId();
            //System.Threading.Tasks.Task.Delay(100).RunSynchronously();
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
            catch (AggregateException)
            {
                var step2 = SendRequest("https://www.netflix.com/Login", $"email={userName}&password={password}&rememberMe=true&flow=websiteSignUp&mode=login&action=loginAction&withFields=email,password,rememberMe,nextPage&authURL={authUrl}&nextPage=", "application/x-www-form-urlencoded; charset=UTF-8");
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
            var profiles = SendRequest("https://www.netflix.com/browse", null, useResponseCookies: true);
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
            //var lolomoCookieValue = cookies[lolomoCookieName].Value;
            //match = System.Text.RegularExpressions.Regex.Match(browse, "\"mylist\":\\[\"lolomos\",\"(?<lolomos>.*?)\"");
            //if (match.Success)
            //    lolomos = FixAuthUrl(match.Groups["lolomos"]?.Value);

            isAuthenticated = true;
            GenerateAuthToken();
            return isAuthenticated;
        }

        const char DELIMITER = '~';
        private void GenerateAuthToken()
        {
            string token = null;
            List<string> jsonCookies = new List<string>();
            foreach (Cookie cookie in cookies)
            {
                var jsonCookie = Newtonsoft.Json.JsonConvert.SerializeObject(cookie);
                jsonCookies.Add(jsonCookie);
            }
            token = String.Join(DELIMITER.ToString(), jsonCookies);

            //byte[] salt = System.Text.Encoding.UTF8.GetBytes("saltsalt");
            //var key = PCLCrypto.NetFxCrypto.DeriveBytes.GetBytes("password", salt, 1000, 32);
            //PCLCrypto.ISymmetricKeyAlgorithmProvider aes = PCLCrypto.WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(PCLCrypto.SymmetricAlgorithmName.Aes, PCLCrypto.SymmetricAlgorithmMode.Cbc, PCLCrypto.SymmetricAlgorithmPadding.PKCS7);
            //PCLCrypto.ICryptographicKey symmetricKey = aes.CreateSymmetricKey(key);
            //byte[] encryptedToken = PCLCrypto.WinRTCrypto.CryptographicEngine.Encrypt(symmetricKey, System.Text.Encoding.UTF8.GetBytes(token));
            //_authToken = Convert.ToBase64String(encryptedToken);

            byte[] compressedToken;
            using (var outStream = new System.IO.MemoryStream())
            {
                using (var tinyStream = new System.IO.Compression.GZipStream(outStream, System.IO.Compression.CompressionMode.Compress))
                using (var mStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(token)))
                    mStream.CopyTo(tinyStream);

                compressedToken = outStream.ToArray();
            }
            _authToken = Convert.ToBase64String(compressedToken);
        }
        private string _authToken = null;
        public string GetAuthToken()
        {
            return _authToken;
        }

        // Update: 12-08-2016
        // Ideally, the cookies returned from Netflix would be stored in memory
        // and associated with the user's returned token.
        // However, the plan is to host this component in a Web API app in the Azure cloud on a shared app plan.
        // This means that it's possible that Microsoft may round robin requests to different servers.
        // I don't want to pay for an Azure Storage account at this point since this is intended
        // to be a proof-of-concept project.
        // Therefore, instead of having the token stored on the server, it needs to be stored at the client
        // and passed with each request.
        private void SetCookiesFromAuthToken(string base64Token)
        {
            if (String.IsNullOrEmpty(base64Token))
                return;

            var token = Convert.FromBase64String(base64Token);
            cookies = new CookieCollection();
            string decompressedToken;
            using (var inStream = new System.IO.MemoryStream(token))
            using (var bigStream = new System.IO.Compression.GZipStream(inStream, System.IO.Compression.CompressionMode.Decompress))
            using (var bigStreamOut = new System.IO.MemoryStream())
            {
                bigStream.CopyTo(bigStreamOut);
                decompressedToken = Encoding.UTF8.GetString(bigStreamOut.ToArray(), 0, (int)bigStreamOut.Length);
            }

            var cookieElements = decompressedToken.Split(DELIMITER);
            cookieElements.ToList().ForEach(elem =>
            {
                Cookie cookie = JsonConvert.DeserializeObject<Cookie>(elem);
                if (!String.IsNullOrEmpty(cookie.Domain))
                    cookies.Add(cookie);
            });
        }
        #endregion

        #region Profiles
        public void SwitchUserProfile(UserProfile profile, string token=null)
        {
            SwitchUserProfileByUsingGlobalApi(profile, token);
        }
        public async System.Threading.Tasks.Task SwitchUserProfileAsync(UserProfile profile, string token = null)
        {
            var task = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                SwitchUserProfileByUsingGlobalApi(profile, token);
            });
            await task;
        }

        private string SwitchUserProfileByUsingGlobalApi(UserProfile profile, string token=null)
        {
            SetCookiesFromAuthToken(token);

            string url = $"https://api-global.netflix.com/android/5.2/api?responseFormat=json&progressive=false&routing=reject&res=high&imgpref=webp&ffbc=phone&appVersion=5.2.0&languages=en&pathFormat=hierarchical&method=call&callPath=['switchProfile']&param='{profile.ProfileId}'";
            string result = SendRequest(url, null);

            var jobject = JObject.Parse(result);
            var userTokens = jobject?["value"]?["profile"]?[$"{profile.ProfileId}"]?["userTokens"];
            var netflixId = userTokens?["NetflixId"].Value<string>();
            var secureNetflixId = userTokens?["SecureNetflixId"].Value<string>();
            cookies["NetflixId"].Value = netflixId;
            cookies["SecureNetflixId"].Value = secureNetflixId;
            GenerateAuthToken();
            return _authToken;
        }

        public List<UserProfile> GetUserProfiles(string token = null)
        {
            return GetUserProfilesByUsingGlobalApi(token);
        }

        public async System.Threading.Tasks.Task<List<UserProfile>> GetUserProfilesAsync(string token = null)
        {
            var task = System.Threading.Tasks.Task.Factory.StartNew<List<UserProfile>>(() =>
            {
                return GetUserProfilesByUsingGlobalApi(token);
            });
            return await task;
        }

        private List<UserProfile> GetUserProfilesByUsingGlobalApi(string token=null)
        {
            SetCookiesFromAuthToken(token);

            List<UserProfile> profiles = new List<UserProfile>();
            
            string url = "https://api-global.netflix.com/android/4.8.1/api?responseFormat=json&progressive=false&routing=reject&res=high&imgpref=webp&ffbc=phone&appVersion=4.8.4&languages=en&pathFormat=hierarchical&method=get&path=['profilesList', 'summary']&path=['profilesList', {'to':5}, ['summary', 'subtitlePreference']]&path=['user', ['summary', 'subtitleDefaults', 'umaEog']]";
            var result = SendRequest(url, null);
            var jobject = JObject.Parse(result);
            var profilesList = jobject["value"]?["profilesList"];
            if (profilesList != null)
            {
                profilesList.Children().OfType<JProperty>().Where(p => p.Name != "summary").ToList().ForEach(p =>
                {
                    var summary = p.Value["summary"];

                    UserProfile profile = new UserProfile
                    {
                        AvatarUrl = summary["avatarUrl"].Value<string>(),
                        //FirstName = summary["firstName"].Value<string>(),
                        UserId = summary["userId"].Value<string>(),
                        ProfileId = summary["profileId"].Value<string>(),
                        ProfileName = summary["profileName"].Value<string>(),
                        IsPrimary = summary["isPrimaryProfile"].Value<bool>()
                    };
                    profiles.Add(profile);
                });
            }

            return profiles;
        }
        #endregion

        #region Videos
        public List<Video> GetVideosByCategory(long categoryId, string token = null)
        {
            return GetVideosByCategoryUsingApiGlobal(categoryId, token);
        }

        public async System.Threading.Tasks.Task<List<Video>> GetVideosByCategoryAsync(long categoryId, string token = null)
        {
            List<Video> videos = null;
            var task = System.Threading.Tasks.Task.Factory.StartNew<List<Video>>(() =>
            {
                videos = GetVideosByCategoryUsingApiGlobal(categoryId, token);
                return videos;
            });
            return await task;
        }

        private static Dictionary<long, List<Video>> cache = new Dictionary<long, List<Video>>();

        private List<Video> GetVideosByCategoryUsingApiGlobal(long categoryId, string token=null)
        {
            SetCookiesFromAuthToken(token);

            List<Video> videos = null;
            if (cache.ContainsKey(categoryId))
            {
                videos = cache[categoryId];
            }
            if (videos != null && videos.Count > 0)
                return videos;

            videos = new List<Video>();

            //string url = $"https://api-global.netflix.com/android/5.3/api?method=get&materialize=true&path=%5B%22topGenres%22%2C%22{categoryId}%22%2C%7B%22from%22%3A0%2C%22to%22%3A21%7D%2C%7B%22from%22%3A0%2C%22to%22%3A11%7D%2C%22summary%22%5D&path=%5B%22topGenres%22%2C%22{categoryId}%22%2C%7B%22from%22%3A0%2C%22to%22%3A21%7D%2C%22summary%22%5D&responseFormat=json&progressive=false&ffbc=phone&appVersion=5.3.0&languages=en-US&dlEnabled=true&pathFormat=graph&res=high&imgpref=webp";
            string url = $"https://api-global.netflix.com/android/5.3/api?method=get&materialize=true&path=%5B%22flatGenre%22%2C%22{categoryId}%22%2C%7B%22from%22%3A0%2C%22to%22%3A100%7D%2C%22summary%22%2C%22tvCardArt%22%5D&responseFormat=json&progressive=false&ffbc=phone&appVersion=5.3.0&languages=en-US&dlEnabled=true&pathFormat=graph&res=high&imgpref=webp";
            string result = SendRequest(url, null);

            JObject jobject = JObject.Parse(result);
            jobject["value"].Children<JProperty>().Where(c => c.Name == "movies" || c.Name == "shows").ToList().ForEach(obj =>
            {
                obj.Value.Children<JProperty>().ToList().ForEach(videoObj =>
                {
                    var summary = videoObj.Value["summary"];
                    var detail = videoObj.Value["detail"];

                    var title = summary["title"].Value<string>();
                    int? year = detail?["year"]?.Value<int>();
                    string imageUrl = (string)(detail?["tvCardUrl"]?.Value<string>() ?? summary["boxartUrl"]);
                    var type = summary["type"].Value<string>();
                    int id = int.Parse(videoObj.Name);

                    //string videoUrl = $"https://www.netflix.com/title/{id}";
                    string videoUrl = $"android-app://com.netflix.mediaclient/http/www.netflix.com/title/{id}";

                    Video video = new Video { Title = title, CardUrl = imageUrl, Id = id, Type = type.ToTitleCase(), VideoUrl = videoUrl };
                    videos.Add(video);
                });
            });

            cache[categoryId] = videos;
            return videos;
        }

        private static string authUrl, pathEvaluator, lolomos;

        private List<Video> GetVideosByCategoryOld(long categoryId, string token=null)
        {
            SetCookiesFromAuthToken(token);

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
                    
                    Video video = new Video { Title = title.Value<string>(), CardUrl = imageUrl.Value<string>(), Id = int.Parse(id), Type= type.ToTitleCase(), VideoUrl=videoUrl };
                    videos.Add(video);
                });
            }
            cache[categoryId] = videos;
            return videos;
        }

        public Video GetDetailedVideoInfo(Video video, string token = null)
        {
            return GetDetailedVideoInfoByUsingApiGlobal(video, token);
        }

        public async System.Threading.Tasks.Task<Video> GetDetailedVideoInfoAsync(Video video, string token = null)
        {
            return await System.Threading.Tasks.Task.Factory.StartNew(() => GetDetailedVideoInfoByUsingApiGlobal(video, token));
        }

        static Dictionary<int, Video> videoDetailCache = new Dictionary<int, Video>();
        private Video GetDetailedVideoInfoByUsingApiGlobal(Video video, string token = null)
        {
            SetCookiesFromAuthToken(token);

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

            string type = video.Type.ToLower();
            var url = $"https://api-global.netflix.com/android/5.3/api?method=get&materialize=true&path=[\"movies\",\"{video.Id}\",\"detail\"]&path=[\"movies\",\"{video.Id}\",\"defaultTrailer\"]&path=[\"movies\",\"{video.Id}\",\"bookmark\"]&path=[\"movies\",\"{video.Id}\",\"inQueue\"]&path=[\"movies\",\"{video.Id}\",\"offlineAvailable\"]&path=[\"movies\",\"{video.Id}\",\"evidence\"]&path=[\"movies\",\"{video.Id}\",\"similars\",\"summary\"]&path=[\"movies\",\"{video.Id}\",\"similars\",{{\"from\":0,\"to\":11}},\"summary\"]&path=[\"movies\",\"{video.Id}\",\"rating\"]&responseFormat=json&progressive=false&ffbc=phone&appVersion=5.3.0&languages=en-US&dlEnabled=true&pathFormat=graph&res=high&imgpref=webp";
            var result = SendRequest(url, null);
            JObject jobject = JObject.Parse(result);

            var value = jobject["value"];
            var videos = value?[type];
            var videoObj = videos?[$"{video.Id}"];

            // Load the cast
            var cast = videoObj?["detail"]?["actors"];
            if (cast != null)
            {
                string[] actors = cast.Value<string>().Split(',').Where(a => !String.IsNullOrEmpty(a)).ToArray();
                video.Starring.AddRange(actors);
            }

            // Load the Genres
            var genres = videoObj?["detail"]?["genres"];
            if (genres != null)
            {
                string[] videoGenres = genres.Value<string>().Split(',');
                video.Genres.AddRange(videoGenres);
            }

            // Load the background images
            string interestingUrl = videoObj?["detail"]?["interestingUrl"]?.Value<string>();
            if (!String.IsNullOrEmpty(interestingUrl))
                video.BackgroundImageUrls.Add(interestingUrl);
            string storyImgUrl = videoObj?["detail"]?["storyImgUrl"]?.Value<string>();
            if (!String.IsNullOrEmpty(storyImgUrl))
                video.BackgroundImageUrls.Add(storyImgUrl);
            string hiResHorzUrl = videoObj?["detail"]?["hiResHorzUrl"]?.Value<string>();
            if (!String.IsNullOrEmpty(hiResHorzUrl))
                video.BackgroundImageUrls.Add(hiResHorzUrl);
            //var backgroundImages = videoObj?["BGImages"]?["470"]?["webp"];
            //if (backgroundImages != null)
            //{
            //    backgroundImages.ToArray().ToList().ForEach(b =>
            //    {
            //        var backgroundImageUrl = b["url"].Value<string>();
            //        video.BackgroundImageUrls.Add(backgroundImageUrl);
            //    });
            //}

            // Load common video details
            if (videoObj != null)
            {
                video.Synopsis = videoObj["detail"]["synopsis"].Value<string>();
                video.ReleaseYear = (short)videoObj["detail"]["year"].Value<short>();
                video.IsInMyList = bool.Parse(videoObj["inQueue"]["inQueue"].Value<string>());
                video.Rating = videoObj["detail"]["certification"].Value<string>();
            }

            videoDetailCache[video.Id] = video;
            return video;
        }
        private Video GetDetailedVideoInfoOld(Video video, string token = null)
        {
            SetCookiesFromAuthToken(token);

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
        #endregion

        #region MyList
        public void AddVideoToMyList(Video video, string token=null)
        {
            AddVideoToMyListByUsingApiGlobal(video, token);
        }

        public async System.Threading.Tasks.Task AddVideoToMyListAsync(Video video, string token=null)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(() => AddVideoToMyListByUsingApiGlobal(video, token));
        }

        private void AddVideoToMyListByUsingApiGlobal(Video video, string token=null)
        {
            SetCookiesFromAuthToken(token);

            string type = video.Type.ToLower();
            var url = $"https://api-global.netflix.com/android/5.0/api?method=call&materialize=true&callPath=[\"lolomos\",\"{Guid.NewGuid()}_ROOT\",\"add\"]&responseFormat=json&progressive=false&ffbc=phone&appVersion=5.0.4&languages=en-US&dlEnabled=true&pathFormat=graph&res=high&imgpref=webp&param='830ca49f-2b5d-4213-ae26-273b80406390_12205839X6XX1499657747125'&param=20&param=['{type}','{video.Id}']&param=-1&pathSuffix=[{{'from':0,'to':11}}]&pathSuffix=['summary']";
            var result = SendRequest(url, null);
            var jobject = JObject.Parse(result);
            //video.IsInMyList = jobject?["value"]?[type]?[video.Id]?["inQueue"]?["inQueue"]?.Value<bool>() ?? false;
            video.IsInMyList = true;
            videoDetailCache[video.Id] = video;
        }

        private void AddVideoToMyListOld(Video video, string token=null)
        {
            SetCookiesFromAuthToken(token);

            var url = $"https://www.netflix.com/api/shakti/pathEvaluator/{pathEvaluator}?withSize=true&materialize=true&model=harris&method=call";
            var postString = $"{{'callPath':['lolomos','{lolomos}','addToList'],'params':['{lolomos}_8437759320454',1,['videos',{video.Id}],14170286,null,null],'paths':[],'pathSuffixes':[[['length','trackIds','context','displayName']],[{{'to':18}}]],'authURL':'{authUrl}'}}";
            var post = JObject.Parse(postString);
            var result = SendRequest(url, postString);
            video.IsInMyList = true;
            videoDetailCache[video.Id] = video;
        }

        public void RemoveVideoFromMyList(Video video, string token = null)
        {
            RemoveVideoFromMyListByUsingApiGlobal(video, token);
        }

        public async System.Threading.Tasks.Task RemoveVideoFromMyListAsync(Video video, string token=null)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(() => RemoveVideoFromMyListByUsingApiGlobal(video, token));
        }

        private void RemoveVideoFromMyListByUsingApiGlobal(Video video, string token = null)
        {
            SetCookiesFromAuthToken(token);

            string type = video.Type.ToLower();
            var url = $"https://api-global.netflix.com/android/5.2/api?method=call&materialize=true&callPath=[\"lolomos\",\"{Guid.NewGuid()}_ROOT\",\"remove\"]&responseFormat=json&progressive=false&ffbc=phone&appVersion=5.2.0&languages=en-US&dlEnabled=true&pathFormat=graph&res=high&imgpref=webp&param='830ca49f-2b5d-4213-ae26-273b80406390_12205839X6XX1499657747125'&param=20&param=['{type}','{video.Id}']&param=-1&pathSuffix=[{{'from':0,'to':11}}]&pathSuffix=['summary']";
            var result = SendRequest(url, null);
            var jobject = JObject.Parse(result);
            //video.IsInMyList = jobject?["value"]?[type]?[video.Id]?["inQueue"]?["inQueue"]?.Value<bool>() ?? false;
            video.IsInMyList = false;
            videoDetailCache[video.Id] = video;
        }

        private void RemoveVideoFromMyListOld(Video video, string token=null)
        {
            SetCookiesFromAuthToken(token);

            var url = $"https://www.netflix.com/api/shakti/pathEvaluator/{pathEvaluator}?withSize=true&materialize=true&model=harris&method=call";
            var postString = $"{{'callPath':['lolomos','{lolomos}','removeFromList'],'params':['{lolomos}_8437759320454',1,['videos',{video.Id}],14170286,null,null],'paths':[],'pathSuffixes':[[['length','trackIds','context','displayName']],[{{'to':18}}]],'authURL':'{authUrl}'}}";
            var post = JObject.Parse(postString);
            var result = SendRequest(url, postString);
            video.IsInMyList = false;
            videoDetailCache[video.Id] = video;
        }
        #endregion

        private static string FixAuthUrl(string authUrl)
        {
            if (authUrl == null)
                return null;

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

        private CookieCollection cookies;
        private string SendRequest(string url, string postString, string contentType = "application/json", Dictionary<string, string> additionalHeaders = null, bool useResponseCookies = false)
        {
            Uri requestUri = new Uri(url);
            HttpWebRequest req = HttpWebRequest.Create(requestUri) as HttpWebRequest;

            req.CookieContainer = new CookieContainer();
            if (cookies != null && cookies.Count > 0) req.CookieContainer.Add(new Uri("https://netflix.com"), cookies);

            byte[] postData = new byte[0];
            if (postString != null)
            {
                req.Method = "POST";
                req.ContentType = contentType;

                postData = Encoding.UTF8.GetBytes(postString);
            }

            //req.Headers["User-Agent"] = "Mozilla/5.2 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
            //req.Headers["Connection"] = "keep-alive";
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

                    cookies = useResponseCookies ? res.Cookies : req.CookieContainer.GetCookies(new Uri("https://netflix.com"));
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