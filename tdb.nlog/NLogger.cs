﻿using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.nlog
{
    /// <summary>
    /// Nlog日志帮助类
    /// Trace 包含大量的信息，例如 protocol payloads。一般仅在开发环境中启用, 仅输出不存文件。
    /// Debug  比 Trance 级别稍微粗略，一般仅在开发环境中启用, 仅输出不存文件。
    /// Info 一般在生产环境中启用。
    /// Warn 一般用于可恢复或临时性错误的非关键问题。
    /// Error 一般是异常信息。
    /// Fatal - 非常严重的错误！
    /// </summary>
    public class NLogger
    {
        #region 单例

        ///// <summary>
        ///// 构造函数
        ///// </summary>
        ///// <param name="logger"></param>
        //private NLogger(NLog.Logger logger)
        //{
        //    this.logger = logger;
        //}

        //private NLog.Logger logger = null;
        //private static object _lock = new object();
        //private static NLogger _ins = null;
        ///// <summary>
        ///// 单例
        ///// </summary>
        //public static NLogger Ins
        //{
        //    get
        //    {
        //        if (_ins == null)
        //        {
        //            lock (_lock)
        //            {
        //                if (_ins == null)
        //                {
        //                    _ins = new NLogger(NLog.LogManager.GetCurrentClassLogger());
        //                }
        //            }
        //        }

        //        return _ins;
        //    }
        //}

        #endregion

        protected NLog.Logger logger = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public NLogger()
        {
            this.logger = NLog.LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// 痕迹日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public virtual void Trace(string msg)
        {
            logger.Trace(msg);
        }

        /// <summary>
        /// 痕迹日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public virtual void Trace(Exception ex, string msg)
        {
            logger.Trace(ex, msg);
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public virtual void Debug(string msg)
        {
            logger.Debug(msg);
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public virtual void Debug(Exception ex, string msg)
        {
            logger.Debug(ex, msg);
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public virtual void Info(string msg)
        {
            logger.Info(msg);
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public virtual void Info(Exception ex, string msg)
        {
            logger.Info(ex, msg);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public virtual void Warn(string msg)
        {
            logger.Warn(msg);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public virtual void Warn(Exception ex, string msg)
        {
            logger.Warn(ex, msg);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public virtual void Error(string msg)
        {
            logger.Error(msg);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public virtual void Error(Exception ex, string msg)
        {
            logger.Error(ex, msg);
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public virtual void Fatal(string msg)
        {
            logger.Fatal(msg);
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="msg">日志内容</param>
        public virtual void Fatal(Exception ex, string msg)
        {
            logger.Fatal(ex, msg);
        }
    }
}