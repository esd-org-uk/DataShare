namespace DS.Domain.Interface
{
    public interface IEmailService
    {
        string SendEmail(string recipientemail, string subject, string body, string returnemail);
    }
}