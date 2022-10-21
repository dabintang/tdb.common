using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace tdb.common
{
    /// <summary>
    /// email帮助类
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="fromEmail">发件人邮箱</param>
        /// <param name="fromEmailName">发件人名称</param>
        /// <param name="smtpHost">SMTP服务器地址</param>
        /// <param name="smtpPort">SMTP服务器端口</param>
        /// <param name="smtpPwd">SMTP密码</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="isBodyHtml">内容是否html</param>
        /// <param name="to">收件人地址</param>
        /// <param name="attachments">附件完整文件名</param>
        public static void Send(
            string fromEmail, 
            string fromEmailName,
            string smtpHost,
            int smtpPort,
            string smtpPwd,
            string subject, 
            string body, 
            bool isBodyHtml, 
            string to, 
            params string[] attachments)
        {
            //抄送地址
            var lstTo = new List<string>() { to };
            //发送电子邮件
            Send(fromEmail, fromEmailName, smtpHost, smtpPort, smtpPwd, subject, body, isBodyHtml, lstTo, null, attachments);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="fromEmail">发件人邮箱</param>
        /// <param name="fromEmailName">发件人名称</param>
        /// <param name="smtpHost">SMTP服务器地址</param>
        /// <param name="smtpPort">SMTP服务器端口</param>
        /// <param name="smtpPwd">SMTP密码</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="isBodyHtml">内容是否html</param>
        /// <param name="lstTo">收件人地址</param>
        /// <param name="lstCC">抄送地址</param>
        /// <param name="attachments">附件完整文件名</param>
        public static void Send(
            string fromEmail, 
            string fromEmailName,
            string smtpHost, 
            int smtpPort, 
            string smtpPwd,
            string subject, 
            string body, 
            bool isBodyHtml, 
            List<string> lstTo, 
            List<string>? lstCC, 
            params string[] attachments)
        {
            //邮件发送类 
            var mail = new MailMessage();
            //是谁发送的邮件 
            mail.From = new MailAddress(fromEmail, fromEmailName);

            //收件人地址               
            foreach (var to in lstTo)
            {
                mail.To.Add(to);
            }

            //抄送地址
            if (lstCC != null)
            {
                foreach (var cc in lstCC)
                {
                    mail.CC.Add(cc);
                }
            }

            //标题 
            mail.Subject = subject;
            //内容编码 
            mail.BodyEncoding = Encoding.Default;
            //发送优先级 
            mail.Priority = MailPriority.Normal;
            //邮件内容 
            mail.Body = body;
            //是否HTML形式发送 
            mail.IsBodyHtml = isBodyHtml;
            //附件 
            foreach (var attachment in attachments)
            {
                mail.Attachments.Add(new Attachment(attachment));
            }

            var smtp = GetSmtpClient(fromEmail, smtpHost, smtpPort, smtpPwd);
            smtp.Send(mail);
        }

        /// <summary>
        /// SMTP
        /// </summary>
        /// <param name="fromEmail">发件人邮箱</param>
        /// <param name="smtpHost">SMTP服务器地址</param>
        /// <param name="smtpPort">SMTP服务器端口</param>
        /// <param name="smtpPwd">SMTP密码</param>
        /// <returns>SMTP</returns>
        private static SmtpClient GetSmtpClient(string fromEmail, string smtpHost, int smtpPort, string smtpPwd)
        {
            //邮件服务器和端口 
            var smtp = new SmtpClient(smtpHost, smtpPort);
            //smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            //指定发送方式 
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //指定登录名和密码 
            smtp.Credentials = new System.Net.NetworkCredential(fromEmail, smtpPwd);
            //超时时间 
            smtp.Timeout = 30000;

            return smtp;
        }
    }
}
