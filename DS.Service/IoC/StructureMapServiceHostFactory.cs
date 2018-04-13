using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using DS.DL.DataContext.Base;
using DS.DL.DataContext.Base.Interfaces;
using DS.Service.WcfRestService;
using StructureMap;
using StructureMap.Graph;

namespace DS.Service.IoC
{
    public class StructureMapServiceHostFactory : ServiceHostFactory
    {
        public StructureMapServiceHostFactory()
        {
            ObjectFactory.Initialize(x =>
            {
                x.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();


                    x.For<IUnitOfWorkFactory>().Use<EFUnitOfWorkFactory>();
                    x.For(typeof(IRepository<>)).Use(typeof(Repository<>));
                    x.For<ISystemConfigurationService>().Use<SystemConfigurationService>();
                    x.For<ICacheProvider>().Use<HttpCache>();
                    x.For<IEsdFunctionService>().Use<EsdFunctionService>();
                    x.For<IDataShareService>().Use<DataShareService>();

                   
                });
            });
        }
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new StructureMapServiceHost(serviceType, baseAddresses);
        }
    }


}
