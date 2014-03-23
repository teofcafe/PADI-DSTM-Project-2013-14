using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator
{
    public interface ICoordinator
    {
        public bool BeginTransaction(Int32 timestamp);
        public bool PrepareTransaction(Int32 timestamp);
        public bool CommitTransaction(Int32 timestamp);
        public bool AbortTransaction(Int32 timestamp);
    }
}
