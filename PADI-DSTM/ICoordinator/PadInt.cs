using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace CoordinatorLibrary
{
    public interface PadInt
    {

        int Read();

        void Write(int value);
    }
}
