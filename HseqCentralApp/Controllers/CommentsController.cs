using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HseqCentralApp.Models;
using DevExpress.Web.Mvc;

namespace HseqCentralApp.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Delegatable).Include(c => c.HseqRecord);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.DelegatableID = new SelectList(db.Delegatables, "DelegatableID", "Title");
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "CaseNo");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommentID,Content,OwnerID,DateCreated,AssociatedType,HseqRecordID,DelegatableID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DelegatableID = new SelectList(db.Delegatables, "DelegatableID", "Title", comment.DelegatableID);
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "CaseNo", comment.HseqRecordID);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.DelegatableID = new SelectList(db.Delegatables, "DelegatableID", "Title", comment.DelegatableID);
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "CaseNo", comment.HseqRecordID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentID,Content,OwnerID,DateCreated,AssociatedType,HseqRecordID,DelegatableID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DelegatableID = new SelectList(db.Delegatables, "DelegatableID", "Title", comment.DelegatableID);
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "CaseNo", comment.HseqRecordID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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

        ///////////////////////////////////////////////////////////////////////////////////////////////

        [ValidateInput(false)]
        public ActionResult CommentsPanelPartial()
        {
            var model = db.Comments;
            return PartialView("~/Views/Shared/_CommentsPanel.cshtml", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CommentsPanelAddNew(HseqCentralApp.Models.Comment item)
        {
            var model = db.Comments;
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to insert the new item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("~/Views/Shared/_CommentsPanel.cshtml", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CommentsPanelPartialUpdate(HseqCentralApp.Models.Comment item)
        {
            var model = db.Comments;
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to update the item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("~/Views/Shared/_CommentsPanel.cshtml", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CommentsPanelPartialDelete(System.Int32 CommentID)
        {
            var model = db.Comments;
            if (CommentID >= 0)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("~/Views/Shared/_CommentsPanel.cshtml", model.ToList());
        }

        HseqCentralApp.Models.ApplicationDbContext db1 = new HseqCentralApp.Models.ApplicationDbContext();

        [ValidateInput(false)]
        public ActionResult CommentDataViewPartial()
        {
            var model = db1.Comments;
            return PartialView("~/Views/Shared/_CommentPanel.cshtml", model.ToList());
        }
    }
}
