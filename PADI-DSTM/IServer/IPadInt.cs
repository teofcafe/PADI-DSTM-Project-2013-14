using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM;

namespace ServerLibrary
{
    public interface IPadInt
    {
         int ReplicatedRead(TimeStamp timestamp);
         void ReplicatedWrite(int value, TimeStamp timestamp);
         bool ReplicatedPrepareCommit(TimeStamp timestamp);
         int Read(TimeStamp timestamp);
         void Write(int value, TimeStamp timestamp);
         bool PrepareCommit(TimeStamp timestamp);
         bool Commit(TimeStamp timestamp);
         void Abort(TimeStamp timeStamp);
         void ReplicateAbort(TimeStamp timestamp);
         void ReplicateCommit(TimeStamp timestamp);
    }
}
