using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Coordinator
{
    public interface ICoordinator
    {
        bool BeginTransaction(Transaction transaction);
        bool PrepareTransaction(Transaction transaction);
        bool CommitTransaction(Transaction transaction);
        bool AbortTransaction(Transaction transaction);
    }
}
