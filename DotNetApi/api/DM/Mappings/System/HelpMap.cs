using FluentNHibernate.Mapping;
using DM.Models.System;

namespace DM.Mappings.System
{
    public class HelpMap : ClassMap<Help>
    {
        public HelpMap()
        {
            Not.LazyLoad();
            Table("_Help");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='HelpId'");
            Map(m => m.ParentId);
            Map(m => m.Page);
            Map(m => m.Ctrl);
            Map(m => m.Title);
            Map(m => m.Text).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(m => m.EntryDate);
            Map(m => m.DisplayOrder);
            //References(m => m.User, "UserId").NotFound.Ignore().Cascade.None();
            Map(m => m.UserId);
            Version(m => m.Version);
        }
    }
}
