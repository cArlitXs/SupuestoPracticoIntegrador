using GestionComida.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionComida.Controllers
{
    public class HomeController : Controller
    {
        private DBTiendaEntities db = new DBTiendaEntities();
        public ActionResult Index()
        {
            DBTiendaEntities db = new DBTiendaEntities();
            // Si existe el cliente correspondiente al usuario actual
            // se va a View, en caso contrario se va a crear el empleado.
            string usuario = User.Identity.GetUserName();
            var cliente = db.Cliente.Where(e => e.Email == usuario).FirstOrDefault();
            if (User.Identity.IsAuthenticated && User.IsInRole("Usuario") && cliente == null)
            {
                return RedirectToAction("Create", "Datos");
            }
            //return View();

            if(User.IsInRole("Administrador"))
                return RedirectToAction("BienvenidoAdmin", "Escaparate");
            else
                return RedirectToAction("Index", "Escaparate");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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