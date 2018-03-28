using FluentNHibernate.Mapping;
using DM.Models.Setup;

namespace DM.Mappings.Setup
{
    class BranchMap : ClassMap<Branch>
    {
        public BranchMap()
        {
            Not.LazyLoad();
            Table("_Branch");
            Id(m => m.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='BranchId'");
            Map(m => m.Name);
            Map(m => m.Code);
            Map(m => m.EntryDate);
            Map(m => m.Status);
            Map(m => m.ExtraData).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            References(m => m.Company, "CompanyId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            //References(m => m.User, "UserId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.UserId);
            References(m => m.Entity, "PIN").ForeignKey("PIN");

            Version(m => m.Version);
        }
    }
}