using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace BankBusinessServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost serviceobj;
            NetTcpBinding tcpbinding = new NetTcpBinding();


            serviceobj = new ServiceHost(typeof(BusinessUserImplementation));
            serviceobj.AddServiceEndpoint(typeof(BusinessUserInterface), tcpbinding, "net.tcp://localhost:8005/BankBusinessServer");
            serviceobj.Open();

            Console.WriteLine("Business Server is now online to handle clients");
            Console.ReadLine();
            serviceobj.Close();
        }

        
    }
}
