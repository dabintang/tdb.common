using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Test();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void Test()
        {
            var b1 = CheckHelper.IsNullableType(typeof(int));
            var b2 = CheckHelper.IsNullableType(typeof(int?));
            var b3 = CheckHelper.IsNullableType(typeof(string));
            var b4 = CheckHelper.IsNullableType(typeof(IIInterface));
            var b5 = CheckHelper.IsNullableType(typeof(enumEEE));
            var b6 = CheckHelper.IsNullableType(typeof(enumEEE?));
        }

        public interface IIInterface
        { }

        public enum enumEEE
        {
            aaa
        }
    }
}
