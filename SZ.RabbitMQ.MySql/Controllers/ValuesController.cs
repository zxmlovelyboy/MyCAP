using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace SZ.RabbitMQ.MySql.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ICapPublisher _capBus;

        public ValuesController(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        [Route("~/without/transaction")]
        //[Route("without")]
        public async Task<IActionResult> WithoutTransaction()
        {
            await _capBus.PublishAsync("SZ.RabbitMQ.MySql", DateTime.Now);

            return Ok("普通发布成功。" + DateTime.Now.ToString("F"));
        }

        [Route("~/dapper/transaction")]
        public IActionResult AdonetWithTransaction()
        {
            using (var connection = new MySqlConnection(AppDbContext.ConnectionString))
            {
                using (var transaction = connection.BeginTransaction(_capBus, true))
                {
                    //your business code
                    connection.Execute("insert into test(name) values('test')", transaction: (IDbTransaction)transaction.DbTransaction);
                    //for (int i = 0; i < 5; i++)
                    //{
                    _capBus.Publish("SZ.RabbitMQ.MySql", DateTime.Now);
                    //}
                }
            }
            return Ok("dapper执行插入数据到数据表。" + DateTime.Now.ToString("F"));
        }

        [Route("~/ef/transaction")]
        //[Route("ef")]
        public IActionResult EntityFrameworkWithTransaction([FromServices]AppDbContext dbContext)
        {
            using (var trans = dbContext.Database.BeginTransaction(_capBus, autoCommit: false))
            {
                dbContext.Persons.Add(new Person() { Name = "ef.transaction" });

                for (int i = 0; i < 1; i++)
                {
                    _capBus.Publish("SZ.RabbitMQ.MySql", DateTime.Now);
                }

                dbContext.SaveChanges();

                trans.Commit();
            }
            return Ok("EF Code First 新增数据到Persons " + DateTime.Now.ToString("F"));
        }

        [NonAction]
        [CapSubscribe("SZ.RabbitMQ.MySql")]
        public void Subscriber(DateTime p)
        {
            Console.WriteLine($@"{DateTime.Now} Subscriber invoked, Info: {p}");
        }

        [NonAction]
        [CapSubscribe("SZ.RabbitMQ.MySql", Group = "group.test2")]
        public void Subscriber2(DateTime p, [FromCap]CapHeader header)
        {
            Console.WriteLine($@"{DateTime.Now} Subscriber invoked, Info: {p}");
        }
    }
}
