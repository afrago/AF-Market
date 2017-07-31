using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AF_Market.Controllers
{
    public class AJAXCoceptController : Controller
    {
        // GET: AJAXCocept
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult JsonFactorial(int n)
        {
            if (!Request.IsAjaxRequest())
            {
                return null;
            }
            // Creación  de un objeto Json
            var result = new JsonResult
            {
              //   Propiedad Factorial, implementada a partir de la funci´n Factoril
                Data = new { Factorial = Factorial(n) }

            };
            return result;
        }

        private double Factorial(int n)
        {
            double factorial = 1;
            for (int i = 2; i < n; i++)
            {
                factorial *= i;
            }
            return factorial;
        }
    }
}