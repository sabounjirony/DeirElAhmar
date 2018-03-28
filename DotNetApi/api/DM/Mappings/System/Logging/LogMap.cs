using FluentNHibernate.Mapping;
using DM.Models.System.Logging;

namespace DM.Mappings.System.Logging
{
    class LogMap : ClassMap<Log>
    {
        public LogMap()
        {
            Not.LazyLoad();
            Table("_Log");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='LogId'");
            Map(m => m.Action);
            Map(m => m.Text).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(m => m.EntryDate);
            References(m => m.Module, "ModuleId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            //References(m => m.Branch, "BranchId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.BranchId);
            //References(m => m.User, "UserId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.UserId);
        }
    }
}