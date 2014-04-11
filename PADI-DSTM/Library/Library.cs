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
using PADI_DSTM;
using CoordinatorLibrary;
using MasterLibrary;
using ServerLibrary;

namespace PADI_DSTM
{
    public static class PadiDstm
    {
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
                PadiDstm.Coordinator = CoordinatorConnector.GetCoordinatorOfTransaction(PadiDstm.transaction);
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Master");

                return false;
            }

            return CallTransactionAsynchronous(new CoordinatorConnector.RemoteTransactionAsyncDelegate(PadiDstm.Coordinator.BeginTransaction));
        }

        public static bool TxCommit()
        {
            CallTransactionAsynchronous(new CoordinatorConnector.RemoteTransactionAsyncDelegate(PadiDstm.Coordinator.PrepareTransaction));
            return CallTransactionAsynchronous(new CoordinatorConnector.RemoteTransactionAsyncDelegate(PadiDstm.Coordinator.CommitTransaction));
        }

        public static bool TxAbort()
        {
            return CallTransactionAsynchronous(new CoordinatorConnector.RemoteTransactionAsyncDelegate(PadiDstm.Coordinator.AbortTransaction));
        }

        public static bool Status()
        {
            return CallAsynchronous(new MasterConnector.RemoteAsyncDelegate(MasterConnector.GetMaster().Status));
        }

        public static bool Fail(string URL)
        {
            return CallAsynchronous(new ServerConnector.RemoteAsyncDelegate(ServerConnector.GetServerWithURL(URL).Fail));
        }

        public static bool Freeze(string URL)
        {
            return CallAsynchronous(new ServerConnector.RemoteAsyncDelegate(ServerConnector.GetServerWithURL(URL).Freeze));
        }

        public static bool Recover(string URL)
        {
            return CallAsynchronous(new ServerConnector.RemoteAsyncDelegate(ServerConnector.GetServerWithURL(URL).Recover));
        }

        public static PADI_DSTM.PadInt CreatePadInt(int uid)
        {
            return new PADI_DSTM.PadInt(PadiDstm.Coordinator.CreatePadInt(uid, PadiDstm.transaction));
        }

        public static PADI_DSTM.PadInt AccessPadInt(int uid)
        {
            return new PADI_DSTM.PadInt(PadiDstm.Coordinator.AccessPadInt(uid, PadiDstm.transaction));
        }

        private static ICoordinator Coordinator
        {
            get { return PadiDstm.coordinator; }
            set { PadiDstm.coordinator = value; }
        }

        private static bool CallTransactionAsynchronous(CoordinatorConnector.RemoteTransactionAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(transaction, null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            return remoteFunction.EndInvoke(RemAr);
        }

        private static CoordinatorLibrary.PadInt CallTransactionAsynchronous(int uid, CoordinatorConnector.RemotePadIntAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(uid, transaction, null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            return remoteFunction.EndInvoke(RemAr);
        }

        private static bool CallAsynchronous(ServerConnector.RemoteAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            return remoteFunction.EndInvoke(RemAr);
        }

        private static bool CallAsynchronous(MasterConnector.RemoteAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            return remoteFunction.EndInvoke(RemAr);
        }
    }
}
