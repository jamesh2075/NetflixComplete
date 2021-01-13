using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using NetflixComplete.Portable;
using Newtonsoft;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ConsoleTest.exe [NetflixUserName] [NetflixPassword]");
            }
            string token = string.Empty;
            string userName = args[0];
            string password = args[1];
            HttpClient c = new HttpClient();
            c.BaseAddress = new Uri("http://localhost:64349");

            // Login
            c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string json = $"{{\"UserName\":\"{userName}\",\"Password\":\"{password}\"}}";
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var authResponse = c.PostAsync("api/auth", content).Result;
            token = authResponse.Content.ReadAsStringAsync().Result;
            c.DefaultRequestHeaders.Add("NetflixCustomerToken", token);

            // Get all profiles
            var profilesResponse = c.GetAsync("api/profiles").Result;
            var allProfilesJsonString = profilesResponse.Content.ReadAsStringAsync().Result;
            var userProfiles = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile[]>(allProfilesJsonString);

            // Switch to one of the profiles
            json = $"{{\"ProfileId\":\"{userProfiles[0].ProfileId}\" }}";
            content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var switchProfileResponse = c.PostAsync("api/profiles/switch", content).Result;

            // Get all of the categories
            var allCategoriesResponse = c.GetAsync("api/categories").Result;
            var allCategoriesJsonString = allCategoriesResponse.Content.ReadAsStringAsync().Result;
            var allCategoriesDynamic = Newtonsoft.Json.Linq.JArray.Parse(allCategoriesJsonString);
            var allCategoriesArray = allCategoriesDynamic.ToList().ConvertAll((category) =>
            {
                return new { Id = category.Value<long>("Id"), Name = category.Value<string>("Name") };
            }
            );

            // Get videos for one of the categories
            //var videosResponse = c.GetAsync("api/videos/category/1365").Result;
            //var videosJsonString = videosResponse.Content.ReadAsStringAsync().Result;

            //allCategoriesArray.ForEach(category =>
            //{
            //    var res = c.GetAsync($"api/videos/category/{category.Id}").Result;
            //    var str = res.Content.ReadAsStringAsync().Result;
            //    var allVideos = Newtonsoft.Json.Linq.JArray.Parse(str);
            //    Console.WriteLine($"{category.Name} ({allVideos.Count} videos)"); ;
            //});


            var api1 = new NetflixComplete.Portable.NetflixApi();
            var a = api1.Authenticate($"{userName}", $"{password}");
            if (a)
            {
                token = api1.GetAuthToken();
                var p = api1.GetUserProfilesAsync(token).Result;
                p = api1.GetUserProfiles(token);
                p = api1.GetUserProfilesAsync(token).Result;
                api1.SwitchUserProfileAsync(p[0], token).Wait();
                //token = api1.GetAuthToken();
                var videos = api1.GetVideosByCategoryAsync(1365, token).Result;
                videos = api1.GetVideosByCategoryAsync(4922, token).Result;
                videos = api1.GetVideosByCategoryAsync(4922, token).Result;
                videos = api1.GetVideosByCategoryAsync(83, token).Result;

                Console.WriteLine("********************M O V I E S********************");
                videos.Where(v => v.Type.ToLower() == "movies").ToList().ForEach(v => Console.WriteLine($"{v.Title}"));
                Console.WriteLine("********************S H O W S********************");
                videos.Where(v => v.Type.ToLower() == "shows").ToList().ForEach(v => Console.WriteLine($"{v.Title}"));
                var testVideo = api1.GetDetailedVideoInfoAsync(videos[1], token).Result;
                api1.AddVideoToMyListAsync(testVideo, token).Wait();
                api1.RemoveVideoFromMyListAsync(testVideo, token).Wait();
                

                //p = api1.GetUserProfiles();
                //var videos = api1.GetVideosByCategoryUsingGlobalApi(1365, token);
                //videos = api1.GetVideosByCategory(1365);
                Console.ReadKey();
            }
        }
    }
}
