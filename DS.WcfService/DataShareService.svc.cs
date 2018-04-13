using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;
using DS.DL.DataContext;
using DS.DL.DataContext.Base;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Service;
using StructureMap;

namespace DS.WcfService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class DataShareService : IDataShareService
    {
        public DataShareService()
        {
            // Setup StructureMap to determine the concrete repository pattern to use.
            ObjectFactory.Initialize(
               x =>
               {
                   x.For<IUnitOfWorkFactory>().Use<EFUnitOfWorkFactory>();
                   x.For(typeof(IRepository<>)).Use(typeof(Repository<>));
               }
            );

            // Select an Entity Framework model to use with the factory.
            EFUnitOfWorkFactory.SetObjectContext(() => new DataShareContext());
        }

        public Category GetCategory()
        {
            var data = CategoryService.Repository.GetAll().FirstOrDefault();
            //var returnData = new XElement("Root", data.InnerXml);

            return data;
        }
    }
}
