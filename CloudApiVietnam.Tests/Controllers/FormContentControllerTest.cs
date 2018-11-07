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
        private static int formContentId { get; set; }

        FormContentController controller = new FormContentController
        {
            Request = new System.Net.Http.HttpRequestMessage(),
            Configuration = new HttpConfiguration()
        };


        // 'A_' voor gezet want hij pakt alfabetische volgorde
        [ClassInitialize()]
        public static void InitTest(TestContext testContext)
        {
            FormContentController controller = new FormContentController
            {
                Request = new System.Net.Http.HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            FormContentBindingModel formContentBindingModel = new FormContentBindingModel
            {
                Content = "[{'Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]",
                FormId = 3
            };

            // Act
            HttpResponseMessage result = controller.Post(formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<FormContent>().Result;


            formContentId = resultContent.Id;
        }

        [TestMethod]
        public void Post_Succes()
        {
            FormContentBindingModel formContentBindingModel = new FormContentBindingModel
            {
                Content = "[{'Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]",
                FormId = 3
            };

            // Act
            HttpResponseMessage result = controller.Post(formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<FormContent>().Result;


            formContentId = resultContent.Id;

            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);

        }


        [TestMethod]
        public void Post_Fail_JSON()
        {
            FormContentBindingModel formContentBindingModel = new FormContentBindingModel
            {
                Content = "[{Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]",
                FormId = 3
            };

            // Act
            HttpResponseMessage result = controller.Post(formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;

            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(resultContent.Message, "JSON in 'content' is not correct JSON: Invalid JavaScript property identifier character: '. Path '[0]', line 1, position 6.");
        }

        [TestMethod]
        public void Delete_Succes()
        {
            // Act
            HttpResponseMessage result = controller.Delete(formContentId);
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);

        }

        [TestMethod]
        public void GetById_Succes()
        {
            // Act
            HttpResponseMessage result = controller.Get(formContentId);
            var resultContent = result.Content.ReadAsAsync<FormContent>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);
        }

        [TestMethod]
        public void GetAll_Succes()
        {
            // Act
            HttpResponseMessage result = controller.Get();
            var resultContent = result.Content.ReadAsAsync<dynamic>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);

        }

        [TestMethod]
        public void Put_Succes()
        {
         
                FormContentBindingModel formContentBindingModel = new FormContentBindingModel();

            formContentBindingModel.FormId = formContentId;
            formContentBindingModel.Content = "[{Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]";

            HttpResponseMessage result = controller.Put(formContentId, formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<dynamic>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);

        }


    }
}
