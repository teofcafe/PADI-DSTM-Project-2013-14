﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace ServerLibrary
{
    public interface IServer
    {
        void VerifyCharge();
        bool VerifyMigration(int uid);
        void Migrate(int uid);
        IPadInt CreatePadInt(int uid, TimeStamp timeStamp);
        IPadInt AccessPadInt(int uid, TimeStamp timeStamp);
    }
}
