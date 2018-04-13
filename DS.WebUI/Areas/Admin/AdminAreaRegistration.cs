using System.Web.Mvc;

namespace DS.WebUI.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            context.Routes.IgnoreRoute("{*alljs}", new { alljs = @".*\.js(/.*)?" });//ignore all js files
            context.Routes.IgnoreRoute("{*allcss}", new { alljs = @".*\.css(/.*)?" });//ignore all css files

            context.MapRoute(
               "Admin_Upload_Csv", // Route name
               "Admin/Upload/UploadCsv", // URL with parameters
               new { controller = "Upload", action = "UploadCsv" }
            );

            context.MapRoute(
               "Admin_Upload_CheckExistsAlready", // Route name
               "Admin/Upload/CheckExistsAlready", // URL with parameters
               new { controller = "Upload", action = "CheckExistsAlready" }
            );

            context.MapRoute(
               "Admin_Upload_SaveCsv", // Route name
               "Admin/Upload/SaveCsv", // URL with parameters
               new { controller = "Upload", action = "SaveCsv" }
            );

            context.MapRoute(
               "Admin_Upload_Delete", // Route name
               "Admin/Upload/Delete/{dataSetDetailId}/{schemaId}", // URL with parameters
               new { controller = "Upload", action = "Delete", dataSetDetailId = UrlParameter.Optional, schemaId = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Upload_DownloadTemplate", // Route name
                "Admin/Upload/DownloadTemplate/{schemaId}/{schemaName}", // URL with parameters
                new { controller = "Upload", action = "DownloadTemplate", schemaId = UrlParameter.Optional, schemaName = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Upload_DownloadDefinition", // Route name
                "Admin/Upload/DownloadDefinition/{schemaId}", // URL with parameters
                new { controller = "Upload", action = "DownloadDefinition", schemaId = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Upload_GetData", // Route name
                "Admin/Upload/GetData", // URL with parameters
                new { controller = "Upload", action = "GetData"}
            );

            context.MapRoute(
              "Admin_Upload", // Route name
              "Admin/Upload/{category}/{schema}/{actionToPerform}", // URL with parameters
              new { controller = "Upload", action = "Index", category = UrlParameter.Optional, schema = UrlParameter.Optional, actionToPerform = UrlParameter.Optional },
              new { action = "^(?!Save).*" },
              new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                 "Admin_Category", // Route name
                 "Admin/Category", // URL with parameters
                 new { controller = "Category", action = "Index"},
                 new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

            context.MapRoute(
             "Admin_Category_Create", // Route name
             "Admin/Category/Create", // URL with parameters
             new { controller = "Category", action = "Create"},
             new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

            context.MapRoute(
              "Admin_Category_Edit", // Route name
              "Admin/Category/{categoryName}", // URL with parameters
              new { controller = "Category", action = "Edit", categoryName = UrlParameter.Optional },
              new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

            context.MapRoute(
              "Admin_Category_Delete", // Route name
              "Admin/Category/{categoryName}/Delete", // URL with parameters
              new { controller = "Category", action = "Delete", categoryName = UrlParameter.Optional },
              new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

            context.MapRoute(
              "Admin_Category_Enable", // Route name
              "Admin/Category/{categoryName}/Enable", // URL with parameters
              new { controller = "Category", action = "Enable", categoryName = UrlParameter.Optional },
              new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );
            
            context.MapRoute(
              "Admin_Schema_Create", // Route name
              "Admin/Schema/{categoryName}/Create", // URL with parameters
              new { controller = "Schema", action = "Create", categoryName = UrlParameter.Optional }, 
              new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                "Admin_Schema_SchemaSource", // Route name
                "Admin/Schema/{categoryName}/SchemaSource", // URL with parameters
                new { controller = "Schema", action = "SchemaSource", categoryName = UrlParameter.Optional },
                new[] { "DS.WebUI.Areas.Admin.Controllers" }
                );
           context.MapRoute(
              "Admin_Schema_Edit", // Route name
              "Admin/Schema/{categoryName}/{schemaName}/Edit", // URL with parameters
              new { controller = "Schema", action = "Edit", categoryName = UrlParameter.Optional, schemaName = UrlParameter.Optional }, 
              new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

           context.MapRoute(
       "Admin_Schema_Edit_Ajax", // Route name
       "Admin/Schema/{categoryName}/{schemaName}/SaveSorting", // URL with parameters
       new { controller = "Schema", action = "SaveSorting", categoryName = UrlParameter.Optional, schemaName = UrlParameter.Optional },
       new[] { "DS.WebUI.Areas.Admin.Controllers" }
     );
           context.MapRoute(
             "Admin_Schema_Approve", // Route name
             "Admin/Schema/{categoryName}/{schemaName}/Approve", // URL with parameters
             new { controller = "Schema", action = "Approve", categoryName = UrlParameter.Optional, schemaName = UrlParameter.Optional },
             new { action = "^(?!Save).*" },
             new[] { "DS.WebUI.Areas.Admin.Controllers" }
           );

           context.MapRoute(
             "Admin_Schema_Delete", // Route name
             "Admin/Schema/{categoryName}/{schemaName}/Delete", // URL with parameters
             new { controller = "Schema", action = "Delete", categoryName = UrlParameter.Optional, schemaName = UrlParameter.Optional },
             new { action = "^(?!Save).*" },
             new[] { "DS.WebUI.Areas.Admin.Controllers" }
           );

           context.MapRoute(
              "Admin_Schema_DeleteAll", // Route name
              "Admin/Schema/{categoryName}/{schemaName}/DeleteAll", // URL with parameters
              new { controller = "Schema", action = "DeleteAll", categoryName = UrlParameter.Optional, schemaName = UrlParameter.Optional },
              new { action = "^(?!Save).*" },
              new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

           context.MapRoute(
             "Admin_Schema_Enable", // Route name
             "Admin/Schema/{categoryName}/{schemaName}/Enable", // URL with parameters
             new { controller = "Schema", action = "Enable", categoryName = UrlParameter.Optional, schemaName = UrlParameter.Optional },
             new { action = "^(?!Save).*" },
             new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

           context.MapRoute(
                "Admin_Schema_AddColumn", // Route name
                "Admin/Schema/{categoryName}/{schemaName}/AddColumn", // URL with parameters
                new { controller = "Schema", action = "AddColumn", categoryName = UrlParameter.Optional, schemaName = UrlParameter.Optional }, 
                new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

           context.MapRoute(
               "Admin_Schema_EditColumn", // Route name
               "Admin/Schema/{categoryName}/{schemaName}/{columnName}/EditColumn", // URL with parameters
               new { controller = "Schema", action = "EditColumn", categoryName = UrlParameter.Optional, schemaName = UrlParameter.Optional, columnName = UrlParameter.Optional },
               new[] { "DS.WebUI.Areas.Admin.Controllers" }
               );

            context.MapRoute(
                "Admin_Schema_DeleteColumn", // Route name
                "Admin/Schema/{categoryName}/{schemaName}/{columnName}/DeleteColumn", // URL with parameters
                new { controller = "Schema", action = "DeleteColumn", categoryName = UrlParameter.Optional, schemaName = UrlParameter.Optional, columnName = UrlParameter.Optional },
                new [] { "DS.WebUI.Areas.Admin.Controllers" }
                );

            context.MapRoute(
              "Admin_Schema", // Route name
              "Admin/Schema/{categoryName}", // URL with parameters
              new { controller = "Schema", action = "Index", categoryName = UrlParameter.Optional },
              new[] { "DS.WebUI.Areas.Admin.Controllers" }
            );

           context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new [] { "DS.WebUI.Areas.Admin.Controllers" }
            );
        }
    }
}
