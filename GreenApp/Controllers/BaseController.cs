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
        protected readonly ApplicationState _applicationState;

        public BaseController(IGreenService greenService, ApplicationState applicationState)
        {
            _greenService = greenService;
            _applicationState = applicationState;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            ViewBag.CurrentGuestName = String.IsNullOrEmpty(User.Identity.Name) ? null : User.Identity.Name;
        }
    }
}