using FluentNHibernate.Mapping;
using DM.Models.Setup;

namespace DM.Mappings.Setup
{
    class AddressMap : ClassMap<Address>
    {
        public AddressMap()
        {
            Not.LazyLoad();
            Table("_Address");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='AddressId'");
            Map(m => m.Sequence);
            Map(m => m.Type);
            Map(m => m.Country);
            Map(m => m.Province);
            Map(m => m.Caza);
            Map(m => m.Village);
            Map(m => m.Region);
            Map(m => m.Street);
            Map(m => m.Building);
            Map(m => m.Floor);
            Map(m => m.Phone1);
            Map(m => m.Phone2);
            Map(m => m.Phone3);
            Map(m => m.Fax);
            Map(m => m.PostalCode);
            Map(m => m.PoBox);
            Map(m => m.AreaNum);
            Map(m => m.PlotNum);
            Map(m => m.ExtraData).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(m => m.EntryDate);
            //References(m => m.User, "UserId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.UserId);
            References(m => m.Entity, "Pin").ForeignKey("PIN").Cascade.None();
            Version(m => m.Version);
        }
    }
}