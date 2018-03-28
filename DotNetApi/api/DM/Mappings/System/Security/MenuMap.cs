using FluentNHibernate.Mapping;
using DM.Models.System.Security;

namespace DM.Mappings.System.Security
{
    class MenuMap : ClassMap<Menu>
    {
        public MenuMap()
        {
            Not.LazyLoad();
            Table("_Menu");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='MenuId'");
            Map(m => m.DescriptionCode);
            Map(m => m.Details);
            Map(m => m.DisplayOrder);
            Map(m => m.Status);

            References(m => m.Parent, "ParentId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            References(m => m.Module, "ModuleId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            //References(m => m.Branch, "BranchId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.BranchId);

            Version(m => m.Version);
        }
    }
}