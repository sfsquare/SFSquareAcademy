using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using PagedList;
using SFSAcademy.Models;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace SFSAcademy.Controllers
{
    public class UserController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: USERs
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var User = from s in db.USERS
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                User = User.Where(s => s.LAST_NAME.Contains(searchString)
                                       || s.FIRST_NAME.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    User = User.OrderByDescending(s => s.LAST_NAME);
                    break;
                case "Date":
                    User = User.OrderBy(s => s.CREATED_AT);
                    break;
                case "date_desc":
                    User = User.OrderByDescending(s => s.CREATED_AT);
                    break;
                default:  // Name ascending 
                    User = User.OrderBy(s => s.LAST_NAME);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(User.ToPagedList(pageNumber, pageSize));
            //return View(db.USERS.ToList());
        }

        // GET: USERs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER uSER = db.USERS.Find(id);
            if (uSER == null)
            {
                return HttpNotFound();
            }
            return View(uSER);
        }

        // GET: USERs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: USERs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,USRNAME,FIRST_NAME,LAST_NAME,EML,ADMIN_IND,STDNT_IND,EMP_IND,HASHED_PSWRD,SALT,RST_PSWRD_CODE,RST_PSWRD_CODE_UNTL,CREATED_AT,UPDATED_AT,PARNT_IND")] USER uSER)
        {
            if (ModelState.IsValid)
            {
                db.USERS.Add(uSER);
                db.SaveChanges();
                if (uSER.ADMIN_IND.Equals("Y"))
                {
                    foreach (var entity in db.USERS_ACCESS.Select(s => new { s.USRS_ID, s.CTL, s.ACTN, s.IS_ACCBLE }).Distinct().Where(a => a.USRS_ID.Equals(1)).ToList())
                    {
                        var UserAccess = new USERS_ACCESS() { USRS_ID = uSER.ID, CTL = entity.CTL, ACTN = entity.ACTN, IS_ACCBLE = entity.IS_ACCBLE };
                        db.USERS_ACCESS.Add(UserAccess);
                        db.SaveChanges();
                    }
                }
                else if(uSER.EMP_IND.Equals("Y"))
                {
                    foreach (var entity in db.USERS_ACCESS.Select(s => new { s.USRS_ID, s.CTL, s.ACTN, s.IS_ACCBLE }).Distinct().Where(a => a.USRS_ID.Equals(2)).ToList())
                    {
                        var UserAccess = new USERS_ACCESS() { USRS_ID = uSER.ID, CTL = entity.CTL, ACTN = entity.ACTN, IS_ACCBLE = entity.IS_ACCBLE };
                        db.USERS_ACCESS.Add(UserAccess);
                        db.SaveChanges();
                    }
                }
                else if (uSER.STDNT_IND.Equals("Y"))
                {
                    foreach (var entity in db.USERS_ACCESS.Select(s => new { s.USRS_ID, s.CTL, s.ACTN, s.IS_ACCBLE }).Distinct().Where(a => a.USRS_ID.Equals(4)).ToList())
                    {
                        var UserAccess = new USERS_ACCESS() { USRS_ID = uSER.ID, CTL = entity.CTL, ACTN = entity.ACTN, IS_ACCBLE = entity.IS_ACCBLE };
                        db.USERS_ACCESS.Add(UserAccess);
                        db.SaveChanges();
                    }
                }
                else if (uSER.PARNT_IND.Equals("Y"))
                {
                    foreach (var entity in db.USERS_ACCESS.Select(s => new { s.USRS_ID, s.CTL, s.ACTN, s.IS_ACCBLE }).Distinct().Where(a => a.USRS_ID.Equals(3)).ToList())
                    {
                        var UserAccess = new USERS_ACCESS() { USRS_ID = uSER.ID, CTL = entity.CTL, ACTN = entity.ACTN, IS_ACCBLE = entity.IS_ACCBLE };
                        db.USERS_ACCESS.Add(UserAccess);
                        db.SaveChanges();
                    }
                }
                else
                {
                    foreach (var entity in db.USERS_ACCESS.Select(s => new { s.USRS_ID, s.CTL, s.ACTN, s.IS_ACCBLE }).Distinct().Where(a => a.USRS_ID.Equals(3)).ToList())
                    {
                        var UserAccess = new USERS_ACCESS() { USRS_ID = uSER.ID, CTL = entity.CTL, ACTN = entity.ACTN, IS_ACCBLE = entity.IS_ACCBLE };
                        db.USERS_ACCESS.Add(UserAccess);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Edit_Privilege", "USERs", new { id = uSER.ID });

            }

            return View(uSER);
        }

        // GET: USERs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER uSER = db.USERS.Find(id);
            if (uSER == null)
            {
                return HttpNotFound();
            }
            return View(uSER);
        }

        // POST: USERs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,USRNAME,FIRST_NAME,LAST_NAME,EML,ADMIN_IND,STDNT_IND,EMP_IND,HASHED_PSWRD,SALT,RST_PSWRD_CODE,RST_PSWRD_CODE_UNTL,CREATED_AT,UPDATED_AT,PARNT_IND")] USER uSER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uSER).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.Notice = "User details updated in system successfully."; }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return View(uSER);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(uSER);
                }
                return RedirectToAction("Index", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
            }
            return View(uSER);
        }

        // GET: USERs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER uSER = db.USERS.Find(id);
            if (uSER == null)
            {
                return HttpNotFound();
            }
            return View(uSER);
        }

        // POST: USERs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = db.PRIVILEGES_USERS.Where(a => a.USER_ID == id).ToList();
            if(entity != null && entity.Count() != 0)
            {
                foreach (var item in entity)
                {
                    PRIVILEGES_USERS pRIVILEGEaCCCESS = db.PRIVILEGES_USERS.Find(item.ID);
                    db.PRIVILEGES_USERS.Remove(pRIVILEGEaCCCESS);
                }
            }            
            USER uSER = db.USERS.Find(id);
            db.USERS.Remove(uSER);
            try { db.SaveChanges(); ViewBag.Notice = "User Deleted from system."; }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                    }
                }
                return View(uSER);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat("There is an Employee or Student or Parent in the system using this User ID. Please delete the base information first.", "\n", e.InnerException.InnerException.Message);
                return View(uSER);
            }
            return RedirectToAction("Index", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Dashboard(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER uSER = db.USERS.Find(id);
            if (uSER == null)
            {
                return HttpNotFound();
            }
            return View(uSER);

        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            if (ModelState.IsValid)
            {
                if (user.IsValid(user.UserName, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, user.RememberMe);
                    int UserId = Convert.ToInt32(this.Session["UserId"]);
                    return RedirectToAction("Dashboard", "User", new { id = UserId });
                }
                else
                {
                    ViewBag.ErrorMessage = "Login data is incorrect!";
                }
            }
            return View(user);
        }

        public ActionResult Forgot_Password()
        {
            var Config = new Models.Configuration();
            ViewBag.network_state = Config.find_by_config_key("NetworkState");
            //ViewBag.network_state = "Active";
            return View();

        }
        [HttpPost]
        public ActionResult Forgot_Password(USER user)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Thank you. Your request is saved. Please contact Admin on 9967803589 in the mean time.";
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

  /*      public ActionResult Edit_Privilege(int? id, string Calling_Method)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var uSERaACCESS = (from C in db.USERS_ACCESS
                               where C.USRS_ID == id
                               select new Models.SelectUserAccess { AccessList = C, Selected= (C.IS_ACCBLE == true ? true : false) }).OrderBy(g => g.AccessList.ID).ToList();


            var uSERaCCESfINAL = (from u in uSERaACCESS
                                      //where conditions or joins with other tables to be included here
                                  group u by new { u.AccessList.LIST_ITEM, u.AccessList.LVL_1_MENU } into grp
                                  let MaxOrderDatePerPerson = grp.Max(g => g.AccessList.ID)
                                  from p in grp
                                  where p.AccessList.ID == MaxOrderDatePerPerson
                                  select p).ToList();
            ViewBag.Calling_Method = Calling_Method;
            return View(uSERaCCESfINAL);

        }*/

        public ActionResult Edit_Privilege(int? id, string Calling_Method)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var uSERpRIVILEGE = (from pr in db.PRIVILEGES
                                 join prusr in db.PRIVILEGES_USERS.Where(x=>x.USER_ID == id) on pr.ID equals prusr.PRIVILEGE_ID into gi
                                 from subgi in gi.DefaultIfEmpty()
                               select new Models.SelectUserPrivilage { PrivilageList = pr,USRS_ID = id, Selected = (subgi == null ? false : true), IsActive = (pr.IS_ACT == true ? true : false) }).OrderBy(g => g.PrivilageList.ID).ToList();


            var uSERpRIVILEGEfINAL = (from u in uSERpRIVILEGE
                                          //where conditions or joins with other tables to be included here
                                      group u by new { u.PrivilageList.PRIVILEGE_TAG, u.PrivilageList.NAME } into grp
                                  let MaxPrIDPerPerson = grp.Max(g => g.PrivilageList.ID)
                                  from p in grp
                                  where p.PrivilageList.ID == MaxPrIDPerPerson
                                      select p).ToList();
            ViewBag.Calling_Method = Calling_Method;
            USER User = db.USERS.Find(id);
            ViewData["User"] = User;
            return View(uSERpRIVILEGEfINAL);

        }


        // POST: USERs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Privilege(IList<SelectUserPrivilage> model, string Calling_Method)
        {
            if (ModelState.IsValid)
            {
                int? UserId = model.FirstOrDefault().USRS_ID;
                foreach (SelectUserPrivilage item in model)
                {
                    if (item.Selected && item.IsActive)
                    {
                        var uPRIVILEGEVal = db.PRIVILEGES_USERS.Where(x => x.PRIVILEGE_ID == item.PrivilageList.ID && x.USER_ID == item.USRS_ID).ToList();
                        if(uPRIVILEGEVal == null || uPRIVILEGEVal.Count() == 0)
                        {
                            var NewPrivilege = new PRIVILEGES_USERS()
                            {
                                PRIVILEGE_ID = item.PrivilageList.ID,
                                USER_ID = item.USRS_ID
                            };
                            db.PRIVILEGES_USERS.Add(NewPrivilege);
                        }
                    }
                    else
                    {
                        var uPRIVILEGEVal = db.PRIVILEGES_USERS.Where(x => x.PRIVILEGE_ID == item.PrivilageList.ID && x.USER_ID == item.USRS_ID).ToList();
                        if (uPRIVILEGEVal != null && uPRIVILEGEVal.Count() != 0)
                        {
                            foreach(var item2 in uPRIVILEGEVal)
                            {
                                PRIVILEGES_USERS PrUserToRemove = db.PRIVILEGES_USERS.Find(item2.ID);
                                db.PRIVILEGES_USERS.Remove(PrUserToRemove);
                            }
                        }
                    }
                }
                db.SaveChanges();
                if (Calling_Method == "Employee")
                {
                    int Employee_ID = db.EMPLOYEEs.Where(x => x.USRID == UserId).SingleOrDefault().ID;
                    return RedirectToAction("Admission4", "Employee", new { Emp_id = Employee_ID });
                }
            }
            return RedirectToAction("Index");
        }
    }
}
