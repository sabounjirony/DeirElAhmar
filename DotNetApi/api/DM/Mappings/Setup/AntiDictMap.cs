using FluentNHibernate.Mapping;
using DM.Models.Setup;

namespace DM.Mappings.Setup
{
    class AntiDictMap : ClassMap<AntiDict>
    {
        public AntiDictMap()
        {
            Not.LazyLoad();
            Table("_AntiDict");
            Id(m => m.Token).GeneratedBy.Assigned();
            Map(m => m.Replacement);
            Map(m => m.Occurance);
            Version(m => m.Version);
        }
    }
}