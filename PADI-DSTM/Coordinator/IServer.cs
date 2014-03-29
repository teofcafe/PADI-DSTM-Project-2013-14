﻿using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator
{
    public interface IServer
    {
        void VerifyCharge();
        bool VerifyMigration(int uid);
        void Migrate(int uid);
        int Read(int uid);
        void Write(int uid, int value);


    }
}
