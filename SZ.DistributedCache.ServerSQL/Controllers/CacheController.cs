using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace SZ.DistributedCache.ServerSQL.Controllers
{
    public class CacheController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public CacheController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public IActionResult Index()
        {
            string nowTime = _distributedCache.GetString("t1");
            if (string.IsNullOrEmpty(nowTime))
            {
                nowTime = DateTime.Now.ToString();

                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
                //1.相对过期时间
                //options.SlidingExpiration = TimeSpan.FromSeconds(10);

                //2. 绝对过期时间(两种形式)
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(100);
                //options.AbsoluteExpiration= new DateTimeOffset(DateTime.Parse("2019-07-16 16:33:10"));

                _distributedCache.SetString("t1", nowTime, options);
            }
            ViewBag.t1 = nowTime;

            return View();
        }
    }
}
