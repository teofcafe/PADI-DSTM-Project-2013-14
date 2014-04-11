using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM;

namespace CoordinatorLibrary
{
    public class CoordinatorConnector
    {
        public delegate bool RemoteTransactionAsyncDelegate(Transaction transaction);
        public delegate PadInt RemotePadIntAsyncDelegate(int uid, Transaction transaction);
        public delegate bool RemoteAsyncDelegate();
        public static ICoordinator GetCoordinatorOfTransaction(Transaction transaction)
        {
            return (ICoordinator)Activator.GetObject(typeof(ICoordinator), transaction.CoordinatorURL);
        }

        public static ICoordinator GetCoordinatorByUrl(string URL)
        {
            return (ICoordinator)Activator.GetObject(typeof(ICoordinator), URL);
        }
    }
}
