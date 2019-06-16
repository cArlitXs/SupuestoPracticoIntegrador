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
    public class LineaPedidoProductoController : Controller
    {
        private DBTiendaEntities db = new DBTiendaEntities();

        // GET: LineaPedidoProducto
        public ActionResult Index()
        {
            string usuario = User.Identity.GetUserName();
            
            int IdUsuario = db.Cliente.Where(e => e.Email == usuario).First().Id;

            int? IdPedido = db.Pedido.Where(e => e.IdUsuario == IdUsuario).FirstOrDefault().Id;

            var lineaPedidoProducto = db.LineaPedidoProducto.Include(l => l.Pedido);

            //return View(lineaPedidoProducto.ToList());

            if (IdPedido != null)
                return View(lineaPedidoProducto.Where(e => e.IdPedido == IdPedido).ToList());
            else
                return View();
        }

        // GET: LineaPedidoProducto/Details/5
        public ActionResult Details(int? IdProducto, int? IdPedido)
        {
            if (IdProducto == null || IdPedido == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LineaPedidoProducto lineaPedidoProducto = db.LineaPedidoProducto.Where(e => e.IdPedido == IdPedido).Where(a => a.IdProducto == IdProducto).FirstOrDefault();
            if (lineaPedidoProducto == null)
            {
                return HttpNotFound();
            }
            return View(lineaPedidoProducto);
        }

        // GET: LineaPedidoProducto/Create
        public ActionResult Create()
        {
            string usuario = User.Identity.GetUserName();
            int iduser = (from e in db.Cliente
                          where e.Email == usuario
                          select e).First().Id;
            int? idped = (from e in db.Pedido
                          where e.IdUsuario == iduser
                          select e).ToList().FirstOrDefault().Id;

            //ViewBag.IdPedido = new SelectList(db.Pedido.Where(e => e.Id == idped).ToList(), "Id", "Id");

            ViewBag.IdPedido = new SelectList(db.Pedido.Where(e => e.IdUsuario == iduser), "Id", "Id");
            ViewBag.IdProducto = new SelectList(db.Producto.Where(e => e.Escaparate == true), "Id", "Nombre");
            return View();
        }

        // POST: LineaPedidoProducto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPedido,IdProducto,Cantidad,PVP")] LineaPedidoProducto lineaPedidoProducto)
        {
            Producto Producto = db.Producto.Find(lineaPedidoProducto.IdProducto);
            lineaPedidoProducto.PVP = ((decimal)Producto.Precio * (decimal)Producto.IVA / 100 * lineaPedidoProducto.Cantidad) + (lineaPedidoProducto.Cantidad * (decimal)Producto.Precio);
            if (ModelState.IsValid)
            {
                db.LineaPedidoProducto.Add(lineaPedidoProducto);
                db.SaveChanges();
                return RedirectToAction("Details/" + db.Pedido.First().Id, "Pedidos");
            }
            ViewBag.IdPedido = new SelectList(db.Pedido, "Id", "Id", lineaPedidoProducto.IdPedido);
            ViewBag.IdProducto = new SelectList(db.Producto, "Id", "Nombre", lineaPedidoProducto.IdProducto);
            return View(lineaPedidoProducto);
        }

        // GET: LineaPedidoProducto/Edit/5
        /*public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LineaPedidoProducto lineaPedidoProducto = db.LineaPedidoProducto.Find(id);
            if (lineaPedidoProducto == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPedido = new SelectList(db.Pedido, "Id", "Id", lineaPedidoProducto.IdPedido);
            ViewBag.IdProducto = new SelectList(db.Producto, "Id", "Nombre", lineaPedidoProducto.IdProducto);
            return View(lineaPedidoProducto);
        }*/
        public ActionResult Edit(int? IdProducto, int? IdPedido)
        {
            if (IdProducto == null || IdPedido == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LineaPedidoProducto lineaPedidoProducto = db.LineaPedidoProducto.Where(e => e.IdPedido == IdPedido).Where(a => a.IdProducto == IdProducto).FirstOrDefault();
            if (lineaPedidoProducto == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPedido = new SelectList(db.Pedido, "Id", "Id", lineaPedidoProducto.IdPedido);
            ViewBag.IdProducto = new SelectList(db.Producto, "Id", "Nombre", lineaPedidoProducto.IdProducto);
            return View(lineaPedidoProducto);
        }

        // POST: LineaPedidoProducto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPedido,IdProducto,Cantidad,PVP")] LineaPedidoProducto lineaPedidoProducto)
        {
            Producto Producto = db.Producto.Find(lineaPedidoProducto.IdProducto);
            lineaPedidoProducto.PVP = ((decimal)Producto.Precio * (decimal)Producto.IVA / 100 * lineaPedidoProducto.Cantidad) + (lineaPedidoProducto.Cantidad * (decimal)Producto.Precio);
            if (ModelState.IsValid)
            {
                db.Entry(lineaPedidoProducto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details/" + db.Pedido.First().Id, "Pedidos");
            }
            ViewBag.IdPedido = new SelectList(db.Pedido, "Id", "Id", lineaPedidoProducto.IdPedido);
            ViewBag.IdProducto = new SelectList(db.Producto, "Id", "Nombre", lineaPedidoProducto.IdProducto);
            return View("Details/" + db.Pedido.First().Id, "Pedidos");
        }

        // GET: LineaPedidoProducto/Delete/5
        public ActionResult Delete(int? IdProducto, int? IdPedido)
        {
            if (IdProducto == null || IdPedido == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LineaPedidoProducto lineaPedidoProducto = db.LineaPedidoProducto.Where(e => e.IdPedido == IdPedido).Where(a => a.IdProducto == IdProducto).FirstOrDefault();
            if (lineaPedidoProducto == null)
            {
                return HttpNotFound();
            }
            return View(lineaPedidoProducto);
        }

        // POST: LineaPedidoProducto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int IdProducto, int IdPedido)
        {
            LineaPedidoProducto lineaPedidoProducto = db.LineaPedidoProducto.Where(e => e.IdPedido == IdPedido).Where(a => a.IdProducto == IdProducto).FirstOrDefault();
            db.LineaPedidoProducto.Remove(lineaPedidoProducto);
            db.SaveChanges();
            //cuando no tenga ningun producto volver a escaparate
            return RedirectToAction("Details/" + db.Pedido.OrderByDescending(e => e.Id).First().Id, "Carrito");
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
