using FluentNHibernate.Mapping;
using DM.Models.System.Security;

namespace DM.Mappings.System.Security
{
    class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Not.LazyLoad();
            Table("_User");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='UserId'");
            Map(m => m.UserName);
            Map(m => m.Password);
            Map(m => m.LanguageId);
            Map(m => m.PasswordHistory);
            Map(m => m.LastPasswordUpdate);
            Map(m => m.MustChangePassword);
            Map(m => m.PageSize);
            Map(m => m.IsBlocked);
            Map(m => m.EnteringUserId, "UserId");
            Map(m => m.EntryDate);

            References(m => m.Entity, "Pin").ForeignKey("Pin").NotFound.Ignore().Cascade.None();
            //References(m => m.Branch, "BranchId").ForeignKey("Id").NotFound.Ignore().Cascade.None();
            Map(m => m.BranchId);

            HasManyToMany(m => m.Roles)
                .Table("_UserRole")
                .ParentKeyColumn("UserId")
                .ChildKeyColumn("RoleId").Not.LazyLoad().Fetch.Join().Cascade.All();

            Version(m => m.Version);
        }
    }
}
