using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction;

namespace Library
{

    /*  Transaction Connect();
        int RegisterServer(ip);
        void UnregisterServer(int id);
        notifyOvercharge(ind id);
        URL GetServerURL(int id);
        int GetServerWithObject(int uid);
        void NotifyNeedMigrate(int id, int uid);
     * 
     * */
    interface IMaster
    {
        //Transaction Connect();

        int RegisterServer(string ip);
    }
}
