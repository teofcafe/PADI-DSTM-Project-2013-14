using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Coordinator;
using PADI_DSTM;
using ServerLibrary;
using System.Collections.Concurrent;
using System.Threading;

namespace Server
{
    public class PadInt : MarshalByRefObject, IPadInt
    {
        private int value = 0;
        private int acessCounter = 0;
        private int id;
        private static int extremeAcessed = 500; //test value

        public static Callback callbackServer;

        public interface Callback
        {
            void RemovePadInt(PadInt padint);
            void DangerAcess(PadInt padint);
            bool IsFreezed();
            bool IsFailed();
            string GetUrl();
        }

        TimeStamp lastSuccessfulCommit;
        TimeStamp lastSuccessfulRead;
        TimeStamp lastSuccessfulWrite;
        Boolean preparedForCommit = false;

        public enum NextStateEnum { TEMPORARY, DELETE, MIGRATE, NONE };
        private NextStateEnum nextState = NextStateEnum.TEMPORARY;

        private ConcurrentDictionary<TimeStamp, int> tries = new ConcurrentDictionary<TimeStamp, int>();

        public int Value
        {
            get { return this.value; }
        }
        public ConcurrentDictionary<TimeStamp, int> Tries
        {
            get { return this.tries; }
        }

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

        public PadInt(int id, TimeStamp timestamp)
        {
            this.id = id;
            tries[timestamp] = this.value;
        }

        public int Read()
        {
            acessCounter++;
            if (acessCounter > extremeAcessed)
                callbackServer.DangerAcess(this);
            return this.value;
        }


        public int Read(TimeStamp timestamp)
        {
            while (callbackServer.IsFreezed())
                Thread.Sleep(1000);

            if (callbackServer.IsFailed())
                throw new TxFailedException("The server " + callbackServer.GetUrl() + " is down!");

            if (timestamp < this.lastSuccessfulWrite)
                throw new TxReadException("The TimeStamp " + timestamp.ToString() + " is too old!");

            if (!this.tries.ContainsKey(timestamp))
                throw new TxAccessException("There was no access or creation of the PadIn!");
            this.lastSuccessfulRead = timestamp;
            return this.tries[timestamp];

        }

        public void Write(int value, TimeStamp timestamp)
        {
            while (callbackServer.IsFreezed())
                Thread.Sleep(1000);

            if (callbackServer.IsFailed())
                throw new TxFailedException("The server " + callbackServer.GetUrl() + " is down!");

            if (timestamp < this.lastSuccessfulRead || timestamp < this.lastSuccessfulWrite)
                throw new TxWriteException("The TimeStamp " + timestamp.ToString() + " is too old!");

            if (!this.tries.ContainsKey(timestamp))
                throw new TxAccessException("There was no access or creation of the PadIn!");
            this.lastSuccessfulWrite = timestamp;
            this.tries[timestamp] = value;
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
            while (callbackServer.IsFreezed())
                Thread.Sleep(1000);

            if (callbackServer.IsFailed())
                throw new TxFailedException("The server " + callbackServer.GetUrl() + " is down!");

            if (timestamp < this.lastSuccessfulRead || timestamp < this.lastSuccessfulWrite)
                throw new TxWriteException("The TimeStamp " + timestamp.ToString() + " is too old!");

            bool prepared = true;

            lock (this)
            {
                if (this.preparedForCommit) prepared = false;
                else this.preparedForCommit = true;
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

            if (!prepared)
                return false;

            if (this.NextState == NextStateEnum.TEMPORARY)
                this.NextState = NextStateEnum.NONE;

            int value;
            if (!this.tries.TryRemove(timeStamp, out value))
            {
                lock (this) { this.preparedForCommit = false; }
                return false;
            }

            this.Write(value);

            lock (this) { this.preparedForCommit = false; }

            return true;
        }


        public void Abort(TimeStamp timeStamp)
        {
            if (this.NextState == NextStateEnum.TEMPORARY && this.tries.Count == 1)
            {
                callbackServer.RemovePadInt(this);
                return;
            }

            int value;
            if (!this.tries.TryRemove(timeStamp, out value))
                throw new TxAccessException("Couldn´t abort transaction with the timestamp " + timeStamp.Timestamp + " because it doesn´t exist!");
        }
    }
}
