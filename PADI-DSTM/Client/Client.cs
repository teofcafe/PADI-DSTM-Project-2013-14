using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Library;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            // allocate and register channel
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            // get reference to remote service
            IMaster master = (IMaster)Activator.GetObject(typeof(IMaster), "tcp://localhost:8089/Master");

            try
            {
                Library.Transaction transaction = (Library.Transaction)master.Connect();
                Console.WriteLine(transaction.ToString());
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate server");
            }
            Console.ReadLine();
        }
    }
}
