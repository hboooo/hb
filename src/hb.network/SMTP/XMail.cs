using System;
using System.Net.Mail;

namespace hb.network.SMTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/24 1:09:05
    /// description:
    /// </summary>
    public class XMail
    {
        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 发送端账号   
        /// </summary>
        public string SendAccount { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayAccountName { get; set; }

        /// <summary>
        /// 发送端密码
        /// </summary>
        public string SendPassword { get; set; }

        /// <summary>
        /// 发送目的地址
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// 发送抄送地址
        /// </summary>
        public string CC { get; set; }

        /// <summary>
        /// 发送主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 发送内容，邮件正文
        /// </summary>
        public string Body { get; set; }

        public void Send()
        {
            SmtpClient client = new SmtpClient();
            //指定电子邮件发送方式  
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = Host;
            client.UseDefaultCredentials = true;
            //用户名、密码
            client.Credentials = new System.Net.NetworkCredential(SendAccount, SendPassword);

            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.From = new MailAddress(SendAccount, DisplayAccountName);
            msg.To.Add(To);
            msg.CC.Add(CC);

            //邮件标题   
            msg.Subject = Subject;
            //邮件内容   
            msg.Body = Body;
            //邮件内容编码  
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            //是否是HTML邮件   
            msg.IsBodyHtml = true;
            //邮件优先级 
            msg.Priority = MailPriority.High;

            try
            {
                client.Send(msg);
                Console.WriteLine("send success");
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
