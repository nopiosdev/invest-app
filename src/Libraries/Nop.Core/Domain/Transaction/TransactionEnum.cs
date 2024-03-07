using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Transaction
{
    public enum TransactionType
    {
        Debit = 10,
        Credit = 20,
        Voided = 30,
    }
    public enum Status
    {
        Completed = 100,
        Pending = 200,
        Removed = 300,
        Declined = 400,
    }
}
