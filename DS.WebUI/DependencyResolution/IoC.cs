
using System.Web;
using System.Web.Hosting;
using DS.DL.DataContext.Base;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain.Interface;
using DS.Service;
using DS.Service.WcfRestService;
using StructureMap;
using StructureMap.Pipeline;

namespace DS.WebUI.DependencyResolution {
    public static class IoC {
        public static IContainer Initialize() {
            ObjectFactory.Initialize(x =>
                        {
                            x.Scan(scan =>
                                    {
                                        scan.TheCallingAssembly();
                                        scan.WithDefaultConventions();
                                    });

                            x.For<IUnitOfWorkFactory>().Use<EFUnitOfWorkFactory>();
                            x.For(typeof(IRepository<>)).Use(typeof(Repository<>));
                            x.For<ISystemConfigurationService>().Use<SystemConfigurationService>();
                            x.For<ICacheProvider>().Use<HttpCache>();
                            x.For<IEsdFunctionService>().Use<EsdFunctionService>();
                            x.For<IEsdInventoryApiService>().Use<EsdInventoryApiService>();
                            x.For<IDataShareService>().Use<DataShareService>();
                            x.For<IDataShareSchemaImportService>().Use<DataShareSchemaImportService>();
                            x.For<IXmlToObjectService>().Use<XmlToObjectService>();
                            x.For<IDataSetSchemaService>().Use<DataSetSchemaService>();
                            x.For<IDataSetSchemaColumnService>().Use<DataSetSchemaColumnService>();
                            x.For<IDataSetSchemaDefinitionService>().Use<DataSetSchemaDefinitionService>();
                            x.For<IContactService>().Use<ContactService>();

                            x.For<IDataSetDetailService>().Use<DataSetDetailService>();
                            x.For<ICategoryService>().Use<CategoryService>();
                            x.For<IDataSetSchemaColumnSqlRepo>().Use<DataSetSchemaColumnSqlRepo>();
                            x.For<IDataSetDetailSqlRepo>().Use<DataSetDetailSqlRepo>();
                            x.For<IDataSetDetailCsvProcessor>().Use<DataSetDetailCsvProcessor>();
                            x.For<IDataSetDetailUploaderService>().Use<DataSetDetailUploaderService>();
                            x.For<ISqlTableUtility>().Use<SqlTableUtility>();

                            x.For<IDataTableSerializer>().Use<CustomDataTableToJsonSerializer>().Named("json");
                            x.For<IDataTableSerializer>().Use<CustomDataTableToXmlSerializer>().Named("xml");

                            x.For<IDebugInfoService>().Use<DebugInfoService>();

                            x.For<IFileSystem>().Use<CustomFileSystemUtilities>();
                            x.For<IGetFunctionServiceXmlContent>()
                             .Use<GetFunctionServiceXmlContentService>()
                             .Ctor<string>("filepath")
                             .Is(System.IO.Path.GetFullPath(HostingEnvironment.MapPath("~/App_Data/functions_services.xml")));

                            x.For<ISqlOrderByColumnAndDirectionFormatter>().Use<SqlOrderByColumnAndDirectionFormatter>();
                            x.For<IEmailService>().Use<EmailService>();

                        });
            return ObjectFactory.Container;
        }
    }
}