using PADI_DSTM;
using ServerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class TryPadInt : MarshalByRefObject
    {
        private int tempValue = 0;
        private LinkedList<TimeStamp> dependencies = new LinkedList<TimeStamp>();
        private int numberOfWaitingDependencies = 0;
        private TimeStamp timeStamp = null;
        public PadInt rootPadInt;

        public enum State {COMMITED, ABORTED, NONE};

        State actualState = State.NONE;

        public interface CallBack
        {
            void RemoveTry(TimeStamp timestamp);
        }

        CallBack callBack = null;

        public TryPadInt(SerializableTryPadInt padInt, PadInt rootPadint)
        {
            this.tempValue = padInt.tempValue;
            this.timeStamp = padInt.timeStamp;
            this.dependencies = new LinkedList<TimeStamp>(padInt.dependencies);
            this.rootPadInt = rootPadint;

        }

        public TryPadInt(TimeStamp timeStamp, CallBack callBack, int value, PadInt rootPadint)
        {
            this.timeStamp = timeStamp;
            this.callBack = callBack;
            this.tempValue = value;
            this.rootPadInt = rootPadint;
        }

        public SerializableTryPadInt ToSerializableTryPadInt()
        {
            return new SerializableTryPadInt(this.timeStamp, this.tempValue, this.numberOfWaitingDependencies, this.dependencies);
        }

        public void waitForDependencies()
        {
            while (dependencies.Count > 0)
            {
                TimeStamp dependencie = dependencies.First();

                lock (dependencie)
                {
                    if (this.actualState == State.NONE)
                        Monitor.Wait(dependencie);
                    else if ( rootPadInt.GetTryPadIntWithTimeStamp(dependencie).ActualState == State.ABORTED)
                    {
                        this.actualState = State.ABORTED;
                        Monitor.PulseAll(this);
                        break;
                    }
                }

                dependencies.RemoveFirst();
            }
        }

        public void AddDependencie (TryPadInt padint){
            this.numberOfWaitingDependencies++;
            this.dependencies.AddLast(padint.timeStamp);
        }

        public LinkedList<TimeStamp> Dependencies
        {
            get
            {
                this.numberOfWaitingDependencies++;
                return this.dependencies;
            }
        }

        public int TempValue
        {
            get
            {
                return this.tempValue;
            }

            set
            {
                this.tempValue = value;
            }
        }

        public CallBack Callback
        {
            get
            {
                return this.callBack;
            }

            set
            {
                this.callBack = value;
            }
        }

        public State ActualState
        {
            get
            {
                if (!(this.numberOfWaitingDependencies > 0))
                {
                    this.callBack.RemoveTry(this.timeStamp);
                    numberOfWaitingDependencies--;
                }
                return this.actualState;
            }

            set
            {
                lock (this)
                {
                    if (this.actualState == State.ABORTED)
                        return;
                    this.actualState = value;
                    Monitor.PulseAll(this);
                }
            }
        }
    }
}
