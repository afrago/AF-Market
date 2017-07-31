using AF_Market.Models;
using AF_Market.ModelView;
using AF_Market.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AF_Market.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Users
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var users = userManager.Users.ToList();
            var usersView = new List<UserView>();

            foreach (var user in users )
            {
                var userView = new UserView
                {
                    EMail = user.Email,
                    Name = user.UserName,
                    UserID  = user.Id 
                };
                usersView.Add(userView);
            }
           // var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
           // Le mandamos la lista de usuarios a la lista
            return View(usersView);
        }

        public ActionResult Roles(string userID)
        {
            if (string.IsNullOrEmpty(userID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var users = userManager.Users.ToList();
            var user = users.Find(u => u.Id == userID);
            if (user == null)
            {
                return HttpNotFound();
            }
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var roles = roleManager.Roles.ToList();
            var rolesView = new List<RoleView>();
            foreach (var item in user.Roles)
            {
                // Obtner el nombre del rol
                var role = roles.Find(r => r.Id == item.RoleId);
                var roleView = new RoleView
                {
                    RoleID = role.Id,
                    RoleName = role.Name 
                };
                rolesView.Add(roleView);
            }

            var userView = new UserView
            {
                EMail = user.Email,
                Name = user.UserName,
                UserID = user.Id,
                Roles = rolesView
            };

            return View(userView);
        }

        public ActionResult AddRole(string userID)
        {
            if(string.IsNullOrEmpty(userID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var users = userManager.Users.ToList();
            var user = users.Find(u => u.Id == userID);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userView = new UserView
            {
                EMail = user.Email,
                Name = user.UserName,
                UserID = user.Id,
            };

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            // Armamos el ViewBack
            var list = roleManager.Roles.ToList();
            list.Add(new IdentityRole  { Id = "", Name = "[Selet a role...]" });
            list = list.OrderBy(c => c.Name).ToList();
            ViewBag.CustomerID = new SelectList(list, "Id", "Name");


            return View(userView);
        }
        [HttpPost ]
        public ActionResult AddRole(string userID, FormCollection form)
        {
            // En caso de haber seleccionado Rol
            var roleID = Request["RoleID"];
            // Buscamos el usuario
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var users = userManager.Users.ToList();
            var user = users.Find(u => u.Id == userID);
            var userView = new UserView
            {
                EMail = user.Email,
                Name = user.UserName,
                UserID = user.Id,
            };
            // En caso de no se haya selecc role
            if (string.IsNullOrEmpty(roleID))
            {
                // armamos la lista de roles
                ViewBag.Error = "You must selecc a role";
                var list = roleManager.Roles.ToList();
                list.Add(new IdentityRole { Id = "", Name = "[Selet a role...]" });
                list = list.OrderBy(c => c.Name).ToList();
                ViewBag.RoleID = new SelectList(list, "Id", "Name");
                return View(userView);
            }
            // En caso de role selecc    // buscamos que no tenga el rol selecc
            // Hay que buscar el rol
            var roles = roleManager.Roles.ToList();
            var role = roles.Find(r => r.Id == roleID );
            if (!userManager.IsInRole(userID , role.Name  ))
            {
                // el usuario no existe en el rol, hay que agregarlo
                userManager.AddToRole(userID, role.Name);
            }


            var rolesView = new List<RoleView>();
            foreach (var item in user.Roles)
            {
                // Obtner el nombre del rol
                role = roles.Find(r => r.Id == item.RoleId);
                var roleView = new RoleView
                {
                    RoleID = role.Id,
                    RoleName = role.Name
                };
                rolesView.Add(roleView);
            }

            userView = new UserView
            {
                EMail = user.Email,
                Name = user.UserName,
                UserID = user.Id,
                Roles = rolesView
            };

            return View("Roles", userView);
        }

        public ActionResult Delete(string userID, string roleID)
        {
            if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(roleID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Eliminar el ROL, pero no tenemos los nombres.. necesitamos user y rol manager
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            var user = userManager.Users.ToList().Find(u => u.Id == userID);
            var role = roleManager.Roles.ToList().Find(r => r.Id == roleID);
            /// Delete user from role
            if (userManager.IsInRole(user.Id, role.Name))
            {
                userManager.RemoveFromRole(user.Id, role.Name);
            }
            /// Prepare the view to return

            // Para devolver a la vista de roless, tenemos que volver a armar el  user view
            var users = userManager.Users.ToList();
             user = users.Find(u => u.Id == userID);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userView = new UserView
            {
                EMail = user.Email,
                Name = user.UserName,
                UserID = user.Id,
            };

             roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            // Armamos el ViewBack
            var list = roleManager.Roles.ToList();
            list.Add(new IdentityRole { Id = "", Name = "[Selet a role...]" });
            list = list.OrderBy(c => c.Name).ToList();
            ViewBag.CustomerID = new SelectList(list, "Id", "Name");


            return View("Roles",userView);
     
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