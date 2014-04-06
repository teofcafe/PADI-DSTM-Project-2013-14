using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace ServerLibrary
{
    public interface IServer
    {
        void VerifyCharge();
        bool VerifyMigration(int uid);
        void Migrate(int uid);
        int Read(int uid, TimeStamp timestamp);
        void Write(int uid, TimeStamp timestamp, int value);
        IPadInt CreatePadInt(int uid, TimeStamp timestamp);

        IPadInt CreatePadInt(int uid);
    }
}
