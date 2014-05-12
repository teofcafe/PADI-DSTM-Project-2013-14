using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM;

namespace ServerLibrary
{
    public interface IServer
    {
        bool VerifyMigration(int uid);
        bool VerifyOverCharge();
        void Migrate(IPadInt uid);
        bool ReceiveSpecialPadInt(SerializablePadInt padInt);
        IPadInt CreatePadInt(int uid, TimeStamp timeStamp);
        IPadInt CreateReplicatedPadInt(int uid, TimeStamp timestamp);
        IPadInt AccessPadInt(int uid, TimeStamp timeStamp);
        IPadInt ReplicatedAccessPadInt(int uid, TimeStamp timeStamp);
        bool Freeze();
        bool Fail();

        bool Recover();
        bool Status();
    }
}
