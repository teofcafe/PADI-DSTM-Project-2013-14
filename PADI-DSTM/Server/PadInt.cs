using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Coordinator;
using TransactionLibrary;
using ServerLibrary;
using System.Collections.Concurrent;

namespace Server
{
    public class PadInt : MarshalByRefObject, IPadInt
    {
        private int value = 0;
        private int acessCounter = 0;
        private int id;
        private static int extremeAcessed = 500; //test value

        public delegate void DangerAcess(PadInt padint);
        public static DangerAcess dangerAcess;

        TimeStamp lastSuccessfulCommit;
        Boolean preparedForCommit = false;

        public enum NextStateEnum {TEMPORARY, DELETE, MIGRATE, NONE};
        private NextStateEnum nextState = NextStateEnum.NONE;

        private ConcurrentDictionary<TimeStamp, int> trys = new ConcurrentDictionary<TimeStamp, int>();

        public TimeStamp LastSuccessfulCommit
        {
            get { return this.lastSuccessfulCommit; }
            set { this.lastSuccessfulCommit = value; }
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
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
            try
            {
                return trys[timestamp];
            }
            catch (Exception)
            {
                trys[timestamp] = this.value;
                return trys[timestamp];
            }
            
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
            bool prepared = true;

            lock (this) {
                if (this.preparedForCommit)
                    prepared = false;
                else
                    this.preparedForCommit = true;
            }

            return prepared;
        }

        public bool Commit(TimeStamp timeStamp)
        {
            bool prepared = false;

            lock (this)
            {
                prepared = this.preparedForCommit;
            }

            if (!prepared) return false;
            if (this.NextState == NextStateEnum.TEMPORARY)
                this.NextState = NextStateEnum.NONE;
            else
            {
                int valueToWrite = this.trys[timeStamp];
                this.Write(valueToWrite);
            }

            lock (this)
            {
                this.preparedForCommit = false;
            }

            return true;
        }
    }
}
