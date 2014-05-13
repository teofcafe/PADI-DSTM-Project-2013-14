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
using System.Runtime.Serialization;

namespace Server
{
    public class PadInt : MarshalByRefObject, IPadInt, TryPadInt.CallBack
    {
        private int value = 0;
        private int accessCounter = 0;
        private int accessPerTick = 0;

        private int id;

        private static int extremeAcessed = 3; //test value
        private static long ticksToWait = 3000;
        private long lastTicksSeen = DateTime.Now.Ticks;

        public static Callback callbackServer;

        public interface Callback
        {
            void EnqueuePadInt(PadInt padint);
            void RemovePadInt(PadInt padint);
            void DangerAcess(PadInt padint);
            bool IsFreezed();
            bool IsFailed();
            string GetUrl();
            void IncrementSystemCharge(int charge);
        }

        TimeStamp lastSuccessfulCommit;
        TimeStamp lastSuccessfulRead;
        TimeStamp lastSuccessfulWrite;
        Boolean preparedForCommit = false;

        public enum NextStateEnum { TEMPORARY, DELETE, MIGRATE, NONE };

        private NextStateEnum nextState = NextStateEnum.TEMPORARY;


        private ConcurrentDictionary<TimeStamp, TryPadInt> tries = new ConcurrentDictionary<TimeStamp, TryPadInt>();

        public int Value
        {
            get { return this.value; }
        }
        public IDictionary<TimeStamp, TryPadInt> Tries
        {
            get { return this.tries; }
        }

        public TimeStamp LastSuccessfulCommit
        {
            get { return this.lastSuccessfulCommit; }
            set { this.lastSuccessfulCommit = value; }
        }
        public bool PreparedForCommit
        {
            get { return this.preparedForCommit; }
            set { this.preparedForCommit = value; }
        }

        public TimeStamp LastSuccessfulRead
        {
            get { return this.lastSuccessfulRead; }
            set { this.lastSuccessfulRead = value; }
        }

        public TimeStamp LastSuccessfulWrite
        {
            get { return this.lastSuccessfulWrite; }
            set { this.lastSuccessfulWrite = value; }
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public PadInt(int id, TimeStamp timestamp)
        {
            this.id = id;
            tries[timestamp] = new TryPadInt(timestamp, this, this.value);
            this.lastSuccessfulWrite = timestamp;
            this.lastSuccessfulRead = timestamp;
            this.lastSuccessfulCommit = timestamp;
            
        }

        public PadInt(SerializablePadInt padInt)
        {
            this.id = padInt.id;
            this.lastSuccessfulCommit = padInt.lastSuccessfulCommit;
            this.lastSuccessfulRead = padInt.lastSuccessfulRead;
            this.lastSuccessfulWrite = padInt.lastSuccessfulWrite;
            this.preparedForCommit = padInt.preparedForCommit;
        }

        public SerializablePadInt ToSerializablePadInt()
        {
            return new SerializablePadInt(this.value, this.id, this.lastSuccessfulCommit, this.lastSuccessfulRead, this.lastSuccessfulWrite, this.preparedForCommit);
        }

        public void CreateTry(TimeStamp timeStamp)
        {
            int value = this.value;
            try
            {
                TryPadInt lastWrite = this.tries[this.lastSuccessfulWrite];
                TryPadInt newTryPadInt = new TryPadInt(timeStamp, this, lastWrite.TempValue);
                lastWrite.AddDependencie(newTryPadInt);
                this.tries[timeStamp] = newTryPadInt;
            } catch (Exception)
            {
                this.tries[timeStamp] = new TryPadInt(timeStamp, this, this.value); ;
            }
        }

        public int ReplicatedRead(TimeStamp timestamp)
        {
            int value = Read(timestamp);
            PadInt padIntReplica;

            try
            {
                IServer targetOfReplica = ServerConnector.GetReplicationServerForObjectWithId(this.id);
                padIntReplica = (PadInt) targetOfReplica.AccessPadInt(this.id, timestamp);
                padIntReplica.Read(timestamp);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                callbackServer.EnqueuePadInt(this);
            }

            return value;
        }

        public void IncrementAccessCounterBy(int number)
        {
            this.accessCounter += number;

            long actualTicks = DateTime.Now.Ticks;
            long ticksPassed = actualTicks - this.lastTicksSeen;

            if (ticksPassed > PadInt.ticksToWait)
            {
                this.accessPerTick = (int)(this.accessCounter / ticksPassed);
                this.lastTicksSeen = actualTicks;
            }

            if (this.accessPerTick > PadInt.extremeAcessed)
                callbackServer.DangerAcess(this);

            PadInt.callbackServer.IncrementSystemCharge(number);
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

            this.IncrementAccessCounterBy(2);

            this.lastSuccessfulRead = timestamp;
            return this.tries[timestamp].TempValue;
        }

        public void ReplicatedWrite(int value, TimeStamp timestamp)
        {
            this.Write(value, timestamp);
            PadInt padIntReplica;

            try
            {
                IServer targetOfReplica = ServerConnector.GetReplicationServerForObjectWithId(this.id);
                padIntReplica = (PadInt)targetOfReplica.AccessPadInt(this.id, timestamp);
                padIntReplica.Write(value, timestamp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                callbackServer.EnqueuePadInt(this);
            }
        }

        public void ReplicateCommit(TimeStamp timestamp){
            Commit(timestamp);
            PadInt padIntReplica; 
            try
            {
                IServer targetOfReplica = ServerConnector.GetReplicationServerForObjectWithId(this.id);
                Console.WriteLine("asssassas" + this.id);
                padIntReplica = (PadInt)targetOfReplica.AccessPadInt(this.id, timestamp);
                padIntReplica.Commit(timestamp);
            }
            catch (Exception e)
            {
                callbackServer.EnqueuePadInt(this);
                Console.WriteLine("tenho cacada" + e.Message.ToString());
            }
        }

     

        public void Write(int value, TimeStamp timestamp)
        {
            Console.WriteLine("Chamei o write com " + timestamp + " e valor " + value);
            while (callbackServer.IsFreezed())
                Thread.Sleep(1000);

            if (callbackServer.IsFailed())
                throw new TxFailedException("The server " + callbackServer.GetUrl() + " is down!");

            if (timestamp < this.lastSuccessfulRead || timestamp < this.lastSuccessfulWrite)
                throw new TxWriteException("The TimeStamp " + timestamp.ToString() + " is too old!");

            if (!this.tries.ContainsKey(timestamp))
                throw new TxAccessException("There was no access or creation of the PadIn!");

            this.IncrementAccessCounterBy(3);

            this.lastSuccessfulWrite = timestamp;

            this.tries[timestamp].TempValue = value;
        }

        public void Write(int value)
        {
            this.value = value;
        }

        public NextStateEnum NextState
        {
            get { return this.nextState; }
            set { this.nextState = value; }
        }

        public bool ReplicatedPrepareCommit(TimeStamp timestamp)
        {
            this.PrepareCommit(timestamp);
            PadInt padIntReplica;

            try
            {
                IServer targetOfReplica = ServerConnector.GetReplicationServerForObjectWithId(this.id);
                padIntReplica = (PadInt)targetOfReplica.AccessPadInt(this.id, timestamp);
                padIntReplica.PrepareCommit(timestamp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                callbackServer.EnqueuePadInt(this);
            }

            return this.preparedForCommit;
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
            Console.WriteLine("Recebi o commit com o timestamp de : " + timeStamp);
            bool prepared = false;

            lock (this)
            {
                prepared = this.preparedForCommit;
            }

            if (!prepared)
                return false;

            if (this.NextState == NextStateEnum.TEMPORARY)
                this.NextState = NextStateEnum.NONE;

            TryPadInt value;
            if (!this.tries.TryGetValue(timeStamp, out value))
            {
                lock (this) { this.preparedForCommit = false; }
                return false;
            }

            this.Write(value.TempValue);
            value.ActualState = TryPadInt.State.COMMITED;
            RemoveTry(timeStamp);
            

           
            lock (this) { this.preparedForCommit = false; }

            return true;
        }

        public void ReplicateAbort(TimeStamp timeStamp)
        {
            Abort(timeStamp);

            PadInt padIntReplica;
            try
            {
                IServer targetOfReplica = ServerConnector.GetReplicationServerForObjectWithId(this.id);
                padIntReplica = (PadInt)targetOfReplica.AccessPadInt(this.id, timeStamp);
                padIntReplica.Abort(timeStamp);
            }
            catch (Exception e)
            {
                callbackServer.EnqueuePadInt(this);
                Console.WriteLine("tenho cacada" + e.Message.ToString());
            }
        }
        
        public void Abort(TimeStamp timeStamp)
        {
            if (this.NextState == NextStateEnum.TEMPORARY && this.tries.Count == 1)
            {
                callbackServer.RemovePadInt(this);
                return;
            }

            TryPadInt value;
            if (!this.tries.TryGetValue(timeStamp, out value))
                throw new TxAccessException("Couldn´t abort transaction with the timestamp " + timeStamp.Timestamp + " because it doesn´t exist!");
        }

        public void RemoveTry(TimeStamp timestamp)
        {
            TryPadInt value;

            this.tries.TryRemove(lastSuccessfulCommit, out value);
        }

/*        public void UpdatePadInt(SerializablePadInt padinToUpdate)
        {
            this.lastSuccessfulCommit = padinToUpdate.lastSuccessfulCommit;
            this.lastSuccessfulRead = padinToUpdate.lastSuccessfulRead;
            this.lastSuccessfulWrite = padinToUpdate.lastSuccessfulWrite;
            this.preparedForCommit = padinToUpdate.preparedForCommit;
            //this.tries = padinToUpdate.tries;
            //this.nextState = padinToUpdate.nextState;
        }

        public void UpdateRead(PadInt padinToUpdate)
        {
            this.lastSuccessfulRead = padinToUpdate.lastSuccessfulRead;
            this.tries = padinToUpdate.tries;
        }

        public void UpdateWrite(PadInt padinToUpdate)
        {
            this.lastSuccessfulWrite = padinToUpdate.lastSuccessfulWrite;
            this.tries = padinToUpdate.tries;
        }*/

    }
}
