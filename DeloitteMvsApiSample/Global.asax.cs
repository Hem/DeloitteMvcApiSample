using DeloitteMvcApiSample.Helpers;
using DeloitteMvcApiSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DeloitteMvcApiSample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            PostAuthenticateRequest += Application_PostAuthenticateRequest;
        }

        private void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if(ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                var userToken = CacheKeyGenerator.GetUserCacheToken(ClaimsPrincipal.Current);

                var cachedUser = CacheHelper.GetOrSet<UserProfile>(userToken, 1, () => GetCurrentUserProfile("Hem"));                
            }
        }


       

        private UserProfile GetCurrentUserProfile(string name)
        {
            var random = new Random();

            return new UserProfile
            {
                UniqueName = name,
                Id = random.Next(int.MaxValue)
            };
        }
        
    }
}
