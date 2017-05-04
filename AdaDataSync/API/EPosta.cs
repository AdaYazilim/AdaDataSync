using System.Net.Mail;

namespace AdaDataSync.API
{
    public class EPosta
    {
        public static void Gonder(string subject, string icerik)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("datasync.adayazilim@gmail.com");
            mail.To.Add("serdar.adayazilim@gmail.com");
            mail.Subject = subject;
            mail.Body = icerik;
            //Attachment attachment = new Attachment(@"F:\Masaüstü\2017AktuerlikSinavKilavuzu.pdf");
            //mail.Attachments.Add(attachment);

            smtpServer.Port = 587;
            //smtpServer.Port = 465;
            smtpServer.Credentials = new System.Net.NetworkCredential("datasync.adayazilim", "nBfgPSvNiQAhXiv5Irn8");
            smtpServer.EnableSsl = true;

            smtpServer.Send(mail);
        }


        //public static void Gonder(string subject, string icerik)
        //{
        //    //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

        //    MailMessage mail = new MailMessage();
        //    SmtpClient smtpServer = new SmtpClient("mail.prestijsigorta.com.tr");

        //    mail.From = new MailAddress("filiz.kazanci@prestijsigorta.com.tr");
        //    mail.To.Add("serdar.adayazilim@gmail.com");
        //    mail.Subject = subject;
        //    mail.Body = icerik;
        //    //Attachment attachment = new Attachment(@"F:\Masaüstü\2017AktuerlikSinavKilavuzu.pdf");
        //    //mail.Attachments.Add(attachment);

        //    //smtpServer.Port = 587;
        //    //smtpServer.Port = 465;
        //    smtpServer.Credentials = new System.Net.NetworkCredential("filiz.kazanci@prestijsigorta.com.tr", "Naz62denizz");
        //    //smtpServer.EnableSsl = true;

        //    smtpServer.Send(mail);
        //}
    }
}