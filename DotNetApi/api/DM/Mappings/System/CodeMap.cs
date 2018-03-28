using FluentNHibernate.Mapping;
using DM.Models.System;

namespace DM.Mappings.System
{
    internal class CodeMap : ClassMap<Code>
    {
        public CodeMap()
        {
            Not.LazyLoad();
            Table("_Code");
            CompositeId().KeyProperty(m => m.TableName)
                .KeyProperty(m => m.CodeName, "Code");
            Map(m => m.Status);
            Map(m => m.Value1);
            Map(m => m.Value2);
            Map(m => m.Value3);
            Map(m => m.Value4);
            Map(m => m.Value5);
            Map(m => m.Value6);
            Map(m => m.IsProtected);
            Map(m => m.DisplayOrder);
            Map(m => m.RelCode);

            Version(m => m.Version);
        }
    }
}