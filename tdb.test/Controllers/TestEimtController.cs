using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestEimtController : ControllerBase
    {
        /// <summary>
        /// 赋值比较
        /// </summary>
        /// <param name="times">赋值次数</param>
        [HttpGet]
        public void Comparative(int times)
        {
            Model model = new Model();
            TimeSpan ts1;
            DateTime time1 = DateTime.Now;

            for (int i = 0; i < times; i++)
            {
                CommHelper.EmitSet<Model>(model, "Name", "测试1");
            }
            DateTime time2 = DateTime.Now;

            ts1 = time2 - time1;
            Console.WriteLine("emit:" + ts1.TotalMilliseconds);

            DateTime t3 = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                CommHelper.ReflectSet(model, "Name", "测试2");
            }
            DateTime t4 = DateTime.Now;
            ts1 = t4 - t3;
            Console.WriteLine("reflect" + ts1.TotalMilliseconds);

            DateTime t5 = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                model.Name = "测试3";
            }
            DateTime t6 = DateTime.Now;
            ts1 = t6 - t5;
            Console.WriteLine("直接赋值" + ts1.TotalMilliseconds);
        }

        /// <summary>
        /// emit方式赋值取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public string EmitSetGetObject(string name)
        {
            var model = new Model();
            CommHelper.EmitSet(model, "Name", name);

            return CommHelper.EmitGet(model, "Name") as string;
        }

        /// <summary>
        /// emit方式赋值取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public string EmitSetGetGenericType(string name)
        {
            var model = new Model();
            CommHelper.EmitSet(model, "Name", name);

            return CommHelper.EmitGet<Model, string>(model, "Name");
        }

        /// <summary>
        /// 方式方式赋值取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public string ReflectGetSet(string name)
        {
            var model = new Model();
            CommHelper.ReflectSet(model, "Name", name);

            return CommHelper.ReflectGet(model, "Name") as string;
        }

        class Model
        {
            public string Name { get; set; }
        }
    }
}
