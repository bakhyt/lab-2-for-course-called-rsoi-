using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AttributeRouting.Web.Mvc;
using ModelLib;
using MvcApplication1.Models;
using System.Web.Http;

namespace MvcApplication1.Controllers
{

    public class OauthController : ApiController
    {
        [Route("~/api/oauth/token")]
        public AuthParams Post(TokenParams tokenParams)
        {
            AuthParams par = new AuthParams();
            // создать access_token
            using (UsersContext context = new UsersContext())
            {
                UserProfile profile = null;
                if (tokenParams.grant_type == "authorization_code")
                {
                    profile = context.UserProfiles.Where(x => x.UserName == tokenParams.code).FirstOrDefault();
                }
                else
                {
                    profile = context.UserProfiles.Where(x => x.RefreshToken == tokenParams.refresh_token).FirstOrDefault();
                }
                if (profile != null)
                {
                    par.access_token = Guid.NewGuid().ToString();
                    par.token_type = "bearer";
                    par.refresh_token = Guid.NewGuid().ToString();
                    par.expires_in = 60;

                    profile.Token = par.access_token;
                    profile.RefreshToken = par.refresh_token;
                    profile.ExpirationDate = DateTime.Now.AddSeconds(par.expires_in);
                    // сохраняем параметры доступа по токену
                    context.SaveChanges();
                }
            }
            return par;
        }
    }
}