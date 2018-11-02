using CloudApiVietnam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static CloudApiVietnam.Models.IvoCustomModels;

namespace CloudApiVietnam.Controllers
{
    [Authorize]
    public class FormulierenController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET alle Formulieren
        public List<Formulieren> Get()
        {
            var formulieren = db.Formulieren.ToList();
            foreach(Formulieren form in formulieren)
            {
                //Zet overige items op null om overbodige data te besparen
                form.FormTemplate = null;
                form.FormContent = null;
            }
            return formulieren;
        }

        // GET specefiek Formulier
        public SpecificForm Get(int id)
        {
            var formulier = db.Formulieren.Where(f => f.Id == id).FirstOrDefault();
            SpecificForm form = new SpecificForm();
            form.tableInfo.id = id;
            form.tableInfo.region = formulier.Region;
            form.tableInfo.title = formulier.Name;
            var FormConverted = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Column>>(formulier.FormTemplate);
            form.columns = FormConverted;
            return form;
        }

        // POST een Formulier
        public void Post(FormulierenBindingModel formContentBindingModel)
        {
            Formulieren formulier = new Formulieren();
            formulier.Name = formContentBindingModel.Name;
            formulier.Region = formContentBindingModel.Region;
            formulier.FormTemplate = formContentBindingModel.FormTemplate;
            
            db.Formulieren.Add(formulier);
            db.SaveChanges();
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            var formulier = db.Formulieren.Where(f => f.Id == id).FirstOrDefault();
            db.Formulieren.Remove(formulier);
            db.SaveChanges();

        }
    }
}
