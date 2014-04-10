﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using PADI_DSTM;
using CoordinatorLibrary;
using MasterLibrary;

namespace PADI_DSTM
{
    public static class PadiDstm
    {
        private static Transaction transaction;
        private static string coordinatorURL;
        private static ICoordinator coordinator;

        public static bool Init()
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);

            return true;
        }

        public static bool TxBegin()
        {
            IMaster master = MasterConnector.GetMaster();
            PadiDstm.transaction = master.Connect();

            try
            {
                PadiDstm.coordinatorURL = PadiDstm.transaction.CoordinatorURL;
                PadiDstm.Coordinator = CoordinatorConnector.GetCoordinatorOfTransaction(PadiDstm.transaction);
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Master");

                return false;
            }
           
            try
            {
                PadiDstm.Coordinator.BeginTransaction(PadiDstm.transaction);

                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not locate Coordenator" + e.ToString());

                return false;
            }
        }

        public static bool TxCommit()
        {
            try
            {
                PadiDstm.Coordinator.PrepareTransaction(PadiDstm.transaction);
                PadiDstm.Coordinator.CommitTransaction(PadiDstm.transaction);
                return true;
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Commit");

                return false;
            }
        }

        public static bool TxAbort()
        {
            try
            {
                PadiDstm.Coordinator.AbortTransaction(PadiDstm.transaction);
                return true;
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate Coordenator to Abort");

                return false;
            }
        }

        public static bool Status()
        {
            return true;
        }

        public static bool Fail(string URL)
        {
            return true;
        }

        public static bool Freeze(string URL)
        {
            return true;
        }

        public static bool Recover(string URL)
        {
            return true;
        }

        public static PADI_DSTM.PadInt CreatePadInt(int uid)
        {
            return new PADI_DSTM.PadInt(PadiDstm.Coordinator.CreatePadInt(uid, PadiDstm.transaction));
        }

        public static PADI_DSTM.PadInt AccessPadInt(int uid)
        {
            return new PADI_DSTM.PadInt(PadiDstm.Coordinator.AccessPadInt(uid, PadiDstm.transaction));
        }

        private static ICoordinator Coordinator
        {
            get { return PadiDstm.coordinator; }
            set { PadiDstm.coordinator = value; }
        }
    }
}
