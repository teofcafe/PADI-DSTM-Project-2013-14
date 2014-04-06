using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace ServerLibrary
{
    public interface IPadInt
    {

         int Read(TimeStamp timestamp);
         void Write(int value, TimeStamp timestamp);
         bool PrepareCommit(TimeStamp timestamp);
         bool Commit(TimeStamp timestamp);
    }
}
