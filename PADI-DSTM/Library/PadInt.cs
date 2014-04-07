using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    public class PadInt : CoordinatorLibrary.PadInt
    {
        private CoordinatorLibrary.PadInt virtualPadInt = null;


        public PadInt(CoordinatorLibrary.PadInt virtualPadInt)
        {
            this.virtualPadInt = virtualPadInt;
        }

        public int Read()
        {
            return this.virtualPadInt.Read();
        }

        public void Write(int value)
        {
            this.virtualPadInt.Write(value);
        }
    }
}
