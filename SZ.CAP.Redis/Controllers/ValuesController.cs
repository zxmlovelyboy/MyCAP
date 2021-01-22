using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SZ.CAP.Redis.Services;

namespace SZ.CAP.Redis.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IRedisClientCore _redisClient;

        public ValuesController(IRedisClientCore redisClient)
        {
            _redisClient = redisClient;
        }

        [HttpPost]
        [Route("Send")]
        public async Task<string> SendVerifyCode(string userCode)
        {
            //create random verify code 
            var dataKey = "Simon" + userCode; // userCode = 110
            var rdCode = new Random().Next(1000, 9999);

            await _redisClient.SetAsync(dataKey, rdCode, 300);

            //send short message
            //业务逻辑处理

            return await Task.FromResult("返回验证码：" + rdCode);
        }

        [HttpGet]
        [Route("Verify")]
        public async Task<string> VerifyCode(string userCode, string verifyCode)
        {
            var dataKey = "Simon" + userCode;
            var resultCode = await _redisClient.GetAsync(dataKey);

            return $"验证返回结果：{verifyCode == resultCode}";
        }
    }
}