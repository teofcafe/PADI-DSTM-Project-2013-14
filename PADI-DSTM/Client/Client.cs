using System;
using PADI_DSTM;
using System.Collections.Generic;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            PadiDstm.Init();

            Dictionary<int, PadInt> usedPadInts = new Dictionary<int, PadInt>();

            Console.WriteLine("-------------------------");
            Console.Write("Command: ");

            string command;

            while (!(command = Console.ReadLine()).ToLower().Contains("quit"))
            {
                try
                {
                    int uid;
                    switch (command)
                    {
                        case "begin":
                            PadiDstm.TxBegin();
                            break;
                        case "create":
                            Console.Write("  UID: ");
                            if (int.TryParse(Console.ReadLine(), out uid))
                            {
                                PadInt padInt = PadiDstm.CreatePadInt(uid);
                                usedPadInts[uid] = padInt;
                                Console.WriteLine(padInt.ToString());
                            }
                            else Console.WriteLine("Invalid UID!!!!!");
                            break;
                        case "access":
                            Console.Write("  UID: ");
                            if (int.TryParse(Console.ReadLine(), out uid))
                            {
                                PadInt padInt = PadiDstm.AccessPadInt(uid);
                                usedPadInts[uid] = padInt;
                                Console.WriteLine(padInt.ToString());
                            }
                            else Console.WriteLine("Invalid UID!!!!!");
                            break;
                        case "commit":
                            Console.WriteLine(PadiDstm.TxCommit());
                            usedPadInts.Clear();
                            break;
                        case "abort":
                            Console.WriteLine(PadiDstm.TxAbort());
                            usedPadInts.Clear();
                            break;
                        case "status":
                            Console.WriteLine(PadiDstm.Status());
                            break;
                        case "write":
                            Console.Write("  UID: ");
                            if (int.TryParse(Console.ReadLine(), out uid))
                            {
                                int value;

                                Console.Write("  Value: ");

                                if (int.TryParse(Console.ReadLine(), out value))
                                {
                                    usedPadInts[uid].Write(value);
                                }
                                else Console.WriteLine("Invalid Value!!!!!");
                            }
                            else Console.WriteLine("Invalid UID!!!!!");
                            break;
                        case "read":
                            Console.Write("  UID: ");
                            if (int.TryParse(Console.ReadLine(), out uid))
                            {
                                try
                                {
                                    Console.WriteLine(usedPadInts[uid].Read());
                                } catch(Exception) {
                                    Console.WriteLine("Not Accessed or Created");
                                }
                                
                            }
                            else Console.WriteLine("Invalid UID!!!!!");
                            break;
                        case "freeze":
                            Console.Write("  URL: ");
                            Console.WriteLine(PadiDstm.Freeze(Console.ReadLine()));
                            break;
                        case "recover":
                            Console.Write("  URL: ");
                            Console.WriteLine(PadiDstm.Recover(Console.ReadLine()));
                            break;
                        case "fail":
                            Console.Write("  URL: ");
                            Console.WriteLine(PadiDstm.Fail(Console.ReadLine()));
                            break;
                    }
                    Console.WriteLine("-------------------------");
                    Console.Write("Command: ");
                }
                catch (TxException e)
                {
                    Console.WriteLine("Exception: " + e.GetType() + " || Message: " + e.Message);
                    Console.WriteLine("-------------------------");
                    Console.Write("Command: ");
                }
            }
        }
    }
}
