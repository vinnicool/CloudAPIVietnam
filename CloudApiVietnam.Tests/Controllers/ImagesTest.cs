using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudApiVietnam.Controllers;
using CloudApiVietnam.Models;
using System.Web.Http;
using System.Web.Http.Results;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
//using Moq;
using System.Web;
using System.Globalization;

namespace CloudApiVietnam.Tests.Controllers
{
    [TestClass]
    public class ImagesTest
    {
        ImagesController controller = new ImagesController();
        [TestMethod]
        public void PostImage_Succes()
        {
            // Arramge

            //Act

            // Assert
        }
        [TestMethod]
        public async Task PostImage_ShouldReturnImageToLargeAsync()
        {

        }
        [TestMethod]
        public void PostImage_ShouldReturnImageTypeNotValid()
        {
            // Arramge

            //Act

            // Assert
        }
        [TestMethod]
        public async Task GetImage_ShouldReturnImageNotFoundAsync()
        {
            // Arramge
            // None

            // Act
            HttpResponseMessage actionResult = await controller.Get("3617ebac-a459-xxxx-bb15-9be01b06cc48");

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.NotFound);
        }
        [TestMethod]
        public async Task GetImage_ImageFoundAsync()
        {
            // Arramge

            // Act
            HttpResponseMessage actionResult =  await controller.Get("c3887efa-1dda-4f2a-9e82-030ef2d9c11b");
            
            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(actionResult.Content.Headers.ContentLength, 46361);
            Assert.AreEqual("image/jpeg",actionResult.Content.Headers.ContentType.MediaType);

        }
        [TestMethod]
        public async Task DeleteImage_ImageNotFoundAsync()
        {
            // Arramge

            // Act
            HttpResponseMessage actionResult = await controller.Delete("3617ebac-a459-xxxx-bb15-9be01b06cc48");

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.NotFound);
        }
        [TestMethod]
        public async Task DeleteImage_NoContentAsync()
        {
            // Arramge

            // Act
            HttpResponseMessage actionResult = await controller.Delete("e86d9809-da2d-4958-9b59-a94dc212611f");

            // Assert
            Assert.AreEqual(actionResult.StatusCode, HttpStatusCode.NoContent);
        }
    }
}
