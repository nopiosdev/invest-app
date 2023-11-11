using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Customers
{
    public partial class IdentityVerificationBuilder : NopEntityBuilder<IdentityVerification>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(IdentityVerification), nameof(IdentityVerification.FormId)))
                    .AsInt32().ForeignKey<Download>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(IdentityVerification), nameof(IdentityVerification.ProofOfAddress)))
                    .AsInt32().ForeignKey<Download>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(IdentityVerification), nameof(IdentityVerification.Document)))
                    .AsInt32().ForeignKey<Download>();
        }
    }
}
