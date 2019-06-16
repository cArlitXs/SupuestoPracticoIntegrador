using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionComida.Models;

namespace GestionComida.Controllers
{
    public class EscaparateController : Controller
    {
        private DBTiendaEntities db = new DBTiendaEntities();
        // GET: Escaparate
        public ActionResult Index(int? id)
        {
            var producto = db.Producto.Include(e => e.Categoria).Where(a => a.Escaparate == true);
            //ViewBag.Categorias = db.Categoria.ToList();
            ViewBag.Categorias = db.Categoria.ToList();

            if (id != null)
                producto = db.Producto.Include(e => e.Categoria).Where(a => a.IdCategoria == id).Where(e => e.Escaparate == true);

            return View(producto.ToList());
        }
        public ActionResult BienvenidoAdmin()
        {
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