using FluentNHibernate.Mapping;
using DM.Models.System.Logging;

namespace DM.Mappings.System.Logging
{
    class LogErrorMap : ClassMap<LogError>
    {
        public LogErrorMap()
        {
            Not.LazyLoad();
            Table("_LogError");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='LogErrorId'");
            Map(m => m.Type);
            Map(m => m.Severity);
            Map(m => m.Source).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(m => m.Text).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(m => m.EntryDate);
            //References(m => m.Branch, "BranchId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.BranchId);
            //References(m => m.User, "UserId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.UserId);
        }
    }
}