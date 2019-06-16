using GestionComida.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionComida.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class DatosController : Controller
    {
        // GET: Datos
        /*public ActionResult Index()
        {
            return View();
        }*/

        private DBTiendaEntities db = new DBTiendaEntities();

        // GET: MisDatos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MisDatos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Email,Telefono,CP,Direccion,Poblacion,Provincia,FechaNacimiento,Activo")] Cliente cliente)
        {
            // Para asignar el Nombre del usuario identificado al campo Email de Empleado
            cliente.Email = User.Identity.GetUserName();
            cliente.Activo = true;

            if (ModelState.IsValid)
            {
                db.Cliente.Add(cliente);
                db.SaveChanges();

                // Redirige a la acción Index del controlador Home
                return RedirectToAction("Index", "Home");
            }
            return View(cliente);
        }

        // GET: MisDatos/Edit
        public ActionResult Edit()
        {
            // Se seleccionan los datos del empleado correspondiente al usuario actual
            string wUsuario = User.Identity.GetUserName();
            var cliente = db.Cliente.Where(e => e.Email == wUsuario).FirstOrDefault();
            if (cliente == null)
            {
                // Si no existe el empleado, redirige a la acción Index del controlador Home
                return RedirectToAction("Index", "Home");
            }
            // Si existe el empleado correspondiente se va a View
            return View(cliente);
        }
        // POST: MisDatos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Email,Telefono,CP,Direccion,Poblacion,Provincia,FechaNacimiento,Activo")] Cliente cliente)
        {
            cliente.Email = User.Identity.GetUserName();
            cliente.Activo = true;
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(cliente);
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