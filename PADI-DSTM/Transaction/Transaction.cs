using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction
{
    public class Transaction
    {
        private Int32 timestamp;
        private string ipCoordinator;

        public Transaction(Int32 timestamp, string ipCoordinator)
        {
            this.timestamp = timestamp;
            this.ipCoordinator = ipCoordinator;
        }
    }
}
