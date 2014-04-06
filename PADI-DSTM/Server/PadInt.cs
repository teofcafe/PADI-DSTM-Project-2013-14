using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Coordinator;
using TransactionLibrary;
using ServerLibrary;

namespace Server
{
    public class PadInt : MarshalByRefObject, IPadInt
    {
        private int value;
        private int acessCounter = 0;
        private int id;
        private static int extremeAcessed = 500; //test value
        public delegate void DangerAcess(PadInt padint);
        public static DangerAcess dangerAcess;

        TimeStamp lastSuccessfulCommit;

        public enum NextStateEnum {DELETE, MIGRATE, NONE};

        private NextStateEnum nextState = NextStateEnum.NONE;

        private Hashtable trys = new Hashtable();

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        public PadInt(int id)
        {
            this.id = id;
        }

        public int Read()
        {
            acessCounter++;
            if (acessCounter > extremeAcessed)
                dangerAcess(this);
            return this.value;
        }

        public int Read(TimeStamp timestamp)
        {
            acessCounter++;
            return (int)trys[timestamp];
        }

        public void Write(int value, TimeStamp timestamp)
        {
            acessCounter++;
            trys[timestamp] = value;
        }

        public void Write(int value)
        {
            acessCounter++;
            this.value = value;
        }

        public NextStateEnum NextState
        {
            get { return this.nextState; }
            set { this.nextState = value; }
        }


        public bool PrepareCommit(TimeStamp timestamp)
        {
            throw new NotImplementedException();
        }

        public bool Commit(TimeStamp timestamp)
        {
            throw new NotImplementedException();
        }
    }
}
