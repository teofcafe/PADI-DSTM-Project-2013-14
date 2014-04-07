﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace CoordinatorLibrary
{
    public class CoordinatorConnector
    {
        public static ICoordinator GetCoordinatorOfTransaction(Transaction transaction)
        {
            return (ICoordinator)Activator.GetObject(typeof(ICoordinator), transaction.CoordinatorURL);
        }
    }
}
