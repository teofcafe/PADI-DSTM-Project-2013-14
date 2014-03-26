using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using System.Collections;

namespace Server
{
    public class PadInt
    {
        private int value;
        private int acessCounter = 0;

        TimeStamp lastSuccessfulCommit;

        public enum NextStateEnum {DELETE, MIGRATE, NONE};

        private NextStateEnum nextState = NextStateEnum.NONE;

        private Hashtable trys = new Hashtable();

        public PadInt(int value)
        {
            this.value = value;
        }

        public int Read()
        {
            acessCounter++;
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
    }
}
