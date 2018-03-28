using FluentNHibernate.Mapping;
using DM.Models.Setup;

namespace DM.Mappings.Setup
{
    class CompanyMap : ClassMap<Company>
    {
        public CompanyMap()
        {
            Not.LazyLoad();
            Table("_Company");
            Id(m => m.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='CompanyId'");
            Map(m => m.Name);
            Map(m => m.EntryDate);
            Map(m => m.ExtraData).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            //References(m => m.User, "UserId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.UserId);
            References(m => m.Entity, "PIN").ForeignKey("PIN");

            Version(m => m.Version);
        }
    }
}