using DispersionLibrary;
using MasterLibrary;
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
        private static IDispersionFormula dispersionFormula = null;
        private static Hashtable serversCache = null;
        private const string ServerEndPoint = "Server";

        static ServerConnector()
        {
            IMaster master = MasterConnector.GetMaster();
            ServerConnector.dispersionFormula = master.GetDispersionFormula();
            ServerConnector.serversCache = new Hashtable();
        }

        public static IServer GetServerResponsibleForObjectWithId(int uid)
        {
            //TODO: Return the replica if the primary doesnt respond
            int serverId = dispersionFormula.GetIdOfServerWithObjectWithId(uid);

            return ServerConnector.GetServerWithId(serverId);
        }

        public static IServer GetMigrationDestinationServerForObjectWithId(int uid) {
            //TODO
            return null;
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
    }
}
