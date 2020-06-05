using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using BankBusinessInterface;
using System.Threading;

namespace BankBusinessServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost serviceobj;
            NetTcpBinding tcpbinding = new NetTcpBinding();


            serviceobj = new ServiceHost(typeof(BusinessUserImplementation));
            serviceobj.AddServiceEndpoint(typeof(IBankBusinessInterface), tcpbinding, "net.tcp://127.0.0.1:5050/BankBusinessServer");
            serviceobj.Open();

            Console.WriteLine("Business Server is now online to handle clients");
            Console.ReadLine();
            serviceobj.Close();

            

        }

        
    }
}
