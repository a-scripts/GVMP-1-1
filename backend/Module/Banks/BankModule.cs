using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Banks
{
    public sealed class BankModule : SqlModule<BankModule, Bank, uint>
    {
        protected override string GetQuery()
        {
            return "SELECT * FROM `bank` ORDER BY id;";
        }



    
    }
}
