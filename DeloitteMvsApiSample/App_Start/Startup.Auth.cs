﻿using System;
using System.Configuration;
using System.IdentityModel.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Owin;
using DeloitteMvcApiSample.Models;
using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OAuth;

namespace DeloitteMvcApiSample
{
    public partial class Startup
    {

        private static string domain = ConfigurationManager.AppSettings["ida:Domain"];
        private static string apiClientId = ConfigurationManager.AppSettings["ida:ApiClientId"];

        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = EnsureTrailingSlash(ConfigurationManager.AppSettings["ida:AADInstance"]);
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        public static readonly string Authority = aadInstance + tenantId;

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        string graphResourceId = "https://graph.windows.net";

        public void ConfigureAuth(IAppBuilder app)
        {
            // db context is not used on page, I believe call is here only to initialize things...
            ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = domain,
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        SaveSigninToken = true,
                        ValidAudience = apiClientId
                    },
                    Provider = new OAuthBearerAuthenticationProvider()
                    {
                        // claim augmentation... 
                        // Add additional elements to the claim
                        // Note: This is called for every request, we may want to "cache" elements.
                        OnValidateIdentity = (context) =>
                        {
                            context.Ticket.Identity.AddClaim(
                                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Administrator")
                                );

                            return Task.FromResult(0);
                        }
                    }
                });
            
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = Authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        //s If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        AuthorizationCodeReceived = (context) =>
                        {
                            var code = context.Code;
                            ClientCredential credential = new ClientCredential(clientId, appKey);
                            string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                            AuthenticationContext authContext = new AuthenticationContext(Authority, new ADALTokenCache(signedInUserID));
                            AuthenticationResult result = authContext.AcquireTokenByAuthorizationCodeAsync(
                                code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, graphResourceId).Result;

                            return Task.FromResult(0);
                        },

                        // Claim Augmentation:
                        // Add additional claims when the request is authenticated
                        // Note: This is called once and then the claims are stored in a cookie or the reference is.
                        SecurityTokenValidated = (context) =>
                        {
                            context.AuthenticationTicket.Identity.AddClaim(
                                    new System.Security.Claims.Claim( System.Security.Claims.ClaimTypes.Role, "Requestor")
                                  );
                            return Task.FromResult(0);
                        }
                    }
                });
        }
        

        private static string EnsureTrailingSlash(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            if (!value.EndsWith("/", StringComparison.Ordinal))
            {
                return value + "/";
            }

            return value;
        }
    }
}
