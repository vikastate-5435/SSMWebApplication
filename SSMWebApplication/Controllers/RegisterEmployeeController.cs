using Newtonsoft.Json;
using SSMWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace SSMWebApplication.Controllers
{
    public class RegisterEmployeeController : Controller
    {
        // GET: RegisterEmployee
        public ActionResult Index()
        {
            List<EmployeeRegister> lstRec = new List<EmployeeRegister>();
            DataSet ds = new DataSet();
            ds.ReadXml(HttpContext.Server.MapPath("~/XML/RecordList.xml"));
            DataView dvPrograms;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][1] != DBNull.Value)
            {
                dvPrograms = ds.Tables[0].DefaultView;
                dvPrograms.Sort = "EmpId";
                foreach (DataRowView dr in dvPrograms)
                {
                    EmployeeRegister model = new EmployeeRegister();
                    model.EmpId = Convert.ToInt32(dr[0]);
                    model.Name = Convert.ToString(dr[1]);
                    model.Gender = Convert.ToString(dr[2]);
                    model.Qualification = Convert.ToString(dr[3]);
                    model.Age = Convert.ToInt32(dr[4]);
                    model.City = Convert.ToInt32(dr[5]);
                    model.State = Convert.ToInt32(dr[6]);
                    model.Email = Convert.ToString(dr[7]);
                    model.Password = Convert.ToString(dr[8]);
                    model.ConfirmPassword = Convert.ToString(dr[9]);
                    model.ProfilePic = Convert.ToString(dr[10]);
                    lstRec.Add(model);
                }
            }
            return View(lstRec);
        }

        public Dictionary<int, string> QualificationsList()
        {
            var dList = new Dictionary<int, string>();
            dList.Add(1, "BA");
            dList.Add(2, "MA");
            dList.Add(3, "BBA");
            dList.Add(4, "MBA");
            dList.Add(5, "BCA");
            dList.Add(6, "MCA");
            dList.Add(7, "BSC");
            dList.Add(8, "MSC");

            return dList;
        }

        [HttpGet]
        public ActionResult WriteToXml()
        {
            var stateList = getAllStates();
            ViewBag.State = stateList;
            return View();
        }
        
        [HttpPost]
            public ActionResult WriteToXml(EmployeeRegister oModel,HttpPostedFileBase ProfilePhoto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ProfilePhoto != null)
                    {
                        //virtual

                        string fileName = Path.GetFileNameWithoutExtension(ProfilePhoto.FileName);
                        string extension = Path.GetExtension(ProfilePhoto.FileName);
                        var supportedTypes = new[] { ".png", ".jpg", ".jpeg", ".gif", ".tiff" };


                        if (!supportedTypes.Contains(extension))
                        {
                            ModelState.AddModelError("", "Not a valid image file format that you have try to upload.");

                        }

                        fileName = fileName + DateTime.Now.ToString("yyMMddssfff") + extension;
                          oModel.ProfilePic = fileName;
                        fileName = Path.Combine(Server.MapPath("~/Content/ProfilePhoto/"), fileName);

                         }


                    CreateOrUpdateXML(oModel);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Something went wrong in XML Generation:" + ex.Message);
                }
            }
            var stateList = getAllStates();

            ViewBag.State = stateList;
            return View();
        }

        public JsonResult getAllStates()
        {
            List<States> lstStates = new List<States>();
            DataSet ds = new DataSet();
            ds.ReadXml(HttpContext.Server.MapPath("~/XML/StatesList.xml"));
            DataView dvPrograms;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][1] != DBNull.Value)
            {
                dvPrograms = ds.Tables[0].DefaultView;
                dvPrograms.Sort = "StateId";
                foreach (DataRowView dr in dvPrograms)
                {
                    States model = new States();
                    model.StateId = Convert.ToInt32(dr[0]);
                    model.StateName = Convert.ToString(dr[1]).Trim();
                    lstStates.Add(model);
                }
            }

            var data = JsonConvert.SerializeObject(lstStates);
            return Json(data,JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCitiesList(int stateId)
        {
            List<Cities> lstCities = new List<Cities>();
            DataSet ds = new DataSet();
            ds.ReadXml(HttpContext.Server.MapPath("~/XML/CitiesList.xml"));
            DataView dvPrograms;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][1] != DBNull.Value)
            {
                dvPrograms = ds.Tables[0].DefaultView;
                dvPrograms.Sort = "CityId";
                foreach (DataRowView dr in dvPrograms)
                {
                    Cities model = new Cities();
                    model.CityId = Convert.ToInt32(dr[0]);
                    model.CityName = Convert.ToString(dr[1]);
                    model.StateId = Convert.ToInt32(dr[2]);
                    lstCities.Add(model);
                }
            }
            var clist = (from c in lstCities.Where(c => c.StateId == stateId)
                         select new
                         {
                             c.CityId,
                             c.CityName
                         }).ToList();
            var data = JsonConvert.SerializeObject(clist);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WriteRecordToXml(List<EmployeeRegister> oModelList)
        {
            try
            {
                foreach (EmployeeRegister oModel in oModelList)
                {
                    CreateOrUpdateXML(oModel);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Something went wrong in XML Generation:" + ex.Message);
            }
            return View();
        }

        public void CreateOrUpdateXML(EmployeeRegister mdl)
        {

            List<EmployeeRegister> rLists = AllRecords();
            bool iSavail = (rLists.Where(rd => rd.EmpId == mdl.EmpId).Any());

            if (iSavail)
            {
                XDocument xmlDoc = XDocument.Load(HttpContext.Server.MapPath("~/XML/RecordList.xml"));
                var items = (from item in xmlDoc.Descendants("Employee") select item).ToList();
                XElement selected = items.Where(p => p.Element("EmpId").Value == mdl.EmpId.ToString()).FirstOrDefault();
                selected.Remove();
                xmlDoc.Save(HttpContext.Server.MapPath("~/XML/RecordList.xml"));
                xmlDoc.Element("Employees").Add(new XElement("Employee",
                                new XElement("EmpId", mdl.EmpId),
                         new XElement("Name", mdl.Name),
                          new XElement("Gender", mdl.Gender),
                          new XElement("Qualification", mdl.Qualification),
                          new XElement("Age", mdl.Age),
                          new XElement("City", mdl.City),
                          new XElement("State", mdl.State),
                          new XElement("Email", mdl.Email),
                          new XElement("Password", mdl.Password),
                          new XElement("ConfirmPassword", mdl.ConfirmPassword),
                          new XElement("ProfilePic", mdl.ProfilePic)
             ));
                xmlDoc.Save(HttpContext.Server.MapPath("~/XML/RecordList.xml"));
            }
            else
            {
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.Load(HttpContext.Server.MapPath("~/XML/RecordList.xml"));
                XmlNodeList nodelist = oXmlDocument.GetElementsByTagName("Employee");
                XDocument xmlDoc = XDocument.Load(HttpContext.Server.MapPath("~/XML/RecordList.xml"));
                xmlDoc.Element("Employees").Add(new XElement("Employee",
                                  new XElement("EmpId", mdl.EmpId),
                           new XElement("Name", mdl.Name),
                            new XElement("Gender", mdl.Gender),
                            new XElement("Qualification", mdl.Qualification),
                            new XElement("Age", mdl.Age),
                            new XElement("City", mdl.City),
                            new XElement("State", mdl.State),
                            new XElement("Email", mdl.Email),
                            new XElement("Password", mdl.Password),
                            new XElement("ConfirmPassword", mdl.ConfirmPassword),
                            new XElement("ProfilePic", mdl.ProfilePic)
                       ));
                xmlDoc.Save(HttpContext.Server.MapPath("~/XML/RecordList.xml"));
            }
        }

        public List<EmployeeRegister> AllRecords()
        {
            List<EmployeeRegister> lstRecords = new List<EmployeeRegister>();
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(HttpContext.Server.MapPath("~/XML/RecordList.xml"));
                DataView dvPrograms;
                dvPrograms = ds.Tables[0].DefaultView;
                dvPrograms.Sort = "EmpId";
                foreach (DataRowView dr in dvPrograms)
                {
                    EmployeeRegister model = new EmployeeRegister();
                    model.EmpId = Convert.ToInt32(dr[0]);
                    model.Name = Convert.ToString(dr[1]);
                    model.Gender = Convert.ToString(dr[2]);
                    model.Qualification = Convert.ToString(dr[3]);
                    model.Age = Convert.ToInt32(dr[4]);
                    model.City = Convert.ToInt32(dr[5]);
                    model.State = Convert.ToInt32(dr[6]);
                    model.Email = Convert.ToString(dr[7]);
                    model.Password = Convert.ToString(dr[8]);
                    model.ConfirmPassword = Convert.ToString(dr[9]);
                    model.ProfilePic = Convert.ToString(dr[10]);
                    lstRecords.Add(model);
                }
            }
            catch (Exception ex)
            {
                return lstRecords;
            }
            return lstRecords;

        }

        public List<EmployeeRegister> GetRecordDetails(int Id)
        {
            List<EmployeeRegister> Record = new List<EmployeeRegister>();
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(HttpContext.Server.MapPath("~/XML/RecordList.xml"));
                DataView dvPrograms;
                dvPrograms = ds.Tables[0].DefaultView;
                dvPrograms.Sort = "EmpId";
                foreach (DataRowView dr in dvPrograms)
                {
                    EmployeeRegister model = new EmployeeRegister();
                    model.EmpId = Convert.ToInt32(dr[0]);
                    model.Name = Convert.ToString(dr[1]);
                    model.Gender = Convert.ToString(dr[2]);
                    model.Qualification = Convert.ToString(dr[3]);
                    model.Age = Convert.ToInt32(dr[4]);
                    model.City = Convert.ToInt32(dr[5]);
                    model.State = Convert.ToInt32(dr[6]);
                    model.Email = Convert.ToString(dr[7]);
                    model.Password = Convert.ToString(dr[8]);
                    model.ConfirmPassword = Convert.ToString(dr[9]);
                    model.ProfilePic = Convert.ToString(dr[10]);
                    Record.Add(model);
                }
            }
            catch (Exception ex)
            {
                return Record;
            }
            return Record.FindAll(o => o.EmpId.Equals(Id));

        }

    }
}