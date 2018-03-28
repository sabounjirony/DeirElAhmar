using FluentNHibernate.Mapping;
using DM.Models.System;

namespace DM.Mappings.System
{
    internal class DocumentMap : ClassMap<Document>
    {
        public DocumentMap()
        {
            Not.LazyLoad();
            Table("_Document");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='DocumentId'");
            Map(m => m.Path);
            Map(m => m.Name);
            Map(m => m.Type);
            Map(m => m.Reference);
            Map(m => m.EntryDate);
            //References(m => m.User, "UserId").NotFound.Ignore().Cascade.None();
            Map(m => m.UserId);
            Version(m => m.Version);
        }
    }
}