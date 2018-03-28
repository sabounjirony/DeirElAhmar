using DM.Models.Setup;
using FluentNHibernate.Mapping;

namespace DM.Mappings.Setup
{
    class ArDictMap : ClassMap<ArDict>
    {
        public ArDictMap()
        {
            Not.LazyLoad();
            Table("_ArDict");
            Id(m => m.Token).GeneratedBy.Assigned();
            Map(m => m.Replacement);
            Map(m => m.Occurance);
            Map(m => m.Hit);
            Map(m => m.Trash).CustomType("StringClob").CustomSqlType("nvarchar(max)"); ;
            Version(m => m.Version);
        }
    }
}