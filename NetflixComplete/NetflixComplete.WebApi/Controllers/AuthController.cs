using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NetflixComplete.WebApi.Controllers
{
    public class AuthController : ApiController
    {
        // POST: api/Auth
        public async System.Threading.Tasks.Task<HttpResponseMessage> Post(Auth auth)
        {
            HttpResponseMessage message = null;
            if (auth == null)
            {
                message = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest) { ReasonPhrase = $"The request is not valid. Please use the syntax {{'UserName':'[NetflixUserName]', 'Password':'[NetflixPassword]'}}" };
            }
            else if (String.IsNullOrEmpty(auth.UserName))
            {
                message = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest) { ReasonPhrase = $"The request is not valid. Please use the syntax {{'UserName':'[NetflixUserName]', 'Password':'[NetflixPassword]'}}" };
            }
            else if (String.IsNullOrEmpty(auth.Password))
            {
                message = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest) { ReasonPhrase = $"The request is not valid. Please use the syntax {{'UserName':'[NetflixUserName]', 'Password':'[NetflixPassword]'}}" };
            }
            else
            {
                string token = string.Empty;
                NetflixComplete.Portable.NetflixApi api = new Portable.NetflixApi();
                try
                {
                    bool isAuthenticated = await api.AuthenticateAsync(auth.UserName, auth.Password);
                    if (!isAuthenticated)
                    {
                        message = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized) { ReasonPhrase = "The Netflix username or password is incorrect." };
                    }
                    else
                    {
                        token = api.GetAuthToken();
                        message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(token) };
                    }
                }
                catch (Exception)
                {
                    // TODO: Log the exception

                    message = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError) { ReasonPhrase = "An error occurred while trying to authenticate the Netflix user. Please try again later." };
                }
            }

            return message;
        }
    }

    public class Auth
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
