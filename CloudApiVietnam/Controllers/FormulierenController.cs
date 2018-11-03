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
    public class FormulierenController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET alle Formulieren
        public List<FormulierWithoutContents> Get()
        {
            var formulieren = db.Formulieren.ToList();
            List<FormulierWithoutContents> forms = new List<FormulierWithoutContents>();
            foreach(Formulieren form in formulieren)
            {
                //Zet Formulieren uit db om in formulieren die van de call worden verwacht
                FormulierWithoutContents formWithoutContent = new FormulierWithoutContents();
                formWithoutContent.id = form.Id;
                formWithoutContent.title = form.Name;
                formWithoutContent.region = form.Region;
                forms.Add(formWithoutContent);
            }
            return forms;
        }

        // GET specefiek Formulier
        public FormTemplateModel Get(int id)
        {
            var formulier = db.Formulieren.Where(f => f.Id == id).FirstOrDefault();
            FormTemplateModel form = new FormTemplateModel();
            TableInfo info = new TableInfo(); 
            info.id = id;
            info.region = formulier.Region;
            info.title = formulier.Name;
            form.tableInfo = info;
            try
            {
                //Als de JSON niet geconvert kan worden naar columns wordt de tableInfo zonder informatie terug gestuurd.
                var FormConverted = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Column>>(formulier.FormTemplate.ToString());
                form.columns = FormConverted;
            } catch
            {
                return form;
            }
            return form;
        }

        // POST een Formulier
        public void Post(FormulierenBindingModel formContentBindingModel)
        {
            Formulieren formulier = new Formulieren();
            formulier.Name = formContentBindingModel.Name;
            formulier.Region = formContentBindingModel.Region;
            try
            {
                //Probeer de JSON die is verstuurd om te zetten naar een SpecificForm (Template), geef anders een error
                formulier.FormTemplate = Newtonsoft.Json.JsonConvert.DeserializeObject<FormTemplateModel>(formContentBindingModel.FormTemplate);
            } catch
            {
                return;
            }
            
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
