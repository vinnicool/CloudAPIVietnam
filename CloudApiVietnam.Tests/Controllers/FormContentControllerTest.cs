using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudApiVietnam;
using CloudApiVietnam.Controllers;
using CloudApiVietnam.Models;
using System.Net;

namespace CloudApiVietnam.Tests.Controllers
{
    [TestClass]
    public class FormContentControllerTest
    {

        // Arrange();
        FormContentController controller = new FormContentController
        {
            Request = new System.Net.Http.HttpRequestMessage(),
            Configuration = new HttpConfiguration()
        };

        [TestMethod]
        public void Post()
        {
            FormContentBindingModel formContentBindingModel = new FormContentBindingModel
            {
                Content = "[{'Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]",
                FormId = 3
            };

            // Act
            HttpResponseMessage result = controller.Post(formContentBindingModel);

            // Assert
            int i = 1;
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
                        
        }

        //[TestMethod]
        //public void Get()
        //{

        //    // Act
        //    var result = controller.Get();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    //Assert.AreEqual(2, result.Count());
        //    //Assert.AreEqual("value1", result.ElementAt(0));
        //    //Assert.AreEqual("value2", result.ElementAt(1));
        //}

        //[TestMethod]
        //public void GetById()
        //{
        //    // Arrange
        //    ValuesController controller = new ValuesController();

        //    // Act
        //    string result = controller.Get(5);

        //    // Assert
        //    Assert.AreEqual("value", result);
        //}



        //[TestMethod]
        //public void Put()
        //{
        //    // Arrange
        //    ValuesController controller = new ValuesController();

        //    // Act
        //    controller.Put(5, "value");

        //    // Assert
        //}

        //[TestMethod]
        //public void Delete()
        //{
        //    // Arrange
        //    ValuesController controller = new ValuesController();

        //    // Act
        //    controller.Delete(5);

        //    // Assert
        //}
    }
}
