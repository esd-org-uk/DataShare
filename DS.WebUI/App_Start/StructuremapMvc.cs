using System.Web.Mvc;
using DS.WebUI.DependencyResolution;
using StructureMap;

[assembly: WebActivator.PreApplicationStartMethod(typeof(DS.WebUI.App_Start.StructuremapMvc), "Start")]

namespace DS.WebUI.App_Start {
    public static class StructuremapMvc {
        public static void Start() {
            var container = (IContainer) IoC.Initialize();
            DependencyResolver.SetResolver(new SmDependencyResolver(container));
        }
    }
}