using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;

namespace Library
{
    public class Library
    {
        private const string masterURL = "tcp://localhost:8089/Master";
        private Transaction transaction;
        private string coordinatorURL;
        private ICoordinator coordinator;

        bool Init()
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            IMaster master = (IMaster)Activator.GetObject(typeof(IMaster), Library.masterURL);

            try
            {
                this.transaction = master.Connect();
                this.coordinatorURL = this.transaction.CoordinatorURL;
                return true;
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Master");
                return false;
            }
        }

        bool TxBegin()
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            this.Coordinator = (ICoordinator)Activator.GetObject(typeof(ICoordinator), this.coordinatorURL);

            try
            {
                this.Coordinator.BeginTransaction(this.transaction);
                return true;
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator");
                return false;
            }
        }

        bool TxCommit()
        {
            try
            {
                this.Coordinator.CommitTransaction(this.transaction);
                return true;
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Commit");
                return false;
            }
        }

        bool TxAbort()
        {
            try
            {
                this.Coordinator.AbortTransaction(this.transaction);
                return true;
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Abort");
                return false;
            }
        }

        bool Status()
        {
            return true;
        }

        bool Fail(string URL)
        {
            return true;
        }

        bool Freeze(string URL)
        {
            return true;
        }

        bool Recover(string URL)
        {
            return true;
        }

        PadInt CreatePadInt(int uid)
        {
            try
            {
                return this.Coordinator.CreatePadInt(uid);
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Create a new PadInt");
                return null;
            }
        }

        PadInt AccessPadInt(int uid)
        {
            try
            {
                return this.Coordinator.AccessPadInt(uid);
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Acess a PadInt");
                return null;
            }
        }

        private ICoordinator Coordinator
        {
            get { return this.coordinator; }
            set { this.coordinator = value; }
        }
    }
}
