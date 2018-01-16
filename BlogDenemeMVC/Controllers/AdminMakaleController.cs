using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogDenemeMVC.Models;
using System.Web.Helpers;
using System.IO;

namespace BlogDenemeMVC.Controllers
{
    public class AdminMakaleController : Controller
    {
        mvcblogDB db = new mvcblogDB();
        // GET: AdminMakale
        public ActionResult Index()
        {
            var makales = db.Makales.ToList();
            return View(makales);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            var makale = db.Makales.Where(x => x.MakeleID == id).FirstOrDefault();
            return View(makale);
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale, string etiketler, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makale.Resim = "/Uploads/MakaleFoto/" + newfoto;
                   

                }
                if (etiketler != null)
                {
                    string[] etiketdizi = etiketler.Split(',');
                    foreach (var i in etiketdizi)
                    {
                        var yenietiket = new Etiket { EtiketAdi = i };
                        db.Etikets.Add(yenietiket);
                        makale.Etikets.Add(yenietiket);

                    }
                }
                makale.UyeID = Convert.ToInt32(Session["uyeid"]);
                db.Makales.Add(makale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(makale);

        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(x => x.MakeleID == id).FirstOrDefault();
            if (makale == null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi",makale.KategoriID);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase Foto,Makale yeniMakale)
        {
            try
            {
                var makale = db.Makales.Where(x => x.MakeleID == id).FirstOrDefault();
                if (Foto != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(makale.Resim)))
                    {
                        System.IO.File.Delete(Server.MapPath(makale.Resim));
                    }
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makale.Resim = "/Uploads/MakaleFoto/" + newfoto;
                    makale.Baslik = yeniMakale.Baslik;
                    makale.Icerik = yeniMakale.Icerik;
                    makale.KategoriID = yeniMakale.KategoriID;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(x => x.MakeleID == id).FirstOrDefault();
            if (makale == null)
                HttpNotFound();
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var makale = db.Makales.Where(x => x.MakeleID == id).FirstOrDefault();
                if (makale == null)
                    HttpNotFound();
                if (System.IO.File.Exists(Server.MapPath(makale.Resim)))
                {
                    System.IO.File.Delete(Server.MapPath(makale.Resim));
                }
                foreach (var i in makale.Yorums.ToList())
                {
                    db.Yorums.Remove(i);
                }
                foreach (var i in makale.Etikets.ToList())
                {
                    db.Etikets.Remove(i);
                }
                db.Makales.Remove(makale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
