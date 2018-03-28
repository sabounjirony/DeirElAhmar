using FluentNHibernate.Mapping;
using DM.Models.System.Security;

namespace DM.Mappings.System.Security
{
    class PermissionMap : ClassMap<Permission>
    {
        public PermissionMap()
        {
            Not.LazyLoad();
            Table("_Permission");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='PermissionId'");
            Map(m => m.Code);
            Map(m => m.EntryDate);
            Map(m => m.Status);

            References(m => m.Module, "ModuleId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            //References(m => m.User, "UserId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.UserId);

            HasManyToMany(m => m.Roles)
                .Table("_RolePermission")
                .ParentKeyColumn("PermissionId")
                .ChildKeyColumn("RoleId").Fetch.Select().Not.LazyLoad().Cascade.None();

            Version(m => m.Version);
        }
    }
}