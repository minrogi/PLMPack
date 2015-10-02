#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace TestPLMPackService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter username and password:");
            string usrName = Console.ReadLine();
            string usrPwd = Console.ReadLine();

            if (string.IsNullOrEmpty(usrName.Trim()) && string.IsNullOrEmpty(usrPwd))
            {
                Console.WriteLine("Please provide the credentails");
            }
            else
            {
                try
                {
                    PLMPack.PLMPackServiceClient client = new PLMPack.PLMPackServiceClient();
                    client.ClientCredentials.UserName.UserName = usrName;
                    client.ClientCredentials.UserName.Password = usrPwd;
                    Console.WriteLine(client.UserDescription());
                    client.Close();
                }
                catch (System.ServiceModel.Security.MessageSecurityException ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
