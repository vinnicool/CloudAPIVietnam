using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudApiVietnam.Controllers;
using CloudApiVietnam.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using System.IO;
using System.Web.Http.Results;

namespace CloudApiVietnam.Tests.Controllers
{
    /// <summary>
    /// Summary description for AccountControllerTest
    /// </summary>
    [TestClass]
    public class AccountControllerTest
    {
        private string id;

        private static AccountController GetController()
        {
            return new AccountController
            {
                Request = new System.Net.Http.HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        private RegisterBindingModel createModel(string Email, string Password, string ConfirmPassword, string UserRole) 
        {
            RegisterBindingModel model = new RegisterBindingModel();
            model.Email = Email;
            model.Password = Password;
            model.ConfirmPassword = ConfirmPassword;
            model.UserRole = UserRole;
            return model;
        }

        private string GetRandomEmail()
        {
            return Path.GetRandomFileName().Replace(".", "").Substring(0, 8) + "@ivobot.nl";
        }

        [TestMethod]
        public void Get_Ok()
        {
            // Act
            HttpResponseMessage response = GetController().Get();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod] 
        public void GetById_Ok()
        {
            //Act
            HttpResponseMessage response = GetController().Get(id);

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void GetById_NotFound()
        {
            //Act
            HttpResponseMessage response = GetController().Get("999999");

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        [TestInitialize()]
        public void Post_Ok()
        {
            string Email = GetRandomEmail();
            RegisterBindingModel model = createModel(Email, "Welkom123!", "Welkom123!", "Admin");

            //Act
            HttpResponseMessage response = GetController().Post(model);

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            User user;
            Assert.IsTrue(response.TryGetContentValue<User>(out user));
            id = user.Id;
        }

        [TestMethod]
        public void Post_Faulted_Password()
        {
            string Email = GetRandomEmail();
            RegisterBindingModel model = createModel(Email, "Welkom123!", "NietGelijkAanAndere1!", "Admin");

            //Act
            HttpResponseMessage response = GetController().Post(model);

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Post_Conflict()
        {
            string Email = GetRandomEmail();
            RegisterBindingModel model = createModel(Email, "Welkom123!", "Welkom123!", "RolDieNietBestaat");

            //Act
            HttpResponseMessage response = GetController().Post(model);

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Conflict);
        }

        [TestMethod]
        [TestCleanup()]
        public void Delete_OK()
        {
            //Act
            HttpResponseMessage response = GetController().Delete(id);

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void Delete_NotFound()
        {
            //Act
            HttpResponseMessage response = GetController().Delete("9999999");

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Delete_Conflict()
        {
            //Act
            HttpResponseMessage response = GetController().Delete("test");

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Conflict);
        }

        [TestMethod]
        public void Put_OK()
        {
            //Arrange
            string Email = GetRandomEmail();
            RegisterBindingModel model = createModel(Email, "Welkom1234!", "Welkom1234!", "User");

            //Act
            HttpResponseMessage response = GetController().Put(id, model);

            //Assert
            Assert.Equals(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void Put_NotFound()
        {
            //Arrange
            string Email = GetRandomEmail();
            RegisterBindingModel model = createModel(Email, "Welkom123!", "Welkom123!", "Admin");

            //Act
            HttpResponseMessage response = GetController().Put("999999", model);

            //Assert
            Assert.Equals(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void Put_Conflict_Id()
        {
            //Arrange
            string Email = GetRandomEmail();
            RegisterBindingModel model = createModel(Email, "Welkom123!", "Welkom123!", "Admin");

            //Act
            HttpResponseMessage response = GetController().Put("teest", model);

            //Assert
            Assert.Equals(response.StatusCode, HttpStatusCode.Conflict);
        }

        [TestMethod]
        public void Put_Conflict_Role()
        {
            //Arrange
            string Email = GetRandomEmail();
            RegisterBindingModel model = createModel(Email, "Welkom123!", "Welkom123!", "TestRole");

            //Act
            HttpResponseMessage response = GetController().Put(id, model);

            //Assert
            Assert.Equals(response.StatusCode, HttpStatusCode.Conflict);
        }
    }
}
