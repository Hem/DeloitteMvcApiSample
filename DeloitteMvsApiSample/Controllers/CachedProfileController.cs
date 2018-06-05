using DeloitteMvcApiSample.Helpers;
using DeloitteMvcApiSample.Models;
using System.Security.Claims;
using System.Web.Http;

namespace DeloitteMvcApiSample.Controllers
{
    public class CachedProfileController : ApiController
    {
        // GET: api/CachedProfile
        public UserProfile Get()
        {
            var userToken = CacheKeyGenerator.GetUserCacheToken(ClaimsPrincipal.Current);

            var cachedUser = CacheHelper.Get<UserProfile>(userToken);

            return cachedUser;
        }

    }
}
