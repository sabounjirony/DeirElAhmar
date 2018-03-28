using System.Xml.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace DM.Conventions
{
    public class XmlTypeConvention : IUserTypeConvention
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Type == typeof(XDocument));
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.CustomType<NHibernate.Type.XDocType>();
        }
    } 
}