using FluentNHibernate.Mapping;
using DM.Models.Setup;

namespace DM.Mappings.Setup
{
    class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Not.LazyLoad();
            Table("_Employee");
            Id(x => x.Id).Column("Id").GeneratedBy.HiLo("_Identity", "Sequence", "0", "Id ='EmployeeId'");
            Map(m => m.EntryDate);
            Map(m => m.Status);
            Map(m => m.Level);
            //References(m => m.User, "UserId").ForeignKey("Id").Cascade.None().NotFound.Ignore();
            Map(m => m.UserId);
            References(m => m.Entity, "Pin").ForeignKey("Pin").Cascade.All().NotFound.Ignore();
            Version(m => m.Version);
        }
    }
}