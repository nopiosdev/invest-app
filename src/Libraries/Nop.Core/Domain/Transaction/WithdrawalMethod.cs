using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Nop.Core.Domain.Transaction
{
    public partial class WithdrawalMethod : BaseEntity
    {
        public int TypeId { get; set; }
        public WalletTypeEnum Type
        {
            get => (WalletTypeEnum)this.TypeId;
            set { this.TypeId = (int)value; }
        }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsRequested { get; set; }
    }
}
