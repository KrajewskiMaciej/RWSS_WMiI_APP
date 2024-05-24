using System;
using System.Net;
using System.Net.Mail;

namespace RWSS_WMiI
{
    class EmailSender
    {
        public static void SendEmail(string imie, string login, string haslo, string emailOd, string emailDo)
        {
            // Konfiguracja serwera SMTP
            var smtpClient = new SmtpClient("smtp-relay.brevo.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("74fc65001@smtp-brevo.com", "TCyHwRWKvB62fL9r"),
                EnableSsl = true,
            };

            string htmlBody = "<html>" +
                  "<body>" +
                  "<h1 style='font-size: 25px;'>Witaj, " + imie + "! Przesyłam dane do logowania w aplikacji RWSS WMiI APP</h1>" +
                  "<p style='font-size: 18px;'></p>" +
                  "<p style='font-size: 16px;'>Login: </p>" + login +
                  "<p style='font-size: 16px;'></p>" +
                  "<p style='font-size: 16px;'>Hasło: </p>" + haslo +
                  "<p style='font-size: 16px;'></p>" +
                  "<p style='font-size: 16px;'></p>" +
                  "<p style='font-size: 14px;'>Wiadomość wysłana automatycznie. Porsimy na nią nie odpowiadać</p>" +
                  "</body>" +
                  "</html>";

            var message = new MailMessage
            {
                From = new MailAddress(emailOd),
                Subject = "Dane logowania RWSS WMiI APP",
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(emailDo);

            smtpClient.Send(message);

            Console.Write("Wiadomość zawierająca login i hasło, została wysłana pomyślnie.");
        }
    }
}
