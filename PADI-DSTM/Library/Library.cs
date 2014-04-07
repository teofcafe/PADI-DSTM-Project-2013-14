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
using TransactionLibrary;
using CoordinatorLibrary;
using MasterLibrary;

namespace PADI_DSTM
{
    public static class PadiDstm
    {
        private const string masterURL = "tcp://localhost:8089/Master";
        private static Transaction transaction;
        private static string coordinatorURL;
        private static ICoordinator coordinator;

        public static bool Init()
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            return true;
        }

        public static bool TxBegin()
        {
            IMaster master = MasterConnector.GetMaster();
            PadiDstm.transaction = master.Connect();

            try
            {
                PadiDstm.coordinatorURL = PadiDstm.transaction.CoordinatorURL;
                System.Console.WriteLine("Library.Init(): " + PadiDstm.coordinatorURL);
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Master");
                return false;
            }

            System.Console.WriteLine("Library.TxBegin(): " + PadiDstm.coordinatorURL);
            PadiDstm.Coordinator = (ICoordinator)Activator.GetObject(typeof(ICoordinator), PadiDstm.coordinatorURL);
           
            try
            {
                PadiDstm.Coordinator.BeginTransaction(PadiDstm.transaction);
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not locate Coordenator" + e.ToString());
                return false;
            }
        }

        public static bool TxCommit()
        {
            try
            {
                Console.WriteLine("TOMESTAMP-> " + PadiDstm.transaction);
                PadiDstm.Coordinator.PrepareTransaction(PadiDstm.transaction);
                PadiDstm.Coordinator.CommitTransaction(PadiDstm.transaction);
                return true;
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Commit");
                return false;
            }
        }

        public static bool TxAbort()
        {
            try
            {
                PadiDstm.Coordinator.AbortTransaction(PadiDstm.transaction);
                return true;
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Abort");
                return false;
            }
        }

        public static bool Status()
        {
            return true;
        }

        public static bool Fail(string URL)
        {
            return true;
        }

        public static bool Freeze(string URL)
        {
            return true;
        }

        public static bool Recover(string URL)
        {
            return true;
        }

        public static PADI_DSTM.PadInt CreatePadInt(int uid)
        {
            System.Console.WriteLine("LIBRARY TRANSACTION: " + PadiDstm.transaction.ToString());

            try
            {
                return new PADI_DSTM.PadInt(PadiDstm.Coordinator.CreatePadInt(uid));
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Create a new PadInt");
                return null;
            }
        }

        public static PADI_DSTM.PadInt AccessPadInt(int uid)
        {
            try
            {
                return new PADI_DSTM.PadInt(PadiDstm.Coordinator.AccessPadInt(uid));
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Acess a PadInt");
                return null;
            }
        }

        private static ICoordinator Coordinator
        {
            get { return PadiDstm.coordinator; }
            set { PadiDstm.coordinator = value; }
        }
    }
}
