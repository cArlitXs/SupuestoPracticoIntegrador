using GestionComida.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GestionComida.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class CarritoController : Controller
    {
        private DBTiendaEntities db = new DBTiendaEntities();

        // GET: Carrito
        public ActionResult Index()
        {
            /*Hacer in condicional por si hay un carrito vacio o no*/
            return View();
        }
        // GET: LineaPedidoProducto/Details/5
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

        public ActionResult Agregar(int id)
        {
            string usuario = User.Identity.GetUserName();
            Cliente cliente = db.Cliente.Where(e => e.Email == usuario).FirstOrDefault();
            int IdUser = db.Cliente.Where(e => e.Email == usuario).First().Id;
            Pedido Pedido = db.Pedido.Where(e => e.IdUsuario == IdUser).OrderByDescending(e => e.Id).FirstOrDefault();

            if (System.Web.HttpContext.Current.Session["pedido"] == null)
            {
                Pedido pedidoNuevo = new Pedido();
                pedidoNuevo.FechaCompra = DateTime.Now;
                pedidoNuevo.IdUsuario = cliente.Id;

                db.Pedido.Add(pedidoNuevo);
                db.SaveChanges();

                System.Web.HttpContext.Current.Session["pedido"] = pedidoNuevo.Id;
            }
            else if(db.LineaPedidoProducto.Where(e => e.IdPedido == Pedido.Id).Where(a => a.IdProducto == id).FirstOrDefault() != null)
            {
                int numPed = Pedido.Id;
                return RedirectToAction("Details/" + numPed, "Carrito");
            }

            LineaPedidoProducto PedidoProducto = new LineaPedidoProducto();
            PedidoProducto.IdPedido = (int)System.Web.HttpContext.Current.Session["pedido"];
            PedidoProducto.IdProducto = id;
            PedidoProducto.Cantidad = 1;
            Producto Producto = db.Producto.Find(id);
            //PedidoProducto.PVP = (decimal)db.Producto.Find(id).Precio;
            PedidoProducto.PVP = ((decimal)Producto.Precio * (decimal)Producto.IVA / 100 * PedidoProducto.Cantidad) + (PedidoProducto.Cantidad * (decimal)Producto.Precio);
            db.LineaPedidoProducto.Add(PedidoProducto);

            db.SaveChanges();

            return RedirectToAction("Index", "Escaparate");
        }

        public ActionResult sumarCantidad(int? IdPed, int? IdProd)
        {
            LineaPedidoProducto pedidoProducto = db.LineaPedidoProducto.Where(e => e.IdPedido == IdPed).Where(a => a.IdProducto == IdProd).FirstOrDefault();
            Producto Producto = db.Producto.Find(pedidoProducto.IdProducto);

            if (pedidoProducto != null)
            {
                pedidoProducto.Cantidad += 1;
                pedidoProducto.PVP = ((decimal)Producto.Precio * (decimal)Producto.IVA / 100 * pedidoProducto.Cantidad) + (pedidoProducto.Cantidad * (decimal)Producto.Precio);
                db.Entry(pedidoProducto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Details/" + IdPed, "Carrito");
        }
        
        public ActionResult restarCantidad(int? IdPed, int? IdProd)
        {
            LineaPedidoProducto pedidoProducto = db.LineaPedidoProducto.Where(e => e.IdPedido == IdPed).Where(a => a.IdProducto == IdProd).FirstOrDefault();
            Producto Producto = db.Producto.Find(pedidoProducto.IdProducto);

            if (pedidoProducto != null && pedidoProducto.Cantidad > 1)
            {
                pedidoProducto.Cantidad -= 1;
                pedidoProducto.PVP = ((decimal)Producto.Precio * (decimal)Producto.IVA / 100 * pedidoProducto.Cantidad) + (pedidoProducto.Cantidad * (decimal)Producto.Precio);
                db.Entry(pedidoProducto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Details/" + IdPed, "Carrito");
        }
        
        public ActionResult Confirmar(int? IdPed)
        {
            if (IdPed == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(IdPed);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }
        
        public ActionResult ConfirmarPedido(int? IdPed)
        {
            Pedido pedido = db.Pedido.Where(a => a.Id == IdPed).FirstOrDefault();
            pedido.FechaPago = DateTime.Now;
            db.Entry(pedido).State = System.Data.Entity.EntityState.Modified;
            //db.Pedido.Add(pedido);
            db.SaveChanges();
            System.Web.HttpContext.Current.Session["pedido"] = null;
            return RedirectToAction("Gracias" , "Carrito");
        }

        public ActionResult CarritoVacio()
        {
            return View();
        }

        public ActionResult Gracias()
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