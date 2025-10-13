using System;
using System.Net.Mail;
using System.Threading.Tasks;
using AccessManagerWeb.Core.Models;
using AccessManagerWeb.Core.Interfaces;

namespace AccessManagerWeb.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromAddress;

        public EmailService(string smtpServer, int smtpPort, string fromAddress)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _fromAddress = fromAddress;
        }

        public async Task SendAccessRequestNotificationAsync(ResourceRequest request)
        {
            var subject = $"New Access Request: {request.ResourceName}";
            var body = $@"
                A new access request has been submitted:
                Requestor: {request.RequestorUsername}
                Resource: {request.ResourceName}
                Reason: {request.RequestReason}
                Date: {request.RequestDate}
            ";

            await SendEmailAsync(subject, body, GetApproverEmailAddress());
        }

        public async Task SendAccessGrantedNotificationAsync(ResourceRequest request)
        {
            var subject = $"Access Request Approved: {request.ResourceName}";
            var body = $@"
                Your access request has been approved:
                Resource: {request.ResourceName}
                Approved By: {request.ApprovedBy}
                Approval Date: {request.ApprovalDate}
            ";

            await SendEmailAsync(subject, body, GetUserEmailAddress(request.RequestorUsername));
        }

        public async Task SendAccessDeniedNotificationAsync(ResourceRequest request)
        {
            var subject = $"Access Request Denied: {request.ResourceName}";
            var body = $@"
                Your access request has been denied:
                Resource: {request.ResourceName}
                Status: {request.Status}
            ";

            await SendEmailAsync(subject, body, GetUserEmailAddress(request.RequestorUsername));
        }

        private async Task SendEmailAsync(string subject, string body, string toAddress)
        {
            using (var message = new MailMessage(_fromAddress, toAddress))
            {
                message.Subject = subject;
                message.Body = body;

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    await client.SendMailAsync(message);
                }
            }
        }

        private string GetApproverEmailAddress()
        {
            // В реальном приложении это может быть получено из конфигурации или базы данных
            return "approver@company.com";
        }

        private string GetUserEmailAddress(string username)
        {
            // В реальном приложении это может быть получено из Active Directory
            return $"{username}@company.com";
        }
    }
}