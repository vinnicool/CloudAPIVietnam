using CloudApiVietnam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CloudApiVietnam.Controllers
{
    [Authorize]
    public class FormContentController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET alle FormContent
        public HttpResponseMessage Get()
        {
            try
            {
                var formContent = db.FormContent.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, formContent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // GET specefiek FormContent
        public HttpResponseMessage Get(int id)
        {
            var formContent = db.FormContent.Where(f => f.Id == id).FirstOrDefault();
            if (formContent == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, formContent);
            }
        }

        // POST een FormContent
        public HttpResponseMessage Post(FormContentBindingModel formContentBindingModel)
        {
            try
            {
                FormContent formContent = new FormContent();
                formContent.Content = formContentBindingModel.Content;
                formContent.FormulierenId = formContentBindingModel.FormId;


                db.FormContent.Add(formContent);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        // PUT FormContent by Id
        public HttpResponseMessage Put(int id, [FromBody]FormContentBindingModel UpdateObject)
        {
            var formContent = db.FormContent.Where(f => f.Id == id).FirstOrDefault();

            if (formContent == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                formContent.FormulierenId = UpdateObject.FormId;
                formContent.Content = UpdateObject.Content;
                return Request.CreateResponse(HttpStatusCode.OK, formContent);
            }
        }

        // DELETE FormContent 
        public HttpResponseMessage Delete(int id)
        {
            var formContent = db.FormContent.Where(f => f.Id == id).FirstOrDefault();

            if (formContent == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                db.FormContent.Remove(formContent);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }


        }
    }
}
