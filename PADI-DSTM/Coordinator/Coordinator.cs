using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction;

namespace Coordinator
{
    public class Coordinator : ICoordinator
    {
        public bool BeginTransaction(Transaction.Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool PrepareTransaction(Transaction.Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool CommitTransaction(Transaction.Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool AbortTransaction(Transaction.Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
