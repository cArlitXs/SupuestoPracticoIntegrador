using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionComida.Models;
using Microsoft.AspNet.Identity;

namespace GestionComida.Controllers
{
    public class PedidosController : Controller
    {
        private DBTiendaEntities db = new DBTiendaEntities();

        // GET: Pedidos
        public ActionResult Index()
        {
            if(User.IsInRole("Administrador"))
            {
                var pedido = db.Pedido.Include(e => e.Cliente);
                return View(pedido.OrderByDescending(e => e.Id).ToList());
            }
            else
            {
                string usuario = User.Identity.GetUserName();
                int iduser = (from e in db.Cliente
                              where e.Email == usuario
                              select e).First().Id;
                return View(db.Pedido.Where(e => e.IdUsuario == iduser).OrderByDescending(e => e.Id).ToList());
            }
        }

        // GET: Pedidos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // GET: Pedidos/Create
        public ActionResult Create()
        {
            /*Pedido pedido;
            string usuario = User.Identity.GetUserName();
            pedido.FechaCompra = DateTime.Now;
            pedido.IdUsuario = (from e in db.Cliente
                                where e.Email == usuario
                                select e).First().Id;*/

            return View();
        }

        // POST: Pedidos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdUsuario,FechaCompra,FechaPago,FechaEnvio,FechaEntrega,FechaDevolucion")] Pedido pedido)
        {
            string usuario = User.Identity.GetUserName();
            pedido.FechaCompra = DateTime.Now;
            //pedido.IdUsuario = db.Cliente.Where(e => e.Email == usuario).First().Id;
            pedido.IdUsuario = (from e in db.Cliente
                                where e.Email == usuario
                                select e).First().Id;

            //if (ModelState.IsValid)
            //{
                db.Pedido.Add(pedido);
                db.SaveChanges();
                return RedirectToAction("Index");
            //}

            //return View(pedido);
        }

        // GET: Pedidos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdUsuario,FechaCompra,FechaPago,FechaEnvio,FechaEntrega,FechaDevolucion")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pedido pedido = db.Pedido.Find(id);
            db.Pedido.Remove(pedido);
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
    }
}
