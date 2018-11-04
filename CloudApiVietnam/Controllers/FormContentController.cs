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

        // GET alle Formulieren
        public List<Formulieren> Get()
        {
            var formulieren = db.Formulieren.ToList();
            return formulieren;
        }

        // GET specefiek Formulier
        public Formulieren Get(int id)
        {
            var formulier = db.Formulieren.Where(f => f.Id == id).FirstOrDefault();
            return formulier;
        }

        // POST een Formulier
        public IHttpActionResult Post(FormContentBindingModel formContentBindingModel)
        {
            FormContent formContent = new FormContent();
            formContent.Content = formContentBindingModel.Content;
            formContent.FormulierenId = formContentBindingModel.FormId;          

   
            db.FormContent.Add(formContent);
            db.SaveChanges();
            return Ok();
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public IHttpActionResult Delete(int id)
        {
            var formulier = db.Formulieren.Where(f => f.Id == id).FirstOrDefault();
            db.Formulieren.Remove(formulier);
            db.SaveChanges();
            return Ok();
        }
    }
}
