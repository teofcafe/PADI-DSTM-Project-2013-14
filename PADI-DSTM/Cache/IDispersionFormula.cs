using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispersionLibrary
{
    public interface IDispersionFormula
    {
        int GetIdOfServerWithObjectWithId(int uid);

        int GetIdOfReplicaServerWithObjectWithId(int uid);
    }
}
