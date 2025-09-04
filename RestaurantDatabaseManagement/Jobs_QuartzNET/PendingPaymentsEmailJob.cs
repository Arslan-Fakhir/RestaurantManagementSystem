using ClosedXML.Excel;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Quartz;
using RestaurantDatabaseManagement.Data;
using RestaurantDatabaseManagement.Services.Implementations;
using System.Net;
using System.Net.Mail;

namespace RestaurantDatabaseManagement.Jobs
{
    // Job to send email reminders for pending payments
    public class PendingPaymentsEmailJob :IJob
    {
        private readonly EmailService _service;
        public PendingPaymentsEmailJob(EmailService Service)
        {
            _service = Service;
        }
        // Execute the job to send email reminders
        public async Task Execute(IJobExecutionContext context)
        {   
            var receiver = "arslanfakhir16@gmail.com";
            var subject = "Pending Payment Reminder - Report";
            var message = "This is a reminder for your pending payment. Please complete the payment at your earliest convenience.";

            await _service.SendEmailAsync(receiver,subject,message); // send email
        }
        
    }
}
