using PADI_DSTM;
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
        private LinkedList<TryPadInt> dependencies = new LinkedList<TryPadInt>();
        private int numberOfWaitingDependencies = 0;
        private TimeStamp timeStamp = null;

        public enum State {COMMITED, ABORTED, NONE};

        State actualState = State.NONE;

        public interface CallBack
        {
            void RemoveTry(TimeStamp timestamp);
        }

        CallBack callBack = null;

        public TryPadInt(TimeStamp timeStamp, CallBack callBack, int value)
        {
            this.timeStamp = timeStamp;
            this.callBack = callBack;
            this.tempValue = value;
        }

        public void waitForDependencies()
        {
            while (dependencies.Count > 0)
            {
                TryPadInt dependencie = dependencies.First();

                lock (dependencie)
                {
                    if (this.actualState == State.NONE)
                        Monitor.Wait(dependencie);
                    else if (dependencie.ActualState == State.ABORTED)
                    {
                        this.actualState = State.ABORTED;
                        Monitor.PulseAll(this);
                        break;
                    }
                }

                dependencies.RemoveFirst();
            }
        }

        public LinkedList<TryPadInt> Dependencies
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

        public State ActualState
        {
            get
            {
                if (this.numberOfWaitingDependencies-- <= 0)
                    this.callBack.RemoveTry(this.timeStamp);
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
