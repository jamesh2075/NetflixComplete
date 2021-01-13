using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NetflixComplete.Portable;
namespace NetflixComplete.WebApi.Controllers
{
    [Filters.TokenAuthorizationFilter]
    public class VideosController : ApiController
    {
        private NetflixApi api = new NetflixApi();
        // GET: api/Videos
        [Route("api/videos/category/{id}")]
        async public System.Threading.Tasks.Task<IEnumerable<Video>> GetByCategory(long id)
        {
            var token = (string)this.ControllerContext.RouteData.Values["token"];
            return await api.GetVideosByCategoryAsync(id, token);
        }

        [Route("api/videos/details")]
        async public System.Threading.Tasks.Task<Video> LoadDetails(Video video)
        {
            if (video == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content= new StringContent("You must specify a valid video."),
                    ReasonPhrase = "You must specify a valid video."
                };
                throw new HttpResponseException(response);
            }
            var token = (string)this.ControllerContext.RouteData.Values["token"];
            return await api.GetDetailedVideoInfoAsync(video, token);
        }

        [Route("api/videos/addtolist")]
        [HttpPost]
        async public void AddToList(Video video, UserProfile profile)
        {
            if (video == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("You must specify a valid video."),
                    ReasonPhrase = "You must specify a valid video."
                };
                throw new HttpResponseException(response);
            }
            if (profile == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("You must specify a valid user profile."),
                    ReasonPhrase = "You must specify a valid user profile."
                };
                throw new HttpResponseException(response);
            }
            var token = (string)this.ControllerContext.RouteData.Values["token"];
            await api.SwitchUserProfileAsync(profile, token);
            await api.AddVideoToMyListAsync(video, token);
        }

        [Route("api/videos/removefromlist")]
        [HttpPost]
        async public void RemoveFromList(Video video, UserProfile profile)
        {
            if (video == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("You must specify a valid video."),
                    ReasonPhrase = "You must specify a valid video."
                };
                throw new HttpResponseException(response);
            }
            if (profile == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("You must specify a valid user profile."),
                    ReasonPhrase = "You must specify a valid user profile."
                };
                throw new HttpResponseException(response);
            }
            var token = (string)this.ControllerContext.RouteData.Values["token"];
            await api.SwitchUserProfileAsync(profile, token);
            await api.RemoveVideoFromMyListAsync(video, token);
        }
    }
}
