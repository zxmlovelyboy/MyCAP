using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace SZ.DistributedCache.Redis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisCacheController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;

        public RedisCacheController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<string> Get(long userId)
        {
            var cacheKey = "IDG_" + userId;
            var data = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(data))
            {
                return data; //returned from Cache
            }

            string str = $"Hello UserId={userId},Welcome to redis distribute cache." + DateTime.Now.ToString("F");
            await _distributedCache.SetStringAsync(cacheKey, str);
            return str;
        } 
    }
}