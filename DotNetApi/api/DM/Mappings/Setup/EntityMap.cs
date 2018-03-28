using FluentNHibernate.Mapping;
using DM.Models.Setup;

namespace DM.Mappings.Setup
{
    class EntityMap : ClassMap<Entity>
    {
        public EntityMap()
        {
            Not.LazyLoad();
            Table("_Entity");
            Id(x => x.Pin).Column("Pin").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='Pin'");
            Map(m => m.Title);
            Map(m => m.Gender);
            Map(m => m.FirstName);
            Map(m => m.FatherName);
            Map(m => m.FamilyName);
            Map(m => m.MotherName);
            Map(m => m.MaidenName);
            Map(m => m.ArFirstName);
            Map(m => m.ArFatherName);
            Map(m => m.ArFamilyName);
            Map(m => m.ArMotherName);
            Map(m => m.ArMaidenName);
            Map(m => m.Dob);
            Map(m => m.Pob);
            Map(m => m.Marital);
            Map(m => m.Language);
            Map(m => m.Mobile);
            Map(m => m.Email);
            Map(m => m.WebAddress);
            Map(m => m.IdType);
            Map(m => m.IdNum);
            Map(m => m.RegNum);
            Map(m => m.NameIndex);
            Map(m => m.Nationality);
            Map(m => m.Education);
            Map(m => m.EntryDate);
            Map(m => m.Status);
            Map(m => m.UserId);
            Map(m => m.BranchId);
            Map(m => m.Notes).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(m => m.ExtraData).CustomType("StringClob").CustomSqlType("nvarchar(max)");

            //HasMany(m => m.Addresses).Inverse().KeyColumn("PIN");
            Version(m => m.Version);
        }
    }
}