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
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FormulierenControllerTest
    {
        FormulierenController controller = new FormulierenController();

        [TestMethod]
        public void FormGet_Ok()
        {
            // Arramge
            FormulierenController controller = GetController();

            // Act
            HttpResponseMessage actionResult = controller.Get();

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.OK);
        }

        private static FormulierenController GetController()
        {
            return new FormulierenController
            {
                Request = new System.Net.Http.HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [TestMethod]
        public void FormGetWithId_Ok()
        {
            // Arramge
            FormulierenController controller = GetController();

            // Act
            //How to determine wich id to pass?
            HttpResponseMessage actionResult = controller.Get(9);

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void FormGetWithId_NotFound()
        {
            // Arramge
            FormulierenController controller = GetController();

            // Act
            HttpResponseMessage actionResult = controller.Get(9999999);

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void FormPost_Ok()
        {
            // Arramge
            FormulierenController controller = GetController();
            FormulierenBindingModel formulierenBindingModel = new FormulierenBindingModel();
            formulierenBindingModel.Name = "Testformulier9999";
            formulierenBindingModel.Region = "Zuid-Holland";
            formulierenBindingModel.FormTemplate = "[{'Naam':'string'},{'Leeftijd':'string'},{'Afwijking':'string'}]";

            // Act
            HttpResponseMessage actionResult = controller.Post(formulierenBindingModel);

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void FormPost_BadRequest()
        {
            // Arramge
            // Arramge
            FormulierenController controller = GetController();

            FormulierenBindingModel formulierenBindingModel = new FormulierenBindingModel();
            formulierenBindingModel.Name = "Testformulier9999";
            formulierenBindingModel.Region = "Zuid-Holland";
            formulierenBindingModel.FormTemplate = "{iets:data";

            // Act
            HttpResponseMessage actionResult = controller.Post(formulierenBindingModel);

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void FormDeleteWithId_NotFound()
        {
            // Arramge
            FormulierenController controller = GetController();

            // Act
            //How to determine wich id to pass?
            HttpResponseMessage actionResult = controller.Get(9999999);

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void FormDeleteWithId_NoContent()
        {
            // Arramge
            FormulierenController controller = GetController();

            // Act
            //How to determine wich id to pass?
            HttpResponseMessage actionResult = controller.Get(9999999);

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.OK);
        }
    }
}
