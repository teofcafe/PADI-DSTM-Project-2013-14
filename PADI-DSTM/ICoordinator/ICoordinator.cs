using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace CoordinatorLibrary
{
    public interface ICoordinator
    {
        bool PrepareTransaction(Transaction transaction);
        bool BeginTransaction(Transaction transaction);
        PadInt CreatePadInt(int uid, Transaction transaction);
        PadInt AccessPadInt(int uid, Transaction transaction);
        bool CommitTransaction(Transaction transaction);
        bool AbortTransaction(Transaction transaction);
    }
}
