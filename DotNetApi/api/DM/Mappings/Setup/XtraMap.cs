using FluentNHibernate.Mapping;
using DM.Models.Setup;

namespace DM.Mappings.Setup
{
    class XtraMap : ClassMap<Xtra>
    {
        public XtraMap()
        {
            Not.LazyLoad();
            Table("_Xtra");
            CompositeId().KeyProperty(p => p.Object)
                 .KeyProperty(p => p.Id)
                 .KeyProperty(p => p.Property);
            Map(m => m.Value).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Version(m => m.Version);
        }
    }
}