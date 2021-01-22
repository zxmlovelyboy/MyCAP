using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SZ.CAP.Redis.Services
{
    public interface IRedisClientCore
    {
        string Get(string key);
        void Set(string key, object t, int expiresSec = 0);
        T Get<T>(string key) where T : new();
        Task<string> GetAsync(string key);
        Task SetAsync(string key, object t, int expiresSec = 0);
        Task<T> GetAsync<T>(string key) where T : new();
    }
}
