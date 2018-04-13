using System;
using System.Data.Entity;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using DS.DL.DataContext;
using DS.DL.DataContext.Base;
using DS.Service.WcfRestService;
using Elmah;

namespace DS.WebUI
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ElmahHandleErrorAttribute());
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*alljs}", new { alljs = @".*\.js(/.*)?" });//ignore all js files
            routes.IgnoreRoute("{*allcss}", new { alljs = @".*\.css(/.*)?" });//ignore all css files
            routes.IgnoreRoute("{*allaspx}", new {allaspx=@".*\.aspx(/.*)?"});
            routes.IgnoreRoute("{*robotstxt}", new {robotstxt=@"(.*/)?robots.txt(/.*)?"});
            routes.IgnoreRoute("{*favicon}", new {favicon=@"(.*/)?favicon.ico(/.*)?"});

            routes.MapRoute(
               "Security", // Route name
               "Security", // URL with parameters
               new { controller = "Security", action = "Login"}, // Parameter defaults
               new[] { "DS.WebUI.Controllers" }
            );

            routes.MapRoute(
               "Download", // Route name
               "Download/{category}/{schema}/{fileTitle}/{downloadFormat}", // URL with parameters
               new { controller = "Download", action = "Index", category = UrlParameter.Optional, schema = UrlParameter.Optional, fileTitle = UrlParameter.Optional, downloadFormat = UrlParameter.Optional }, // Parameter defaults
               new { controller = "^(?!api).*" },
               new[] { "DS.WebUI.Controllers" }
            );

            routes.MapRoute(
               "ViewAjax", // Route name
               "View/Ajax/{category}/{schema}", // URL with parameters
               new { controller = "View", action = "Ajax", category = UrlParameter.Optional, schema = UrlParameter.Optional}, // Parameter defaults
               new { controller = "^(?!api).*" },
               new[] { "DS.WebUI.Controllers" }
            );

            routes.MapRoute(
                "ViewHelp",
                "View/Help",
                new {controller = "View", action = "Help"},
                new[] {"DS.WebUi.Controllers"}
            );

            routes.MapRoute(
               "View", // Route name
               "View/{category}/{schema}", // URL with parameters
               new { controller = "View", action = "Index", category = UrlParameter.Optional, schema = UrlParameter.Optional }, // Parameter defaults
               new { controller = "^(?!api).*" },
               new[] { "DS.WebUI.Controllers" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },// Parameter defaults
                new { controller = "^(?!api).*" },
                new [] { "DS.WebUI.Controllers" }
            );

            routes.Add(new ServiceRoute("api", new WebServiceHostFactory(), typeof(DataShareService)));
        }

        protected void Application_Start()
        {
            //// Setup StructureMap to determine the concrete repository pattern to use.
            //ObjectFactory.Initialize(
            //   x =>
            //   {
            //       x.For<IUnitOfWorkFactory>().Use<EFUnitOfWorkFactory>();
            //       x.For(typeof(IRepository<>)).Use(typeof(Repository<>));
            //   }
            //);
            /*Moved to dependencyResolution IoC.cs*/

            // Select an Entity Framework model to use with the factory.
            EFUnitOfWorkFactory.SetObjectContext(() => new DataShareContext());

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //Never recreate the database
            Database.SetInitializer<DataShareContext>(null);
#if DEBUG            
            //track LINQ queries executed using entity FrameworkProfiler
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
#endif

            var sqlUtility = new DS.Service.SqlTableUtility();
            sqlUtility.DropTables(sqlUtility.GetUnusedDSTables());
            sqlUtility = null;

        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.Request["RequireUploadifySessionSync"] != null)
                UploadifySessionSync();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            UnitOfWork.Current.Dispose(); 
        }

        protected void UploadifySessionSync()
        {
            try
            {
                string session_param_name = "SessionId";
                string session_cookie_name = "ASP.NET_SessionId";

                if (HttpContext.Current.Request[session_param_name] != null)
                    UploadifyUpdateCookie(session_cookie_name, HttpContext.Current.Request.Form[session_param_name]);
            }
            catch (Exception ex) { Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex)); }

            try
            {
                string auth_param_name = "SecurityToken";
                string auth_cookie_name = FormsAuthentication.FormsCookieName;

                if (HttpContext.Current.Request[auth_param_name] != null)
                    UploadifyUpdateCookie(auth_cookie_name, HttpContext.Current.Request.Form[auth_param_name]);
            }
            catch (Exception ex) { Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex)); }
        }

        private void UploadifyUpdateCookie(string cookie_name, string cookie_value)
        {
            var cookie = HttpContext.Current.Request.Cookies.Get(cookie_name);
            if (cookie == null)
                cookie = new HttpCookie(cookie_name);
            cookie.Value = cookie_value;
            HttpContext.Current.Request.Cookies.Set(cookie);
        }

        public class ElmahHandleErrorAttribute : HandleErrorAttribute
        {
            public override void OnException(ExceptionContext context)
            {
                base.OnException(context);

                var e = context.Exception;
                if (!context.ExceptionHandled   // if unhandled, will be logged anyhow
                    || RaiseErrorSignal(e)      // prefer signaling, if possible
                    || IsFiltered(context))     // filtered?
                    return;

                LogException(e);
            }

            private static bool RaiseErrorSignal(Exception e)
            {
                var context = HttpContext.Current;
                if (context == null)
                    return false;
                var signal = ErrorSignal.FromContext(context);
                if (signal == null)
                    return false;
                signal.Raise(e, context);
                return true;
            }

            private static bool IsFiltered(ExceptionContext context)
            {
                var config = context.HttpContext.GetSection("elmah/errorFilter")
                             as ErrorFilterConfiguration;

                if (config == null)
                    return false;

                var testContext = new ErrorFilterModule.AssertionHelperContext(
                                          context.Exception, HttpContext.Current);

                return config.Assertion.Test(testContext);
            }

            private static void LogException(Exception e)
            {
                var context = HttpContext.Current;
                ErrorLog.GetDefault(context).Log(new Error(e, context));
            }
        }
    }
}