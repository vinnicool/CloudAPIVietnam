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
        // Arrange
        AccountController controller = new AccountController();
        string testUserId;

        [TestMethod]
        public void Get_Ok()
        {
            // Act
            HttpResponseMessage response = controller.Get();

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod] 
        public void GetById_Ok()
        {
            //Act
            HttpResponseMessage response = controller.Get(testUserId);

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void GetById_NotFound()
        {
            //Act
            HttpResponseMessage response = controller.Get("999999");

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Post_Ok()
        {
            RegisterBindingModel model = new RegisterBindingModel();
            model.Email = Path.GetRandomFileName().Replace(".", "").Substring(0, 8) + "@ivobot.nl";
            model.Password = "Welkom123!";
            model.ConfirmPassword = "Welkom123!";
            model.UserRole = "Admin";

            //Act
            Task<IHttpActionResult> response = controller.Post(model);

            //Assert
            //TODO Nog kijken of response.Status gelijk is aan Ok()
            Assert.AreEqual(response.Status, Ok());
        }

        [TestMethod]
        public void Post_Faulted_Password()
        {
            RegisterBindingModel model = new RegisterBindingModel();
            model.Email = Path.GetRandomFileName().Replace(".", "").Substring(0, 8) + "@ivobot.nl";
            model.Password = "Welkom123!";
            model.ConfirmPassword = "NietGelijkAanAndere";
            model.UserRole = "Admin";

            //Act
            Task<IHttpActionResult> response = controller.Post(model);


            //Assert
            Assert.AreEqual(response.Status, TaskStatus.Faulted);
        }

        [TestMethod]
        public void Post_Faulted_Role()
        {
            RegisterBindingModel model = new RegisterBindingModel();
            model.Email = Path.GetRandomFileName().Replace(".", "").Substring(0, 8) + "@ivobot.nl";
            model.Password = "Welkom123!";
            model.ConfirmPassword = "Welkom123!";
            model.UserRole = "GeenRole";

            //Act
            Task<IHttpActionResult> response = controller.Post(model);

            //Assert
            Assert.AreEqual(response.Status, TaskStatus.Faulted);
        }

        [TestMethod]
        public void Delete()
        {
            //Act
            //TODO Wijzigen naar correcte ID die altijd bestaat
            HttpResponseMessage response = controller.Delete("1");

            //Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        private TaskStatus Ok()
        {
            throw new NotImplementedException();
        }

        public AccountControllerTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            //
            // TODO: Add test logic here
            //
        }
    }
}
