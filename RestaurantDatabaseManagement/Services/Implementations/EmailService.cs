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


        public async Task<string> ExportPaymentsToExcel()
        {
            var result = await (from p in _ctx.Payments
                                join o in _ctx.Orders on p.order_id equals o.order_id
                                where p.payment_status == 0 && p.transaction_id==null
                                select new
                                {
                                    PaymentId = p.payment_id,
                                    OrderId = o.order_id,
                                    CustomerEmail = o.customer_email
                                }).ToListAsync();

            if (result.Count != 0)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "PendingPayments.xlsx");

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("PendingPayments");

                    worksheet.Cell(1, 1).Value = "Payment ID";
                    worksheet.Cell(1, 2).Value = "Order ID";
                    worksheet.Cell(1, 3).Value = "Customer Email";

                    for (int i = 0; i < result.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = result[i].PaymentId;
                        worksheet.Cell(i + 2, 2).Value = result[i].OrderId;
                        worksheet.Cell(i + 2, 3).Value = result[i].CustomerEmail;
                    }

                    workbook.SaveAs(filePath);
                }

                return filePath;
            }

            return "No record found!";
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
                IsBodyHtml = false // set true if sending HTML email
            };

            mailMessage.To.Add(email);

            // Attach Excel file
            var attachmentPath = await ExportPaymentsToExcel();
            if (!attachmentPath.Contains("No record found!") && File.Exists(attachmentPath))
            {
                var attachment = new Attachment(attachmentPath);
                mailMessage.Attachments.Add(attachment);
            }

            await client.SendMailAsync(mailMessage);
        }
    }
}
