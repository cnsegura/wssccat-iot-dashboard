using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wssccatIotDashboard.Models;

namespace wssccatIotDashboard.Controllers
{
    public class GetKafkaDataController : Controller
    {
        // GET: GetKafkaData
        public ActionResult KafkaValues()
        {
            
            return View();
        }
    }
}