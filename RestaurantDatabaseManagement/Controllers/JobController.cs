using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantDatabaseManagement.Services.Implementations;

namespace RestaurantDatabaseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly EmailService _emailService;
        public JobController(EmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpPost]
        [Route("PendingPaymentsReminder")]
        public ActionResult PendingPaymentsReminder() //recurring job
        {
            //var receiver = "mmuaz9193lgu@gmail.com";
            var receiver = "arslanfakhir16@gmail.com";
            var subject = "Pending Payment Reminder - Report";
            var message = "This is a reminder for your pending payment. Please complete the payment at your earliest convenience.";

            RecurringJob.AddOrUpdate<EmailService>("Recurring job for pending payments", x => _emailService.SendEmailAsync(receiver,subject,message),Cron.Hourly);

            return Ok("Pending payments job created successfully!");
        }
    }
}
