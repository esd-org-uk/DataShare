using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Mvc;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using StructureMap;

namespace DS.WebUI.Controllers.Base
{
    public class BaseController : Controller
    {
        protected ISystemConfigurationService _systemConfigurationService;
        protected ICategoryService _categoryService;

        public BaseController(ISystemConfigurationService systemConfigurationService, ICategoryService categoryService)
        {
            _systemConfigurationService = systemConfigurationService;
            _categoryService = categoryService;
            if (_systemConfigurationService != null)
            {
                ViewBag.CouncilTitle = _systemConfigurationService.GetSystemConfigurations().CouncilName;//ConfigurationManager.AppSettings["CouncilName"];
                ViewBag.CouncilUrl = _systemConfigurationService.GetSystemConfigurations().CouncilUrl;
                ViewBag.AnalyticsTrackingRef = _systemConfigurationService.GetSystemConfigurations().AnalyticsTrackingRef;
                
            }

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //ViewBag.CouncilTitle = ConfigurationManager.AppSettings["CouncilName"];
            BuildBreadCrumb();

        }

   
        /// <summary>
        /// create/edit new user - ken1/ken2/ken6
        /// for admin it uses all if else ( ken1/2/3/4/5/6)
        /// for frontend uses ken1/3/4
        /// todo- try adnd refactor this to something more readable
        ///  </summary>
        private void BuildBreadCrumb()
        {

            ViewBag.BreadCrumbPrefix = "hacked_key_schema_";
            var allTitles = _categoryService.BreadCrumbsTitles();

            var controller = RouteData.Values["controller"].ToString().ToLower();
            var category = RouteData.Values.ContainsKey("category") ? RouteData.Values["category"].ToString() : "";

            category = (category == "" && RouteData.Values.ContainsKey("categoryName")
                           ? RouteData.Values["categoryName"].ToString()
                           : category).ToLower();
            
            var schema = RouteData.Values.ContainsKey("schema") ? RouteData.Values["schema"].ToString() : "";
            
            schema = (schema == "" && RouteData.Values.ContainsKey("schemaName")
                         ? RouteData.Values["schemaName"].ToString()
                         : schema).ToLower();
            
            var action = RouteData.Values.ContainsKey("action") ? RouteData.Values["action"].ToString().ToLower() : "";

            if (controller != "" && controller != "home")//***Ken1***
            {
                ViewBag.BreadCrumbs = new OrderedDictionary { { "Home", "/" } };
                var key = allTitles.ContainsKey(controller) ? allTitles[controller] : controller;
                var value = (category == "" && action == "index") ? "" : string.Format("/{0}", controller);
                ViewBag.BreadCrumbs.Add(key, value);
                ViewBag.BackUrl = new KeyValuePair<string,string>("/","Home");
            }

            if (action != "" && action.ToLower() != "index" && category == "" && schema == "")//***Ken2***
            {
                var key = allTitles.ContainsKey(action) ? allTitles[action] : "";
                var value = "";
                ViewBag.BreadCrumbs.Add(key, value);
                ViewBag.BackUrl = new KeyValuePair<string, string>(string.Format("/{0}", controller), allTitles.ContainsKey(controller) ? allTitles[controller] : controller);
            }

            if (category != "")//***Ken3***
            {
                var key = allTitles.ContainsKey(category) ? allTitles[category] : category;
                var value = schema != "" ? string.Format("/{0}/{1}", controller, category) : "";
                ViewBag.BreadCrumbs.Add(key,value);
                ViewBag.BackUrl = new KeyValuePair<string, string>(string.Format("/{0}", controller), allTitles.ContainsKey(controller) ? allTitles[controller] : controller);                
            }
            if (schema != "" && action.ToLower() != "addcolumn" && action.ToLower() != "editcolumn")//***Ken4***
            {
                
                var key =  allTitles.ContainsKey(category + '_' + schema)
                              ? allTitles[category + '_' + schema]
                              : category + " " + schema;
                ViewBag.BreadCrumbs.Add(ViewBag.BreadCrumbPrefix + key, "");
                ViewBag.BackUrl = new KeyValuePair<string, string>(string.Format("/{0}/{1}", controller, category), allTitles.ContainsKey(category) ? allTitles[category] : category);                
            }
            else if ((action.ToLower() == "addcolumn" || action.ToLower() == "editcolumn") && category != "" && schema != "")//***Ken5***
            {
                var key = allTitles.ContainsKey(category + '_' + schema)
                              ? allTitles[category + '_' + schema]
                              : category + " - " + schema;
                var value = string.Format("/{0}/{1}/{2}/Edit?showfields=2", controller, category, schema);

                if (ViewBag.BreadCrumbs.Contains(key)) 
                    ViewBag.BreadCrumbs.Add(ViewBag.BreadCrumbPrefix + key, value);
                else
                    ViewBag.BreadCrumbs.Add(key, value);

                ViewBag.BreadCrumbs.Add("Define field" , "");
                ViewBag.BackUrl = new KeyValuePair<string, string>(string.Format("/{0}/{1}/{2}/Edit?showfields=2", controller, category, schema), allTitles.ContainsKey(schema) ? allTitles[schema] : schema);
            }
            else if (controller == "useradmin" && action != "index")//***Ken6***
            {
                ViewBag.BreadCrumbs.Add("Edit User","");
            }
        }


     
    }
}
