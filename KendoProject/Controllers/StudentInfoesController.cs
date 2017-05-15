using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Xml;
using System.Web.Script.Serialization;
using System.IO;
using KendoProject.Models;
using StudentBasic.ModelView;

namespace KendoProject.Controllers
{
    public class StudentInfoesController : Controller
    {
        private StudentEntities db = new StudentEntities();

        // GET: StudentInfoes
        public ActionResult Index()
        {
            var studentInfoes = db.StudentInfoes.Include(s => s.Department);
            return View(studentInfoes.ToList());
        }

        // GET: StudentInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentInfo studentInfo = db.StudentInfoes.Find(id);
            if (studentInfo == null)
            {
                return HttpNotFound();
            }
            return View(studentInfo);
        }

        // GET: StudentInfoes/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        // POST: StudentInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,DepartmentID,Year")] StudentInfo studentInfo)
        {
            if (ModelState.IsValid)
            {
                db.StudentInfoes.Add(studentInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", studentInfo.DepartmentID);
            return View(studentInfo);
        }

        // GET: StudentInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentInfo studentInfo = db.StudentInfoes.Find(id);
            if (studentInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", studentInfo.DepartmentID);
            return View(studentInfo);
        }

        // POST: StudentInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,DepartmentID,Year")] StudentInfo studentInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", studentInfo.DepartmentID);
            return View(studentInfo);
        }

        // GET: StudentInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentInfo studentInfo = db.StudentInfoes.Find(id);
            if (studentInfo == null)
            {
                return HttpNotFound();
            }
            return View(studentInfo);
        }

        // POST: StudentInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentInfo studentInfo = db.StudentInfoes.Find(id);
            db.StudentInfoes.Remove(studentInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public string Id { get; set; }
        public JsonResult ClientNameExist(int? ID, string Name)
        {
            bool exsits = (from m in db.StudentInfoes.Where(m => ID.HasValue ? (m.ID != ID && m.Name == Name) : (m.Name == Name)) select m).Any();
            if (exsits) { return Json(false, JsonRequestBehavior.AllowGet); }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }



        public ActionResult ProductView1(int? deptid, [DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result;
            if (deptid == null)
            {
                IQueryable<StudentInfoModelView> model = (from m in db.StudentInfoes
                                                          select new StudentInfoModelView
                                                          {
                                                              ID = m.ID,
                                                              Name = m.Name,
                                                              DepartmentID = m.DepartmentID,
                                                              dptName = db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentName).FirstOrDefault(),
                                                              Year = m.Year

                                                          });
                result = model.ToDataSourceResult(request);
            }
            else
            {
                IQueryable<StudentInfoModelView> model = (from m in db.StudentInfoes
                                                          select new StudentInfoModelView
                                                          {
                                                              ID = m.ID,
                                                              Name = m.Name,
                                                              DepartmentID = db.Departments.Where(n => n.DepartmentID == deptid).Select(s => s.DepartmentID).FirstOrDefault(),
                                                              dptName = db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentName).FirstOrDefault(),
                                                              Year = m.Year

                                                          });
                result = model.ToDataSourceResult(request);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowGridView()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StudentDestroy([DataSourceRequest]DataSourceRequest request, StudentInfo student)
        {
            if (ModelState.IsValid)
            {
                StudentInfo d = db.StudentInfoes.Find(student.ID);
                db.StudentInfoes.Remove(d);
                db.SaveChanges();
            }
            return Json(new[] { student }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult StudentReport()
        {
            //var countStd = from m in db.StudentInfoes
            //               join n in db.Departments on m.DepartmentID equals n.DepartmentID
            //               group m by m.DepartmentID into g
            //               select g.Count() ;
            //var Std = from m in db.StudentInfoes
            //          join n in db.Departments on m.DepartmentID equals n.DepartmentID
            //          group n by new { n.DepartmentID , n.DepartmentName };




            //var studentCount = db.StudentInfoes.Where(m => m.DepartmentID).Count();
            // ViewBag.DepartmentID = Std;
            return View();
        }

        public ActionResult DepartmentPartialView(int? deptid)
        {
            if (deptid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var studentCount = db.StudentInfoes.Where(m => m.DepartmentID == deptid).Count();
                Department data = db.Departments.Where(m => m.DepartmentID == deptid).FirstOrDefault();
                StudentInfoModelView model = new StudentInfoModelView();

                model.DepartmentID = data.DepartmentID;
                model.dptName = data.DepartmentName;
                model.countStudent = studentCount;
                model.departmentCode = Convert.ToInt32(data.DepartmentCode);

                // model.dptName = db.Departments.Where(n => n.DepartmentID == model.DepartmentID).Select(s => s.DepartmentName).FirstOrDefault();
                return PartialView(model);



            }

        }

        public ActionResult StudentCreatePopUp(int? id)
        {
            StudentInfoModelView model = new StudentInfoModelView();
            if (id != null)
            {
                StudentInfo student = db.StudentInfoes.Where(m => m.ID == id).FirstOrDefault();
                model.ID = student.ID;
                model.Name = student.Name;
                model.DepartmentID = student.DepartmentID;
                model.Year = student.Year;
                ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
            }
            else
            {

                ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName");
            }
            return PartialView(model);
        }


        public JsonResult StudentSave(int? ID, StudentInfoModelView model)
        {

            if (ModelState.IsValid)
            {
                StudentInfo student = new StudentInfo();
                if (model.ID > 0)
                {
                    student = db.StudentInfoes.Find(model.ID);
                }
                student.Name = model.Name;
                if (model.ID > 0)
                {
                    student.DepartmentID = model.DepartmentID;
                    student.Year = model.Year;
                    db.StudentInfoes.Attach(student);
                    db.Entry(student).State = EntityState.Modified;
                }
                else
                {
                    student.DepartmentID = model.DepartmentID;
                    student.Year = model.Year;
                    db.StudentInfoes.Add(student);
                }
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }


            //if (ModelState.IsValid)
            //{
            //    StudentInfo student = new StudentInfo();
            //    if(ID!=null)
            //    {
            //        student = db.StudentInfoes.Find(model.ID);
            //    }
            //    student.Name = model.Name;
            //    student.DepartmentID = model.DepartmentID;
            //    student.Year = model.Year;
            //    if (ID != null)
            //    {
            //        db.Entry(student).State = EntityState.Modified;                  
            //    }
            //    else
            //    {
            //        db.StudentInfoes.Add(student);        

            //    }
            //    db.SaveChanges();
            //    return Json("Success", JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json("Error", JsonRequestBehavior.AllowGet);
            //}

        }

        public ActionResult studentDetails(int? id)
        {
            StudentInfoModelView model = new StudentInfoModelView();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StudentInfo student = db.StudentInfoes.Where(m => m.ID == id).FirstOrDefault();
            Department dpt = db.Departments.Where(m => m.DepartmentID == student.DepartmentID).First();
            model.ID = student.ID;
            model.Name = student.Name;
            model.DepartmentID = student.DepartmentID;
            model.Year = student.Year;
            model.dptName = dpt.DepartmentName;
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName");
            return PartialView(model);
        }

        public ActionResult DepartmentList()
        {

            var Std = from m in db.StudentInfoes
                      join n in db.Departments on m.DepartmentID equals n.DepartmentID
                      group n by new { n.DepartmentID, n.DepartmentName } into g
                      select new StudentInfoModelView { DepartmentID = g.Key.DepartmentID, dptName = g.Key.DepartmentName };
            //var deptinfo = db.Departments.Select(c => new StudentInfoModelView
            //            {
            //                DepartmentID = c.DepartmentID,
            //                dptName = c.DepartmentName
            //            })
            //            .OrderBy(e => e.DepartmentID);

            return Json(Std, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult ToolbarRead()
        //{

        //    //Department dpt = db.Departments.Where(m=> m.DepartmentID == deptid).First();

        //    //Department model = new Department();
        //    //model.DepartmentID = dpt.DepartmentID;
        //    //model.DepartmentName = dpt.DepartmentName;
        //    //return Json(model, JsonRequestBehavior.AllowGet);


        //    //var department = new Department();
        //    //var categories = department.categories
        //    //            .Select(c => new CategoryViewModel
        //    //            {
        //    //                CategoryID = c.CategoryID,
        //    //                CategoryName = c.CategoryName
        //    //            })
        //    //            .OrderBy(e => e.CategoryName);

        //    //return Json(categories, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult DynamicField()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DynamicField(StudentInfoModelView model)
        {


            Department department = new Department();

            if (model.alldept != "")
            {
                string[] deptlist = model.alldept.Split(',');
                for (int i = 0; i < deptlist.Length - 1; i++)
                {
                    department.DepartmentName = deptlist[i].Split('|')[0];
                    department.DepartmentCode = Convert.ToInt32(deptlist[i].Split('|')[1]);
                    db.Departments.Add(department);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string errorMsg = ex.Message.ToString();
                    }
                }
            }
            return RedirectToAction("DynamicFieldShow");
            // return View("DynamicField",model);
        }

        public ActionResult DynamicFieldShow()
        {
            //Department department = new Department();
            //List<Department> model = db.Departments.ToList();


            IEnumerable<StudentInfoModelView> model = (from m in db.Departments.ToArray()
                                                       select new StudentInfoModelView
                                                       {
                                                           DepartmentID = m.DepartmentID,
                                                           departmentCode = (int)m.DepartmentCode,
                                                           dptName = m.DepartmentName

                                                       });


            return View(model);
            //return Json(result, JsonRequestBehavior.AllowGet);


            //StudentInfoModelView info = new StudentInfoModelView();
            //List<StudentInfoModelView> model = db.Departments.ToList();
            //StudentInfoModelView model = new StudentInfoModelView();
            //model.DepartmentID = department.DepartmentID;
            //model.dptName = department.DepartmentName;
            //model.departmentCode = Convert.ToInt32(department.DepartmentCode);
            //ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentCode", "DepartmentName");       
        }

        public ActionResult DynamicFieldEdit()
        {
            var model = (from m in db.Departments.ToArray()
                         select new Department
                         {
                             DepartmentID = m.DepartmentID,
                             DepartmentCode = (int)m.DepartmentCode,
                             DepartmentName = m.DepartmentName

                         });
            ViewBag.Department = model;

            return PartialView();
        }

        [HttpPost]
        public ActionResult DynamicFieldShow(StudentInfoModelView model)
        {
            Department department = new Department();

            if (model.alldept != null)
            {
                string[] deptlist = model.alldept.Split(',');
                for (int i = 0; i < deptlist.Length - 1; i++)
                {
                    if (Convert.ToInt32(deptlist[i].Split('|')[2]) == 0)
                    {
                        department.DepartmentName = deptlist[i].Split('|')[0];
                        department.DepartmentCode = Convert.ToInt32(deptlist[i].Split('|')[1]);
                        db.Departments.Add(department);
                    }
                    else
                    {
                        department = db.Departments.Find(Convert.ToInt32(deptlist[i].Split('|')[2]));
                        department.DepartmentName = deptlist[i].Split('|')[0];
                        department.DepartmentCode = Convert.ToInt32(deptlist[i].Split('|')[1]);
                        db.Entry(department).State = EntityState.Modified;
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string errorMsg = ex.Message.ToString();
                    }
                }
            }
            return RedirectToAction("DynamicFieldShow");
        }

        public ActionResult TableView()
        {
            IEnumerable<StudentInfoModelView> model = (from m in db.StudentInfoes.ToArray()
                                                       select new StudentInfoModelView
                                                       {
                                                           ID = m.ID,
                                                           Name = m.Name,
                                                           DepartmentID = m.DepartmentID,
                                                           dptName = db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentName).FirstOrDefault(),
                                                           Year = m.Year,
                                                           departmentCode = Convert.ToInt32(db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentCode).FirstOrDefault()),



                                                       });
            ViewBag.Student = model;
            ViewBag.AllStudent = model;



            return View();

        }

        public ActionResult PartialTableView(String Name)
        {
            IEnumerable<StudentInfoModelView> model, model2;
            if (string.IsNullOrEmpty(Name))
            {
                model = (from m in db.StudentInfoes.ToArray()
                         select new StudentInfoModelView
                         {
                             ID = m.ID,
                             Name = m.Name,
                             DepartmentID = m.DepartmentID,
                             dptName = db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentName).FirstOrDefault(),
                             Year = m.Year,
                             departmentCode = Convert.ToInt32(db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentCode).FirstOrDefault()),



                         });
                ViewBag.Student = model;
                ViewBag.AllStudent = model;
            }
            else
            {

                model2 = (from m in db.StudentInfoes.ToArray()
                          select new StudentInfoModelView
                          {
                              ID = m.ID,
                              Name = m.Name,
                              DepartmentID = m.DepartmentID,
                              dptName = db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentName).FirstOrDefault(),
                              Year = m.Year,
                              departmentCode = Convert.ToInt32(db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentCode).FirstOrDefault()),



                          });
                ViewBag.AllStudent = model2;

                model = (from m in db.StudentInfoes.Where(m => m.Name.Contains(Name))
                         select new StudentInfoModelView
                         {
                             ID = m.ID,
                             Name = m.Name,
                             DepartmentID = m.DepartmentID,
                             dptName = db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentName).FirstOrDefault(),
                             Year = m.Year,
                             departmentCode = (int)db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentCode).FirstOrDefault(),

                         });
                ViewBag.Student = model;
            }
            return PartialView();
        }

        public ActionResult PictureUpload()
        {
            return View();
        }
        private IEnumerable<string> Getfileinfo(IEnumerable<HttpPostedFileBase> files)
        {
            IEnumerable<string> result = null;
            IEnumerable<string> extension = from a in files
                                            where a != null
                                            select string.Format("{0}", Path.GetExtension(a.FileName));
            foreach (var type in extension)
            {
                if (type.Equals(".jpeg") || type.Equals(".jpg") || type.Equals(".png"))
                {
                    result = from a in files
                             where a != null
                             select string.Format("{0} {1} bytes)", Path.GetFileName(a.FileName), a.ContentLength);
                }
                else result = null;
            }





            return result;
        }

        [HttpPost]
        public ActionResult Submit(IEnumerable<HttpPostedFileBase> files)
        {
            IEnumerable<string> result;
            if (files != null)
            {
                result = Getfileinfo(files);
                Session["UploadedFile"] = result;
            }

            return View("PictureResult");
        }

        public ActionResult PictureResult()
        {
            return View();
        }

        public ActionResult DragDrop()
        {
            IEnumerable<StudentInfoModelView> model;

            model = (from m in db.StudentInfoes.ToArray()
                     select new StudentInfoModelView
                     {
                         ID = m.ID,
                         Name = m.Name,
                         DepartmentID = m.DepartmentID,
                         dptName = db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentName).FirstOrDefault(),
                         Year = m.Year,
                         departmentCode = Convert.ToInt32(db.Departments.Where(n => n.DepartmentID == m.DepartmentID).Select(s => s.DepartmentCode).FirstOrDefault()),
                     });
            ViewBag.Student = model;

            return View();
        }

        public ActionResult TableViewEdit(int? id)
        {

            StudentInfoModelView model = new StudentInfoModelView();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StudentInfo student = db.StudentInfoes.Where(m => m.ID == id).FirstOrDefault();
            Department dpt = db.Departments.Where(m => m.DepartmentID == student.DepartmentID).First();
            model.Name = student.Name;
            model.DepartmentID = student.DepartmentID;
            model.Year = student.Year;
            model.dptName = dpt.DepartmentName;
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName");
            

            return View(model);

        }

        [HttpPost]
        public ActionResult TableViewEdit(StudentInfoModelView model)
        {
            StudentInfo student = db.StudentInfoes.Where(m => m.ID == model.ID).FirstOrDefault();
            student.Name = model.Name;
            student.DepartmentID = model.DepartmentID;
            student.Year = model.Year;
        
             db.Entry(student).State = EntityState.Modified;
             db.SaveChanges();


             return Redirect("TableView/StudentInfoes");
        }

        public ActionResult TableViewDelete(int? id)
        {
            StudentInfo student = db.StudentInfoes.Find(id);
            
            db.StudentInfoes.Remove(student);
            db.SaveChanges();

            return RedirectToAction("TableView/StudentInfoes");
        }

        public ActionResult TableViewDetail(int? id)
        {
            StudentInfoModelView model = new StudentInfoModelView();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StudentInfo student = db.StudentInfoes.Where(m => m.ID == id).FirstOrDefault();
            Department dpt = db.Departments.Where(m => m.DepartmentID == student.DepartmentID).First();
            model.Name = student.Name;
            model.DepartmentID = student.DepartmentID;
            model.Year = student.Year;
            model.dptName = dpt.DepartmentName;
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName");


            return View(model);
        }
    }
}