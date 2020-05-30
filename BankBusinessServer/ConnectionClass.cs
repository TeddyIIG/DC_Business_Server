using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ServerUserInterface;

namespace BankBusinessServer
{
    class ConnectionClass
    {
        public UserInterface ConnectedInterface()
        {

            ChannelFactory<UserInterface> UserFactory;
            UserInterface Iuser;
            Console.WriteLine("Attempting Connection to Server.... ");
            NetTcpBinding tcpBinding = new NetTcpBinding();
            string connection = "net.tcp://localhost:8004/BankServer";

            UserFactory = new ChannelFactory<UserInterface>(tcpBinding, connection);

            Iuser = UserFactory.CreateChannel();




            return Iuser;


        }
    }
}
