using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using CloudApiVietnam.Controllers;
using CloudApiVietnam.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudApiVietnam.Tests.Controllers
{
    [TestClass]
    class FormulierenControllerTest
    {

        //Arrange
        FormulierenController controller = new FormulierenController();

        [TestMethod]
        public void Get()
        {
            //Act
            HttpResponseMessage result = controller.Get();

            //Assert
            Assert.IsNotNull(result);
            //Op nog meer dingen testen?
        }

        [TestMethod]
        public void GetById()
        {
            //Act
            HttpResponseMessage result = controller.Get(1);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Post()
        {
            FormulierenBindingModel model = new FormulierenBindingModel();
            model.Name = "TestModel";
            model.Region = "Hanoi";
            model.FormTemplate = "[{'Test':'string'},{'Naam':'string'},{'Naam':'string'}]";

            //Act
            HttpResponseMessage result = controller.Post(model);

            //Arrange
            Assert.IsNotNull(result);
        }

        [TestMethod] 
        public void Put()
        {
            FormulierenBindingModel model = new FormulierenBindingModel();
            model.Name = "NieuweTestModel";
            model.Region = "Vinh";
            model.FormTemplate = "[{'NieuweTest':'string'},{'Naam':'string'},{'Naam':'string'}]";

            //Act
            HttpResponseMessage result = controller.Put(1, model);

            //Arrange
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void Delete()
        {
            //Act
            IHttpActionResult result = controller.Delete(1);

            //Arrange
            Assert.IsNotNull(result);
        }
    }
}
