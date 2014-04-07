using System;
using PADI_DSTM;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            PadiDstm.Init();

            PadiDstm.TxBegin();

            PadInt padint = PadiDstm.CreatePadInt(10);
            Console.WriteLine(padint.ToString());
            PadInt padintAccessed = PadiDstm.AccessPadInt(10);

            Console.ReadLine();
        }
    }
}
