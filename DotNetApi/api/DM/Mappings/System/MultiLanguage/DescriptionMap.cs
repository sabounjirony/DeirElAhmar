using FluentNHibernate.Mapping;
using DM.Models.System.MultiLanguage;

namespace DM.Mappings.System.MultiLanguage
{
    class DescriptionMap : ClassMap<Description>
    {
        public DescriptionMap()
        {
            Not.LazyLoad();
            Table("_Description");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='DescriptionId'");
            Map(m => m.LanguageId);
            Map(m => m.Parent);
            Map(m => m.Code);
            Map(m => m.Text);
            Version(m => m.Version);
        }
    }
}