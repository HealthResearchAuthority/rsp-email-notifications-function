namespace Rsp.NotifyFunction.Models
{
    public class EmailNotificationMessage
    {
        public string EventType { get; set; } = null!;
        public string EventName { get; set; } = null!;
        public string EmailTemplateId { get; set; } = null!;

        /// <summary>
        /// List of recipient email addresses
        /// </summary>
        public string RecipientAdress { get; set; } = null!;

        /// <summary>
        /// Personalisation data for any placeholder fields in the email template
        /// </summary>
        public Dictionary<string, dynamic> Data { get; set; } = new Dictionary<string, dynamic>();
    }
}