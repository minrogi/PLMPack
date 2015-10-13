#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Mail;
using SendGrid;
#endregion

namespace TestSendGrid
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create the email object first, then add the properties.
                var sgMessage = new SendGridMessage();
                sgMessage.AddTo("fgasnier71@gmail.com");
                sgMessage.From = new MailAddress("fgasnier@treedim.com", "fgasnier");
                sgMessage.Subject = "Testing the SendGrid Library";
                sgMessage.Text = "Hello World!";
                SendAsync(sgMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        private static async void SendAsync(SendGridMessage message)
        {
            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential("azure_0eab1456a36f779016c55fb98b816b54@azure.com", "BVci6TbM65jE6WG");

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            try
            {
                await transportWeb.DeliverAsync(message);
                Console.WriteLine("Sent!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
