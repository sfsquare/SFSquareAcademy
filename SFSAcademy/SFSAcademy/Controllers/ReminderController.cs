using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFSAcademy;

namespace SFSAcademy.Controllers
{
    public class ReminderController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Reminder
        public ActionResult Index()
        {
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var reminders = (from rm in db.REMINDERs.Include(x => x.USER).Include(x => x.USER1)
                             where rm.RCPNT == UserId && rm.IS_DEL_BY_RCPNT == false
                             select new ReminderSelect { ReminderData = rm, Selected = false}).OrderByDescending(x => x.ReminderData.CREATED_AT).ToList();
            //var reminders = db.REMINDERs.Include(x => x.USER).Include(x => x.USER1).Where(x => x.RCPNT == UserId && x.IS_DEL_BY_RCPNT == false).OrderByDescending(x => x.CREATED_AT).ToList();
            ViewData["reminders"] = reminders;
            var read_reminders = db.REMINDERs.Where(x => x.RCPNT == UserId && x.IS_READ == true && x.IS_DEL_BY_RCPNT == false).OrderByDescending(x => x.CREATED_AT).ToList();
            ViewData["read_reminders"] = read_reminders;
            var new_reminder_count = db.REMINDERs.Where(x => x.RCPNT == UserId && x.IS_READ == false && x.IS_DEL_BY_RCPNT == false).ToList();
            ViewData["new_reminder_count"] = new_reminder_count;
            return View();
        }

        // GET: Reminder/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REMINDER rEMINDER = db.REMINDERs.Find(id);
            if (rEMINDER == null)
            {
                return HttpNotFound();
            }
            return View(rEMINDER);
        }

        // GET: Reminder/Create
        public ActionResult Create()
        {
            ViewBag.SNDR = new SelectList(db.USERS, "ID", "USRNAME");
            ViewBag.RCPNT = new SelectList(db.USERS, "ID", "USRNAME");
            return View();
        }

        // POST: Reminder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SNDR,RCPNT,SUB,BODY,IS_READ,IS_DEL_BY_SNDR,IS_DEL_BY_RCPNT,CREATED_AT,UPDATED_AT")] REMINDER rEMINDER)
        {
            if (ModelState.IsValid)
            {
                db.REMINDERs.Add(rEMINDER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SNDR = new SelectList(db.USERS, "ID", "USRNAME", rEMINDER.SNDR);
            ViewBag.RCPNT = new SelectList(db.USERS, "ID", "USRNAME", rEMINDER.RCPNT);
            return View(rEMINDER);
        }

        // GET: Reminder/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REMINDER rEMINDER = db.REMINDERs.Find(id);
            if (rEMINDER == null)
            {
                return HttpNotFound();
            }
            ViewBag.SNDR = new SelectList(db.USERS, "ID", "USRNAME", rEMINDER.SNDR);
            ViewBag.RCPNT = new SelectList(db.USERS, "ID", "USRNAME", rEMINDER.RCPNT);
            return View(rEMINDER);
        }

        // POST: Reminder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SNDR,RCPNT,SUB,BODY,IS_READ,IS_DEL_BY_SNDR,IS_DEL_BY_RCPNT,CREATED_AT,UPDATED_AT")] REMINDER rEMINDER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rEMINDER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SNDR = new SelectList(db.USERS, "ID", "USRNAME", rEMINDER.SNDR);
            ViewBag.RCPNT = new SelectList(db.USERS, "ID", "USRNAME", rEMINDER.RCPNT);
            return View(rEMINDER);
        }

        // GET: Reminder/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REMINDER rEMINDER = db.REMINDERs.Find(id);
            if (rEMINDER == null)
            {
                return HttpNotFound();
            }
            return View(rEMINDER);
        }

        // POST: Reminder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            REMINDER rEMINDER = db.REMINDERs.Find(id);
            db.REMINDERs.Remove(rEMINDER);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //This action is created from Student -> Profile page. Employee Ids are all tutors list comma seprated 
        public ActionResult Create_Reminder(string employee_ids )
        {
            string TutorList = employee_ids;
            ViewBag.TutorList = TutorList;
            ViewBag.SNDR = new SelectList(db.USERS, "ID", "USRNAME");
            ViewBag.RCPNT = new SelectList(db.USERS, "ID", "USRNAME");
            return View();
        }

        // POST: Reminder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Reminder([Bind(Include = "ID,SNDR,RCPNT,SUB,BODY,IS_READ,IS_DEL_BY_SNDR,IS_DEL_BY_RCPNT,CREATED_AT,UPDATED_AT")] REMINDER rEMINDER)
        {
            if (ModelState.IsValid)
            {
                db.REMINDERs.Add(rEMINDER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SNDR = new SelectList(db.USERS, "ID", "USRNAME", rEMINDER.SNDR);
            ViewBag.RCPNT = new SelectList(db.USERS, "ID", "USRNAME", rEMINDER.RCPNT);
            return View(rEMINDER);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
