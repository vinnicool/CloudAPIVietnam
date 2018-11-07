using CloudApiVietnam.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public HttpResponseMessage Get()
        {
            try
            {
                var formulieren = db.Formulieren.Include("FormContent").ToList();
                return Request.CreateResponse(HttpStatusCode.OK, formulieren);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // GET specifiek Formulier
        public HttpResponseMessage Get(int id)
        {
            var formulier = db.Formulieren.Include("FormContent").Where(f => f.Id == id).FirstOrDefault();
            if (formulier == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, formulier);
            }
        }


        // POST een Formulier
        public HttpResponseMessage Post(FormulierenBindingModel formulierenBindingModel)
        {
            try
            {
                IsJSON isJson = IsValidJson(formulierenBindingModel.FormTemplate); // Check of JSON klopt en maak resultaat object
                if (!isJson.Status) // als resultaat object status fals is return error
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "JSON in 'content' is not correct JSON: " + isJson.Error);
                }

                Formulieren formulier = new Formulieren();
                formulier.Name = formulierenBindingModel.Name;
                formulier.Region = formulierenBindingModel.Region;
                formulier.FormTemplate = formulierenBindingModel.FormTemplate;

                db.Formulieren.Add(formulier);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // PUT api/values/5
        public HttpResponseMessage Put(int id, [FromBody]FormulierenBindingModel UpdateObject)
        {
            var form = db.Formulieren.Where(f => f.Id == id).FirstOrDefault();

            if (form == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No form found with id: " + id.ToString());
            }

            var formContentList = db.FormContent.Where(s => s.FormulierenId == id).Select(p => p.Content).ToList();

            List<JArray> formContentArray = new List<JArray>();

            foreach (var formContent in formContentList)
            {
                formContentArray.Add(JArray.Parse(formContent));
            }

            form.Name = UpdateObject.Name;
            form.Region = UpdateObject.Region;
            form.FormTemplate = UpdateObject.FormTemplate;
            var formTemplate = JArray.Parse(form.FormTemplate);
 
            foreach (var formContent in formContentArray.ToList())
            {
                bool changed = false;
                List<string> unchangedTokens = new List<string>();
                foreach (JObject formContentToken in formContent.ToList())
                {
                    List<JProperty> formContentProperty = formContentToken.Properties().ToList();

                    foreach (JObject formTemplateToken in formTemplate.ToList())
                    {
                        List<JProperty> formTemplateProperty = formTemplateToken.Properties().ToList();

                        if (formContentProperty.First().Name != formTemplateProperty.First().Name && !changed && !unchangedTokens.Contains(formContentProperty.First().Name) && !unchangedTokens.Contains(formTemplateProperty.First().Name))
                        {
                            if (formContent.Count == formTemplate.Count)
                            {
                                formContentToken[formTemplateProperty.First().Name] = formContentProperty.First().Value;
                                formContentToken.Remove(formContentProperty.First().Name);
                            }
                            if (formContent.Count < formTemplate.Count)
                            {
                                formContent.Add(formTemplateToken);
                            }
                            if (formContent.Count > formTemplate.Count)
                            {
                                formContentToken.Remove();
                            }
                            changed = true;

                        }
                        else if (formContentProperty.First().Name == formTemplateProperty.First().Name)
                        {
                            unchangedTokens.Add(formTemplateProperty.First().Name);
                        }
                    }
                    formContentProperty.Remove(formContentProperty.First());
                }
                //db.FormContent.
                //db.Entry(formContent).State = EntityState.Modified;
                //db.SaveChanges();
            }
            

            return Request.CreateResponse(HttpStatusCode.OK, form);
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The paramaters doesn't contain a type or the type isn't known.");
        }

 
        public HttpResponseMessage Delete(int id)
        {
            var formulier = db.FormContent.Where(f => f.Id == id).FirstOrDefault();

            if (formulier == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Form found with id: " + id.ToString());
            }
            else
            {
                try
                {
                    db.FormContent.Remove(formulier);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
        }

        private static IsJSON IsValidJson(string strInput)
        {
            IsJSON result = new IsJSON();
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    result.Status = true;
                    return result;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    result.Error = jex.Message;
                    result.Status = false;
                    return result;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    result.Error = ex.ToString();
                    result.Status = false;
                    return result;
                }
            }
            else
            {
                result.Status = false;
                result.Error = "JSON doesn't start or and with with '{/}' or '[/]' ";
                return result;
            }
        }
    }
}
