using AF_Market.Models;
using AF_Market.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AF_Market.Controllers
{
    public class OrdersController : Controller
    {
        AF_MarketContext db = new AF_MarketContext();
        
        // Esta acción es el GET
        public ActionResult NewOrder()
        {
            var orderView = new OrderView();
            orderView.Customer = new Customer();
            orderView.Products = new List<ProductOrder>();
            
            Session["orderView"] = orderView;

            var list = db.Customers.ToList();
            list.Add(new Customer{ CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
            list = list.OrderBy(c => c.FullName).ToList();
            ViewBag.CustomerID = new SelectList(list, "CustomerID", "FullName");
            
            return View(orderView);
        }
        // La siguiente acción que llamamos es el POST
        [HttpPost]
        public ActionResult NewOrder(OrderView orderView)
        {
            orderView = Session["overView"] as OrderView;

            // Obtenemos el control HTML a traves de su ID, como desconecemos el ID tenemos que mirarlo desde el depurador del explorador
            var customerID = int.Parse(Request["CustomerID"]);
            if (customerID == 0)
            {

                var list = db.Customers.ToList();
                list.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
                list = list.OrderBy(c => c.FullName).ToList();
                ViewBag.CustomerID = new SelectList(list, "CustomerID", "FullName");

                ViewBag.Error = "Debe seleccionar un cliente";
                return View(orderView);

            }
            var customer = db.Customers.Find(customerID);
            if (customer == null)
            {

                var list = db.Customers.ToList();
                list.Add(new Customer{ CustomerID = 0, FirstName = "[Seleccione un cliente]" });
                list = list.OrderBy(c => c.FullName).ToList();
                ViewBag.DocumentTypeID = new SelectList(list, "CustomerID", "FullName ");
                ViewBag.Error = "Cliente no existe";
                return View(orderView);

            }

            if (orderView.Products.Count == 0)
            {
                var list = db.Customers.ToList();
                list.Add(new Customer{ CustomerID = 0, FirstName = "[Seleccione un cliente]" });
                list = list.OrderBy(c => c.FullName).ToList();
                ViewBag.DocumentTypeID = new SelectList(list, "CustomerID", "FullName ");
                ViewBag.Error = "Debe ingresar detalle";
                return View(orderView);
            }

            // Manejo transaccional para almacenar más de un registro cada vez
            int orderID = 0;
            int i = 0;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order
                    {
                        CustomerID = customerID,
                        DateOrder = DateTime.Now,
                        OrderStatus = OrderStatus.Created
                    };

                    db.Orders.Add(order);

                    db.SaveChanges();

                    orderID = db.Orders.ToList().Select(o => o.OrderID).Max();

                    foreach (var item in orderView.Products)
                    {
                        var orderDetail = new OrderDetail
                        {
                            Description = item.Description,
                            Price = item.Price,
                            Quantity = item.Quantity,
                            ProductID = item.ProductID,
                            OrderID = orderID
                        };
                        db.OrderDetails.Add(orderDetail);
                        // Hay que hacer el save changes dentro de cada uno de los objetos
                        db.SaveChanges();

                        i++;
                        if(i > 2)
                        {
                            // Generamos un error
                            int a = 0;
                            i /= a;
                            // La base de datos queda incompleta --> Rollback
                            // Manejo transaccional : o hacemos todo o no hacemos nada!!
                        }
                    }
                    transaction.Commit();    
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    ViewBag.Error = "ERROR;" + e.Message;

                    var listCu = db.Customers.ToList();
                    listCu.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente]" });
                    listCu = listCu.OrderBy(c => c.FullName).ToList();
                    ViewBag.DocumentTypeID = new SelectList(listCu, "CustomerID", "FullName ");
                    ViewBag.Error = "Debe ingresar detalle";


                    return View(orderView);
                }
            }

            ViewBag.Message = string.Format("La orden: {0}, grabada ok", orderID);
            // Antes de retornar a la vista, despues de almacenar la info en bbdd pasamos el viewback a la vista
            var listC = db.Customers.ToList();
            listC.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente]" });
            listC = listC.OrderBy(c => c.FullName).ToList();
            ViewBag.DocumentTypeID = new SelectList(listC, "CustomerID", "FullName ");
            ViewBag.Error = "Debe ingresar detalle";
          
            // Tenemos que generar otra lista, de otra manera lo pasa nulo
             orderView = new OrderView();
            orderView.Customer = new Customer();
            orderView.Products = new List<ProductOrder>();

            Session["orderView"] = orderView;

            return View(orderView);
            // RedirectToAction("NewOrder");
        }

        // Llamada desde el boton de la vista de Añadir producto
        public ActionResult AddProduct()
        {
            var list = db.Products.ToList();
            list.Add(new ProductOrder { ProductID = 0, Description = "[Seleccione un producto]"});
            list = list.OrderBy(p => p.Description).ToList();
            ViewBag.ProductID = new SelectList(list, "ProductID", "Description ");

            ViewBag.Error = "Debe seleccionar un producto"; 
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductOrder productOrder)
        {          
            var orderView = Session["orderView"] as OrderView;

            var productID = int.Parse(Request["ProductID"]);
            if (productID == 0)
            {
                var list = db.Products.ToList();
                list.Add(new ProductOrder { ProductID = 0, Description = "[Seleccione un producto]" });
                list = list.OrderBy(c => c.Description).ToList();
                ViewBag.ProductID = new SelectList(list, "ProductID", "Description ");
                ViewBag.Error = "Debe seleccionar un producto";
                return View(productOrder);
            }

            var product = db.Products.Find(productID);
            if (product == null)
            {
                var list = db.Products.ToList();
                list.Add(new ProductOrder { ProductID = 0, Description = "[Seleccione un producto]" });
                list = list.OrderBy(c => c.Description).ToList();
                ViewBag.ProductID = new SelectList(list, "ProductID", "Description ");
                ViewBag.Error = "Producto no existe";
                return View(productOrder);
            }

            productOrder = orderView.Products.Find(p => p.ProductID == productID);
            if (productOrder == null)
            {

                productOrder = new ProductOrder
                {
                    Description = product.Description,
                    Price = product.Price,
                    ProductID = product.ProductID,
                    Quantity = float.Parse(Request["Quantity"]),
                };
                orderView.Products.Add(productOrder);
            }
            else
            {
                productOrder.Quantity += float.Parse(Request["Quantity"]);

            }
           

            var listC = db.Customers.ToList();
            listC.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
            listC = listC.OrderBy(c => c.FullName).ToList();
            ViewBag.CustomerID = new SelectList(listC, "CustomerID", "FullName");
            return View("NewOrder",     orderView);
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