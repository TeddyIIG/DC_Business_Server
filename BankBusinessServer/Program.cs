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
            ServiceHost serviceobj; //creates a service object
            NetTcpBinding tcpbinding = new NetTcpBinding(); //creates a new tcpbinding object and initializes it


            serviceobj = new ServiceHost(typeof(BusinessUserImplementation)); //assigns the service host object with the implementation class
            serviceobj.AddServiceEndpoint(typeof(IBankBusinessInterface), tcpbinding, "net.tcp://127.0.0.1:5050/BankBusinessServer"); //sets the end point and binds the interface that the service implements for distributed communication
            serviceobj.Open(); //opens the connection

            Console.WriteLine("Business Server is now online to handle clients");
            Console.ReadLine();
            serviceobj.Close(); //closes connections



        }

        
    }
}
