using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace SZ.DistributedCache.ServerSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;

        public ValuesController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpPost]
        public async Task<string> GetNow(string cacheId)
        {
            string nowTime = _distributedCache.GetString(cacheId);
            if (string.IsNullOrEmpty(nowTime))
            {
                nowTime = DateTime.Now.ToString();

                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(100)
                };
                //1.相对过期时间
                //options.SlidingExpiration = TimeSpan.FromSeconds(10);

                //2. 绝对过期时间(两种形式)
                //options.AbsoluteExpiration= new DateTimeOffset(DateTime.Parse("2019-07-16 16:33:10"));

                await _distributedCache.SetStringAsync(cacheId, nowTime, options);
            }

            return await Task.FromResult(nowTime);
        }
    }
}
