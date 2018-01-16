using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogDenemeMVC.Models;
using PagedList;
using PagedList.Mvc;

namespace BlogDenemeMVC.Controllers
{
    public class HomeController : Controller
    {
        mvcblogDB db = new mvcblogDB();
        // GET: Home
        public ActionResult Index(int Page=1)
        {

            var makale = db.Makales.OrderByDescending(x => x.MakeleID).ToPagedList(Page,5);
            return View(makale);
        }
        public ActionResult BlogAra(string Ara)
        {
            var aranan = db.Makales.Where(x => x.Baslik.Contains(Ara)).ToList();
            return View(aranan.OrderByDescending(x=>x.EklenmeTarihi));
        }
        public ActionResult MakaleDetay(int id)
        {
            var makale = db.Makales.Where(x => x.MakeleID == id).FirstOrDefault();
            if (makale == null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }
        public ActionResult Hakkimizda()
        {
            return View();
        }
        public ActionResult Iletisim()
        {
            return View();
        }
        public ActionResult KategoriPartial()
        {
            return View(db.Kategoris.ToList());
        }
        
        public JsonResult YorumYap(string yorum,int MakaleID)
        {
            var uyeid = Session["uyeid"];
            if (yorum == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            db.Yorums.Add(new Yorum { UyeID = Convert.ToInt32(uyeid), MakaleID = MakaleID, Icerik = yorum, Tarih = "12/12/2017" });
            db.SaveChanges();
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult YorumSil(int id)
        {
            var uyeid = Session["uyeid"];
            var yorum = db.Yorums.Where(x => x.YorumID == id).FirstOrDefault();
            var makale = db.Makales.Where(x => x.MakeleID == yorum.MakaleID).FirstOrDefault();
            if (yorum.UyeID == Convert.ToInt32(Session["uyeid"]))
            {
                db.Yorums.Remove(yorum);
                db.SaveChanges();
                return RedirectToAction("MakaleDetay", "Home", new { id = makale.MakeleID });
            }
            else
                return HttpNotFound();
        }
        public ActionResult OkunmaArttir(int MakaleID)
        {
            var makale = db.Makales.Where(x => x.MakeleID == MakaleID).FirstOrDefault();
            if (makale.Okunma == null)
            {
                makale.Okunma = 1;
            }
            else
                makale.Okunma += 1;
            db.SaveChanges();
            return View();
        }
        public ActionResult KategoriMakale(int id,int Page=1)
        {
            var makaleler = db.Makales.Where(x => x.KategoriID == id).OrderByDescending(x => x.EklenmeTarihi).ToPagedList(Page, 5);
            return View(makaleler);
        }
    }
}