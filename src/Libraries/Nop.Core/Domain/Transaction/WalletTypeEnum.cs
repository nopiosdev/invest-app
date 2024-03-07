using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Transaction
{
    public enum WalletTypeEnum
    {
        BankTransfer = 10,
        CryptoWallet = 20,
        DigitalWallet = 30,
        //MailedCheck = 40,
    }
}
