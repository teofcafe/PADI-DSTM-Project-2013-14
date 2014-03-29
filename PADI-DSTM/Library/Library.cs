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

        public bool Init()
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            IMaster master = (IMaster)Activator.GetObject(typeof(IMaster), Library.masterURL);

            try
            {
                this.transaction = master.Connect();
                this.coordinatorURL = this.transaction.CoordinatorURL;
                System.Console.WriteLine("Library.Init(): " + this.coordinatorURL);
                return true;
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Master");
                return false;
            }
        }

        public bool TxBegin()
        {

            System.Console.WriteLine("Library.TxBegin(): " + this.coordinatorURL);
            this.Coordinator = (ICoordinator)Activator.GetObject(typeof(ICoordinator), this.coordinatorURL);

            try
            {
                this.Coordinator.BeginTransaction(this.transaction);
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not locate Coordenator" + e.ToString());
                return false;
            }
        }

        public bool TxCommit()
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

        public bool TxAbort()
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

        public bool Status()
        {
            return true;
        }

        public bool Fail(string URL)
        {
            return true;
        }

        public bool Freeze(string URL)
        {
            return true;
        }

        public bool Recover(string URL)
        {
            return true;
        }

        public PadInt CreatePadInt(int uid)
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

        public PadInt AccessPadInt(int uid)
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
