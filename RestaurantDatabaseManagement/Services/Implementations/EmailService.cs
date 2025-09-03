using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using RestaurantDatabaseManagement.Data;
using System.Net;
using System.Net.Mail;

namespace RestaurantDatabaseManagement.Services.Implementations
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _ctx;

        public EmailService(IConfiguration configuration, ApplicationDbContext ctx)
        {
            _configuration = configuration;
            _ctx = ctx;
        }

        private async Task<MemoryStream?> ExportPaymentsToExcelAsync()
        {
            var result = await (from p in _ctx.Payments
                                join o in _ctx.Orders on p.order_id equals o.order_id
                                where p.payment_status == 0 && p.transaction_id == null
                                select new
                                {
                                    PaymentId = p.payment_id,
                                    OrderId = o.order_id,
                                    CustomerEmail = o.customer_email
                                }).ToListAsync();

            if (result.Count == 0)
                return null;

            var memoryStream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("PendingPayments");

                // Header row
                worksheet.Cell(1, 1).Value = "Payment ID";
                worksheet.Cell(1, 2).Value = "Order ID";
                worksheet.Cell(1, 3).Value = "Customer Email";

                // Data rows
                for (int i = 0; i < result.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = result[i].PaymentId;
                    worksheet.Cell(i + 2, 2).Value = result[i].OrderId;
                    worksheet.Cell(i + 2, 3).Value = result[i].CustomerEmail;
                }

                workbook.SaveAs(memoryStream);
            }

            memoryStream.Position = 0; // reset to beginning
            return memoryStream;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = _configuration.GetValue<string>("Credentials:email");
            var pass = _configuration.GetValue<string>("Credentials:password");

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(mail, pass),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(mail),
                Subject = subject,
                Body = message,
                IsBodyHtml = false
            };

            mailMessage.To.Add(email);

            // Get Excel as stream
            var excelStream = await ExportPaymentsToExcelAsync();
            if (excelStream != null)
            {
                var attachment = new Attachment(excelStream, "PendingPayments.xlsx",
                                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                mailMessage.Attachments.Add(attachment);
                await client.SendMailAsync(mailMessage);
            }     
        }
    }
}
