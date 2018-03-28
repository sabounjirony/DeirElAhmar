using FluentNHibernate.Mapping;
using DM.Models.System;

namespace DM.Mappings.System
{
    internal class CommentMap : ClassMap<Comment>
    {
        public CommentMap()
        {
            Not.LazyLoad();
            Table("_Comment");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='CommentId'");
            Map(m => m.Reference);
            Map(m => m.Text).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(m => m.EntryDate);
            //References(m => m.User, "UserId").NotFound.Ignore().Cascade.None();
            Map(m => m.UserId);
            Version(m => m.Version);
        }
    }
}