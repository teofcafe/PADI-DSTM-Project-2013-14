using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator
{
    public interface IServer
    {
        void VerifyCharge();
        void VerifyMigration(int uid);
        void Migrate(int[] servers);
        int read(int uid);
    }
}
