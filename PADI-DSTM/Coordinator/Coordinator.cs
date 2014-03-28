using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Coordinator
{
    public class Coordinator : ICoordinator
    {
        public bool BeginTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool PrepareTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool CommitTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool AbortTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }


        public PadInt CreatePadInt(int uid)
        {
            throw new NotImplementedException();
        }

        public PadInt AccessPadInt(int uid)
        {
            throw new NotImplementedException();
        }
    }
}
