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
    public class UyeController : Controller
    {
        mvcblogDB db = new mvcblogDB();
        // GET: Uye
        public ActionResult Index(int id)
        {
            var uye = db.Uyes.Where(x => x.UyeID == id).FirstOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeID)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Uye uye)
        {
            var login = db.Uyes.Where(x => x.KullaniciAdi == uye.KullaniciAdi).FirstOrDefault();
            if (login.KullaniciAdi == uye.KullaniciAdi && login.Email == uye.Email && login.Sifre == uye.Sifre)
            {
                Session["uyeid"] = login.UyeID;
                Session["kullaniciadi"] = login.KullaniciAdi;
                Session["yetkiid"] = login.YetkiID;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Uyari = "Kullanici adi, Sifre veya Mail adresinizi kontrol ettiniz";
                return View();
            }

        }

        public ActionResult Logout()
        {
            Session["uyeid"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Uye uye, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newfoto);
                    uye.UyeFoto = "/Uploads/UyeFoto/" + newfoto;
                    uye.YetkiID = 2;
                    db.Uyes.Add(uye);
                    db.SaveChanges();
                    Session["uyeid"] = uye.UyeID;
                    Session["kullaniciadi"] = uye.KullaniciAdi;
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("Fotoğraf", "Fotoğraf seçiniz");

            }
            return View(uye);
        }
        public ActionResult Edit(int id)
        {
            var uye = db.Uyes.Where(x => x.UyeID == id).FirstOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeID)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        [HttpPost]
        public ActionResult Edit(Uye uye, int id, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                var uyes = db.Uyes.Where(x => x.UyeID == id).FirstOrDefault();
                if (Foto != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(uye.UyeFoto)))
                    {
                        System.IO.File.Delete(Server.MapPath(uyes.UyeFoto));
                    }
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newfoto);
                    uyes.UyeFoto = "/Uploads/UyeFoto/" + newfoto;
                }
                uyes.AdSoyad = uye.AdSoyad;
                uyes.KullaniciAdi = uye.KullaniciAdi;
                uyes.Sifre = uye.Sifre;
                uyes.Email = uye.Email;
                db.SaveChanges();
                Session["kullaniciadi"] = uye.KullaniciAdi;
                return RedirectToAction("Index", "Home", new { id = uyes.UyeID });

            }
            return View();
        }
        public ActionResult UyeProfil(int id)
        {
            var uye = db.Uyes.Where(x => x.UyeID == id).FirstOrDefault();
            return View(uye);
        }
    }
}