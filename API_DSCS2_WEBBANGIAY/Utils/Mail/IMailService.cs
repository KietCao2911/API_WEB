using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY.Utils.Mail
{
    public interface IMailService
    {

        Task SendEmailAsync(MailRequest mailRequest);
    }
}
