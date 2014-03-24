using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using System.Collections;

namespace Server
{
    class PadInt
    {
        private int value;
        private int acessCounter = 0;

        TimeStamp lastSuccessfulCommit;

        public enum NextStateEnum {DELETE, MIGRATE, NONE};

        private NextStateEnum nextState = NextStateEnum.NONE;

        Hashtable trys = new Hashtable();

        public int Read()
        {
            acessCounter++;
            return this.value;
        }

        public int Read(TimeStamp timestamp)
        {
            acessCounter++;
            return this.trys[timestamp];
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
    }
}
