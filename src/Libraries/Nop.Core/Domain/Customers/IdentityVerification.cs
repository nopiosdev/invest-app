using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public partial class IdentityVerification : BaseEntity
    {
        public int FormId { get; set; }

        public int ProofOfAddress { get; set; }

        public int Document { get; set; }
    }
}
