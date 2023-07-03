﻿using ETicaretAPI.Application.Abstraction.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class MailService : IMailService
    {
        readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new ();  
            mail.IsBodyHtml = isBodyHtml;
            mail.To.Add(to);
            mail.Subject = subject; 
            mail.Body = body;
            mail.From = new(_configuration["Mail:Username"], "EVLİ E-Ticaret",System.Text.Encoding.UTF8);

            SmtpClient smtp = new ();
            smtp.Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]);
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Host = "smtp.gmail.com";


            await smtp.SendMailAsync(mail);
        }
    }
}
