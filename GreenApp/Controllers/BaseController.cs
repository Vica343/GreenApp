using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GreenApp.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IGreenService _greenService;

        public BaseController(IGreenService greenService)
        {
            _greenService = greenService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);           
        }
    }
}