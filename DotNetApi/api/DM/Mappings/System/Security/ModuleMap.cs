using FluentNHibernate.Mapping;
using DM.Models.System.Security;

namespace DM.Mappings.System.Security
{
    class ModuleMap : ClassMap<Module>
    {
        public ModuleMap()
        {
            Not.LazyLoad();
            Table("_Module");
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(m => m.Description);
            Map(m => m.Path);
            Map(m => m.EntryDate);
            Map(m => m.Status);
            Map(m => m.EnableLogging);
            //References(m => m.Author, "Author").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.Author);
            //References(m => m.User, "UserId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.UserId);
            References(m => m.SystemModule, "SystemModule").ForeignKey("Id").Cascade.None().NotFound.Ignore();

            Version(m => m.Version);
        }
    }
}