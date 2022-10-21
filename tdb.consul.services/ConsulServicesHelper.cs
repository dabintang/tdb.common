using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tdb.consul.services
{
    /// <summary>
    /// consul服务帮助类
    /// </summary>
    public class ConsulServicesHelper
    {
        /// <summary>
        /// 把服务注册到Consul
        /// </summary>
        /// <param name="consulIP">consul服务IP</param>
        /// <param name="consulPort">consul服务端口</param>
        /// <param name="serviceIP">服务IP</param>
        /// <param name="servicePort">服务端口</param>
        /// <param name="serviceName">服务在consul上的服务名</param>
        /// <param name="healthCheckApiUrl">心跳检查地址</param>
        /// <param name="deregisterCriticalServiceAfter">心跳检测失败多久后注销服务</param>
        /// <param name="interval">间隔多久心跳检测一次</param>
        /// <param name="timeout">心跳检测超时时间</param>
        /// <param name="appLifetime">程序生命周期</param>
        public static void RegisterToConsul(
            string consulIP,
            int consulPort,
            string serviceIP,
            int servicePort,
            string serviceName,
            string healthCheckApiUrl,
            TimeSpan? deregisterCriticalServiceAfter,
            TimeSpan? interval,
            TimeSpan? timeout,
            IHostApplicationLifetime? appLifetime)
        {
            //Consul地址
            var consulClient = new ConsulClient(p => { p.Address = new Uri($"http://{consulIP}:{consulPort}"); });

            //心跳检测设置
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = deregisterCriticalServiceAfter, //心跳检测失败多久后注销
                Interval = interval, //间隔多久心跳检测一次
                HTTP = healthCheckApiUrl, //心跳检查地址，本服务提供的地址
                Timeout = timeout  //心跳检测超时时间
            };

            //注册信息
            var registration = new AgentServiceRegistration()
            {
                ID = $"{serviceIP}:{servicePort}", //服务ID，唯一
                Name = serviceName, //服务名（如果服务搭集群，它们的服务名应该是一样的，但是ID不一样）
                Address = $"{serviceIP}", //服务地址
                Port = servicePort, //服务端口
                Tags = Array.Empty<string>(), //服务标签，一般可以用来设置权重等本地服务特有信息
                Checks = new[] { httpCheck }, //心跳检测设置
            };

            //向Consul注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();

            //关闭程序后注销到Consul
            if (appLifetime != null)
            {
                appLifetime.ApplicationStopped.Register(() =>
                {
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
            }
        }

        /// <summary>
        /// 获取已注册到consul的本服务地址
        /// </summary>
        /// <param name="consulIP">consul服务IP</param>
        /// <param name="consulPort">consul服务端口</param>
        /// <param name="serviceName">服务在consul上的服务名</param>
        /// <returns></returns>
        public static CatalogService[] FindServices(string consulIP, int consulPort, string serviceName)
        {
            using var consul = new ConsulClient(c =>
            {
                c.Address = new Uri($"http://{consulIP}:{consulPort}"); //Consul地址
            });
            //获取服务
            var services = consul.Catalog.Service(serviceName).Result.Response;
            return services;
        }
    }
}
