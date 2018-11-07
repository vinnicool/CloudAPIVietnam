using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudApiVietnam.Controllers;
using CloudApiVietnam.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http;

namespace CloudApiVietnam.Tests.Controllers
{
    [TestClass]
    public class FormulierenControllerTest
    {
        private int id;

        private static FormulierenController GetController()
        {
            return new FormulierenController
            {
                Request = new System.Net.Http.HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        private FormulierenBindingModel createFormulierenModel(string Name, string Region, string FormTemplate)
        {
            FormulierenBindingModel model = new FormulierenBindingModel
            {
                Name = Name,
                Region = Region,
                FormTemplate = FormTemplate
            };
            return model;
        }

        [TestMethod]
        [TestInitialize()]
        public void Get_Ok()
        {
            // Arrange
            FormulierenController controller = GetController();

            // Act
            HttpResponseMessage response = controller.Get();

            // Assert
            List<Formulieren> formulier;
            Assert.IsTrue(response.TryGetContentValue<List<Formulieren>>(out formulier));
            id = formulier[0].Id; //Probeer het Id van het eerste formulier te krijgen
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void GetById_Ok()
        {
            // Arrange
            FormulierenController controller = GetController();

            // Act
            HttpResponseMessage response = controller.Get(id);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void GetById_NotFound()
        {
            // Arrange
            FormulierenController controller = GetController();

            // Act
            HttpResponseMessage response = controller.Get(-2);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Post_Ok()
        {
            // Arrange
            FormulierenController controller = GetController();
            FormulierenBindingModel model = createFormulierenModel("Testformulier9999", "Zuid-Holland", "[{'Naam':'string'},{'Leeftijd':'string'},{'Woonplaats':'string'}]");
            
            // Act
            HttpResponseMessage response = controller.Post(model);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void Post_BadRequest()
        {
            // Arrange
            FormulierenController controller = GetController();
            FormulierenBindingModel model = createFormulierenModel("Testformulier9999", "Zuid-Holland", "{iets:data");

            // Act
            HttpResponseMessage response = controller.Post(model);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void Delete_NotFound()
        {
            // Arrange
            FormulierenController controller = GetController();

            // Act
            HttpResponseMessage response = controller.Get(-2);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        [TestCleanup()]
        public void Delete_NoContent()
        {
            // Arrange
            FormulierenController controller = GetController();

            // Act
            HttpResponseMessage response = controller.Get(id);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }
    }
}
