using FluentNHibernate.Mapping;
using DM.Models.System.Security;

namespace DM.Mappings.System.Security
{
    class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Not.LazyLoad();
            Table("_Role");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='RoleId'");
            Map(m => m.Code);
            Map(m => m.FullParent).ReadOnly();
            References(m => m.ParentRole, "ParentRoleId").NotFound.Ignore().Cascade.None();
            Version(m => m.Version);
        }
    }
}