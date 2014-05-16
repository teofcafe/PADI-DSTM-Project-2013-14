using DispersionLibrary;
using MasterLibrary;
using PADI_DSTM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public static class ServerConnector
    {
        public delegate void RemoteVerifyChargeAsyncDelegate();
        public delegate bool RemoteVerifyMigrationAsyncDelegate(int uid);
        public delegate void RemoteMigrateAsyncDelegate(int uid);
        public delegate IPadInt RemoteCreatePadIntAsyncDelegate(int uid, TimeStamp timeStamp);
        public delegate IPadInt RemoteAccessPadIntAsyncDelegate(int uid, TimeStamp timeStamp);
        public delegate bool RemoteAsyncDelegate();

        private static IDispersionFormula dispersionFormula = null;
        private static Hashtable serversCache = null;
        public const string ServerEndPoint = "Server";

        static ServerConnector()
        {
            IMaster master = MasterConnector.GetMaster();
            ServerConnector.dispersionFormula = master.GetDispersionFormula();
            ServerConnector.serversCache = new Hashtable();
        }

        public static IServer GetServerWithObjectWithId(int uid)
        {
            int serverId = dispersionFormula.GetIdOfServerWithObjectWithId(uid);
            IServer serverWithObject = ServerConnector.GetServerWithId(serverId);

            if (!serverWithObject.HasPadIntWithId(uid))
            {
                Console.WriteLine("PadInt doesnt exist in Normal Server");
                try
                {
                    IMaster master = MasterConnector.GetMaster();
                    Console.WriteLine("Trying to find PadInt in Migration Server");
                    serverWithObject = ServerConnector.GetServerWithURL(master.GetServerOfMigratedPadInt(uid) + "/Server");
                }
                catch (Exception)
                {
                    Console.WriteLine("Trying to find in Replication Server");
                    return GetReplicationServerForObjectWithId(uid);
                }
            }

            return serverWithObject;
        }

        public static IServer GetServerResponsibleForObjectWithId(int uid)
        {
            int serverId = dispersionFormula.GetIdOfServerWithObjectWithId(uid);
            IServer serverResponsibleForObject = ServerConnector.GetServerWithId(serverId);

            return serverResponsibleForObject;
        }

        public static IServer GetMigrationDestinationServerForObjectWithId(int uid) {
            //TODO
            return null;
        }

        public static IServer GetReplicationServerForObjectWithId(int uid)
        {
            int serverId;
            try
            {
                serverId = dispersionFormula.GetIdOfReplicaServerWithObjectWithId(uid);
                return ServerConnector.GetServerWithId(serverId);
            }
            catch (Exception)
            {
                IMaster master = MasterConnector.GetMaster();
                return ServerConnector.GetServerWithURL(master.GetServerOfMigratedPadInt(uid) + "/Server");
            }  
        }

        public static IServer GetServerWithId(int serverId)
        {
            Console.WriteLine("ServerConnector.GetServerWithId() Called with " + serverId);
            string serverURL = ServerConnector.serversCache[serverId] as string;

            if (serverURL == null)
            {
                serverURL = MasterConnector.GetMaster().GetServerURL(serverId) + "/" + ServerEndPoint;
                Console.WriteLine("ServerConnector.GetServerWithId() Not in cache, contacted Master for URL " + serverURL);
                ServerConnector.serversCache[serverId] = serverURL;
            }

            return (IServer)Activator.GetObject(typeof(IServer), serverURL);
        }

        public static IServer GetServerWithURL(string URL)
        {
            return (IServer)Activator.GetObject(typeof(IServer), URL);
        }
    }
}
