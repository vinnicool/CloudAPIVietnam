using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using CloudApiVietnam.Models;
using CloudApiVietnam.Providers;
using CloudApiVietnam.Results;
using System.Linq;
using System.Net;

namespace CloudApiVietnam.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        //GET /api/Account
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Get()
        {
            try
            {
                List<UserInfo> usersInfo = new List<UserInfo>();
                List<User> users = db.Users.ToList();
                if(users == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "There are no users in the database.");

                foreach (User user in users)
                {
                    UserInfo info = new UserInfo
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = user.Roles,
                        UserName = user.UserName
                    };
                    usersInfo.Add(info);
                }
                return Request.CreateResponse(HttpStatusCode.OK, usersInfo);
            } catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
            }
        }

        //GET /api/Account/{id}
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                User user = db.Users.Where(u => u.Id == id).FirstOrDefault();

                if(user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No user found with id: " + id);

                UserInfo info = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = user.Roles,
                    UserName = user.UserName
                };
                return Request.CreateResponse(HttpStatusCode.OK, info);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
            }
        }
               
        //POST /api/Account
        //Voor nu even AllowAnonymous voor het eenvoudig testen
        [AllowAnonymous]
        public HttpResponseMessage Post(RegisterBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                IdentityResult result = new IdentityResult();
                try
                {
                    result = UserManager.Create(user, model.Password);
                }
                catch
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, "The user could not be added.");
                }

                if (!result.Succeeded)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.Errors.ToString());

                try
                {
                    UserManager.AddToRole(user.Id, model.UserRole);
                }
                catch
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, "The user role could not be added.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, user);
            } catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
            }
        }
        
        //DELETE /api/Account
        [AllowAnonymous]
        public HttpResponseMessage Delete(string id)
        {
            User user = db.Users.Where(f => f.Id == id).FirstOrDefault();

            if (user == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            else
            {
                try
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
                }
            }
        }

        //PUT /api/Account/{id}
        [AllowAnonymous]
        public HttpResponseMessage Put(string id, [FromBody]RegisterBindingModel model)
        {
            User user = db.Users.Where(f => f.Id == id).FirstOrDefault();
            IdentityRole role = db.Roles.Where(r => r.Name == model.UserRole).FirstOrDefault();
            
            if (user == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No user found with id: " + id.ToString());
            else
            {
                try
                {
                    if (role == null)
                        throw new System.ArgumentException("There is no userrole named: " + role.Name);
                    else
                    {
                        if (user.Roles.FirstOrDefault().RoleId != role.Id)
                        {
                            UserManager.RemoveFromRole(user.Id, role.Name);
                            UserManager.AddToRole(user.Id, model.UserRole);
                        }
                        user.Email = model.Email;
                    }
                 
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
                }
            }
        }

        #region Helpers




        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

      
        #endregion
    }
}
