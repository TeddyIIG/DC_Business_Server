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
                UserInterface Iuser;
                Console.WriteLine("Attempting Connection to Server.... ");
                NetTcpBinding tcpBinding = new NetTcpBinding();
                string connection = "net.tcp://127.0.0.1:5001/BankServer";

                UserFactory = new ChannelFactory<UserInterface>(tcpBinding, connection);

                Iuser = UserFactory.CreateChannel();




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
