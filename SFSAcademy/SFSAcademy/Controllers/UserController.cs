using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using PagedList;
using SFSAcademy;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

namespace SFSAcademy.Controllers
{
    public class UserController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: USERs
        public ActionResult Index(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var User = (from s in db.USERS.Where(x => x.IS_DEL == false)
                       select s).OrderBy(x=>x.USRNAME).ToList();

            return View(User);
        }

        public ActionResult All(string sortOrder, string currentFilter, string searchString, int? page, string Notice, string ErrorMessage)
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

            var User = from s in db.USERS.Where(x => x.IS_DEL == false)
                       select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                User = User.Where(s => s.ROLE.Contains(searchString));
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

            int pageSize = 100;
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
        public ActionResult Create(User uSER)
        {
            if (ModelState.IsValid)
            {
                //uSER.CREATED_AT = System.DateTime.Now;
                //uSER.UPDATED_AT = System.DateTime.Now;
                //string FullName = Regex.Replace(string.Concat(uSER.FIRST_NAME, uSER.LAST_NAME), @"\s", "");
                //uSER.USRNAME = FullName;
                //uSER.HASHED_PSWRD = string.Concat(uSER.FIRST_NAME, uSER.LAST_NAME, "123");
                //uSER.ROLE = Role;
                var NewUser = new USER() { USRNAME = uSER.UserName, FIRST_NAME = uSER.FirstName, LAST_NAME = uSER.LastName, HASHED_PSWRD = uSER.Password, ROLE = uSER.Role, EML = uSER.Email };
                db.USERS.Add(NewUser);
                db.SaveChanges();
                return RedirectToAction("Edit_Privilege", "User", new { id = NewUser.ID });

            }

            return View(uSER);
        }

        // GET: USERs/Edit/5
        public ActionResult Edit(int? id, string ErrorMessage)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            ViewBag.UserId = Convert.ToInt32(this.Session["UserId"]);
            User User = (from usr in db.USERS
                        where usr.ID == id
                        select new User { UserName = usr.USRNAME, FirstName = usr.FIRST_NAME, LastName = usr.LAST_NAME, Role = usr.ROLE, Password = usr.HASHED_PSWRD, Email = usr.EML, Id = usr.ID }).FirstOrDefault();
            if (User == null)
            {
                return HttpNotFound();
            }
            return View(User);
        }

        // POST: USERs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User uSER)
        {
            if (ModelState.IsValid)
            {
                USER user_to_update = db.USERS.Find(uSER.Id);
                user_to_update.USRNAME = uSER.UserName;
                user_to_update.EML = uSER.Email;
                user_to_update.HASHED_PSWRD = uSER.Password;
                user_to_update.ROLE = uSER.Role;
                db.Entry(user_to_update).State = EntityState.Modified;
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
            ViewBag.ErrorMessage = "There seems to be some issue with Model State. Please try again.";
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
            var entity = db.PRIVILEGES_USERS.Where(a => a.USER_ID == id).ToList();
            if (entity != null && entity.Count() != 0)
            {
                foreach (var item in entity)
                {
                    PRIVILEGES_USERS pRIVILEGEaCCCESS = db.PRIVILEGES_USERS.Find(item.ID);
                    db.PRIVILEGES_USERS.Remove(pRIVILEGEaCCCESS);
                }
            }
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
                RedirectToAction("Edit", new { id = uSER.ID, ErrorMessage = ViewBag.ErrorMessage });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat("There is an Employee or Student or Parent in the system using this User ID. Please delete the base information first.", "\n", e.InnerException.InnerException.Message);
                RedirectToAction("Edit", new { id = uSER.ID, ErrorMessage = ViewBag.ErrorMessage });
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
        public ActionResult Login(USER user)
        {
            if (ModelState.IsValid)
            {
                if (user.IsValid(user.USRNAME, user.HASHED_PSWRD))
                {
                    FormsAuthentication.SetAuthCookie(user.USRNAME, true);
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
            var Config = new Configuration();
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


        public ActionResult Edit_Privilege(int? id, string Calling_Method)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var uSERpRIVILEGE = (from pr in db.PRIVILEGES
                                 join prusr in db.PRIVILEGES_USERS.Where(x=>x.USER_ID == id) on pr.ID equals prusr.PRIVILEGE_ID into gi
                                 from subgi in gi.DefaultIfEmpty()
                               select new SelectUserPrivilage { PrivilageList = pr,USRS_ID = id, Selected = (subgi == null ? false : true), IsActive = (pr.IS_ACT == true ? true : false) }).OrderBy(g => g.PrivilageList.PRIR).ToList();


            var uSERpRIVILEGEfINAL = (from u in uSERpRIVILEGE
                                          //where conditions or joins with other tables to be included here
                                      group u by new { u.PrivilageList.PRIVILEGE_TAG, u.PrivilageList.NAME } into grp
                                  let MaxPrIDPerPerson = grp.Max(g => g.PrivilageList.ID)
                                  from p in grp
                                  where p.PrivilageList.ID == MaxPrIDPerPerson
                                      select p).ToList();
            ViewBag.Calling_Method = Calling_Method;

            //var privilege_tag = db.PRIVILEGE_TAG.ToList();
            var privilege_tag = (from pt in db.PRIVILEGE_TAG
                                 select new SFSAcademy.Privilege_Tags { Privilege_Tag = pt, Selected = false }).OrderBy(x=>x.Privilege_Tag.PRIR).ToList();
            ViewData["privilege_tag"] = privilege_tag;
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
                    if (item.Selected)
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
                try { db.SaveChanges(); ViewBag.Notice = "User details updated in system successfully."; }
                catch (DbEntityValidationException e){foreach (var eve in e.EntityValidationErrors){foreach (var ve in eve.ValidationErrors){
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);}}
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                }
                if (Calling_Method == "Employee")
                {
                    int Employee_ID = db.EMPLOYEEs.Where(x => x.USRID == UserId).SingleOrDefault().ID;
                    return RedirectToAction("Admission4", "Employee", new { Emp_id = Employee_ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            return RedirectToAction("Index", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
        }

        public ActionResult Privilege_Access(int? Privilege_id, int? User_Id)
        {
            if (Privilege_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var aCCESSsRIVILEGE = (from usrac in db.USERS_ACCESS
                                 join prusr in db.PRIVILEGE_ACCESS.Where(x => x.PRIVILEGE_ID == Privilege_id) on usrac.ID equals prusr.USERS_ACCESS_ID into gi
                                 from subgi in gi.DefaultIfEmpty()
                                 select new SelectUserAccess { AccessList = usrac, PRIVILEGE_ID = Privilege_id, USRS_ID = User_Id, Selected = (subgi == null ? false : true)}).OrderBy(g => g.AccessList.CTL).ToList();


            var aCCESSsRIVILEGEfINAL = (from u in aCCESSsRIVILEGE
                                          //where conditions or joins with other tables to be included here
                                      group u by new { u.AccessList.CTL, u.AccessList.ACTN } into grp
                                      let MaxPrIDPerPerson = grp.Max(g => g.AccessList.ID)
                                      from p in grp
                                      where p.AccessList.ID == MaxPrIDPerPerson
                                      select p).ToList();

            //var privilege_tag = db.PRIVILEGE_TAG.ToList();
            var All_Controller = (from usrac in db.USERS_ACCESS.Select(x=>x.CTL).Distinct()
                               select new SFSAcademy.UserAccessController { AccessListController = usrac, Selected = false }).OrderBy(x => x.AccessListController).ToList();
            ViewData["controller_tag"] = All_Controller;
            PRIVILEGE Privilege = db.PRIVILEGES.Find(Privilege_id);
            ViewData["Privilege"] = Privilege;
            return View(aCCESSsRIVILEGEfINAL);

        }


        // POST: USERs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Privilege_Access(IList<SelectUserAccess> model)
        {
            int? UserId = model.FirstOrDefault().USRS_ID;
            if (ModelState.IsValid)
            {
                int? PrivilegeId = model.FirstOrDefault().PRIVILEGE_ID;
                foreach (SelectUserAccess item in model)
                {
                    if (item.Selected)
                    {
                        var uACCESSEVal = db.PRIVILEGE_ACCESS.Where(x => x.USERS_ACCESS_ID == item.AccessList.ID && x.PRIVILEGE_ID == item.PRIVILEGE_ID).ToList();
                        if (uACCESSEVal == null || uACCESSEVal.Count() == 0)
                        {
                            var NewAccess = new PRIVILEGE_ACCESS()
                            {
                                USERS_ACCESS_ID = item.AccessList.ID,
                                PRIVILEGE_ID = item.PRIVILEGE_ID
                            };
                            db.PRIVILEGE_ACCESS.Add(NewAccess);
                        }
                    }
                    else
                    {
                        var uACCESSEVal = db.PRIVILEGE_ACCESS.Where(x => x.USERS_ACCESS_ID == item.AccessList.ID && x.PRIVILEGE_ID == item.PRIVILEGE_ID).ToList();
                        if (uACCESSEVal != null && uACCESSEVal.Count() != 0)
                        {
                            foreach (var item2 in uACCESSEVal)
                            {
                                PRIVILEGE_ACCESS PrAccessToRemove = db.PRIVILEGE_ACCESS.Find(item2.ID);
                                db.PRIVILEGE_ACCESS.Remove(PrAccessToRemove);
                            }
                        }
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("Edit_Privilege", new { id = UserId });
        }

        public JsonResult IsUserExists(string UserName)
        {
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.   
            return Json(!db.USERS.Any(x => x.USRNAME == UserName), JsonRequestBehavior.AllowGet);
        }       
    }
}
