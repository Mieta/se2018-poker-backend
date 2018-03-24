using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlanningPoker2018_backend;
using PlanningPoker2018_backend.Controllers;

namespace PlanningPoker2018_backend.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Przygotowanie
            HomeController controller = new HomeController();

            // Wykonanie
            ViewResult result = controller.Index() as ViewResult;

            // Sprawdzenie
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
