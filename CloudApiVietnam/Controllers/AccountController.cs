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


        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Get()
        {
            try
            {
                List<UserInfo> usersInfo = new List<UserInfo>();
                var users = db.Users.ToList();
                foreach (User user in users)
                {
                    UserInfo info = new UserInfo();
                    info.Id = user.Id;
                    info.Email = user.Email;
                    info.Roles = user.Roles;
                    info.UserName = user.UserName;
                    usersInfo.Add(info);
                }

                return Request.CreateResponse(HttpStatusCode.OK, usersInfo);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }


        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                User user = db.Users.Where(u => u.Id == id).FirstOrDefault();
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
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        
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
               
  
        [AllowAnonymous]
        public HttpResponseMessage Post(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ModelState);
            }

            var user = new User() { UserName = model.Email, Email = model.Email };

            IdentityResult result = UserManager.Create(user, model.Password);

            if (!result.Succeeded)
            {

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, result.Errors.ToString());
            }
            UserManager.AddToRole(user.Id, model.UserRole);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }




        [AllowAnonymous]
        public HttpResponseMessage Delete(string id)
        {
            var User = db.Users.Where(f => f.Id == id).FirstOrDefault();

            if (User == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                try
                {
                    db.Users.Remove(User);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Could not remove user");
                }
            }
        }

        
        [AllowAnonymous]
        public HttpResponseMessage Put(string id, [FromBody]RegisterBindingModel model)
        {
            User user = db.Users.Where(f => f.Id == id).FirstOrDefault();
            var role = db.Roles.Where(r => r.Name == model.UserRole).FirstOrDefault();
            
            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No user found with id: " + id.ToString());
            }
            else
            {
                try
                {
                    if (role == null)
                    {

                        throw new System.ArgumentException("There is no userRole named: " + role.Name);
                    }
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
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
        }


        #region Helpers


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
