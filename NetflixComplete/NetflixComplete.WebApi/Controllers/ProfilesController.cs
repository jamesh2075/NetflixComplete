using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using NetflixComplete.Portable;

namespace NetflixComplete.WebApi.Controllers
{
    [Filters.TokenAuthorizationFilter]
    public class ProfilesController : ApiController
    {
        NetflixApi api = new NetflixApi();

        // GET: api/Profiles
        public IEnumerable<UserProfile> Get()
        {
            var token = (string)this.ControllerContext.RouteData.Values["token"];
            return api.GetUserProfiles(token);
        }

        // GET: api/Profiles/ABCDEFG
        public UserProfile Get(string id)
        {
            var token = (string)this.ControllerContext.RouteData.Values["token"];
            return api.GetUserProfiles(token).Where(p => p.ProfileId == id).FirstOrDefault();
        }

        [Route("api/profiles/primary")]
        public UserProfile GetPrimary()
        {
            var token = (string)this.ControllerContext.RouteData.Values["token"];
            return api.GetUserProfiles(token).Where(p => p.IsPrimary).FirstOrDefault();
        }

        [Route("api/profiles/switch")]
        public void Post(UserProfile profile)
        {
            var token = (string)this.ControllerContext.RouteData.Values["token"];
            api.SwitchUserProfile(profile, token);
        }
    }
}
