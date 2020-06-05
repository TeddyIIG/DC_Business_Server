using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ServerUserInterface;
using System.Windows;

namespace BankBusinessServer
{
    class ConnectionClass
    {
        public UserInterface ConnectedInterface()
        {
            try
            {
                ChannelFactory<UserInterface> UserFactory; 
                UserInterface Iuser; //Referencing user interface from bankserver dll
                Console.WriteLine("Attempting Connection to Server.... ");
                NetTcpBinding tcpBinding = new NetTcpBinding(); //creates a new tcpbinding object and initializes it
                string connection = "net.tcp://127.0.0.1:5001/BankServer"; //The tcp endpoint which the channel factory will be configured to

                UserFactory = new ChannelFactory<UserInterface>(tcpBinding, connection); //Initializes the channel factory, configured to the defined tcp endpoint

                Iuser = UserFactory.CreateChannel(); //Create a communication channel

                return Iuser;
            }
            catch (EndpointNotFoundException ex)
            {
                Console.WriteLine("The endpoint has not been configured correctly");
                return null;
            }
        }
    }
}
