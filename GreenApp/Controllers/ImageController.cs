using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GreenApp.Model;
using GreenApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GreenApp.Controllers
{
    public class ImageController : BaseController
    {
        private readonly UserManager<Guest> _userManager;

        public ImageController(IGreenService greenService, ApplicationState applicationState, UserManager<Guest> userManager)
           : base(greenService, applicationState)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var model = _greenService.GetImage(1);
            var viewModel = new ImageUploadViewModel();
            viewModel.CreatedOn = model.CreatedOn;
            viewModel.Name = model.Name;
            viewModel.UploadedBy = model.UploadedBy;
            viewModel.Data = model.Data;
            viewModel.FileType = model.FileType;
            viewModel.Extension = model.Extension;

            return View("Index", viewModel); ;
        }

        [HttpPost]
        public async Task<IActionResult> UploadToDatabase(IFormFile file)
        {
            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            var fileModel = new ImageUploadViewModel
            {
                 CreatedOn = DateTime.UtcNow,
                 Name = fileName,
                 FileType = file.ContentType,
                 UploadedBy = guest.Id,
                 Extension = extension
            };
            using (var dataStream = new MemoryStream())
            {
                  await file.CopyToAsync(dataStream);
                  fileModel.Data = dataStream.ToArray();
            }

            if (!await _greenService.SaveImageAsync(guest.UserName, fileModel))
            {
                ModelState.AddModelError("", "A feltöltés rögzítése sikertelen, kérem próbálja újra!");
                return View("Index");
            }

            TempData["Message"] = "File successfully uploaded to Database";
            return RedirectToAction("Index");
        }

        [HttpGet]
        private IActionResult LoadFile()
        {
            var model = _greenService.GetImage(1);
            var viewModel = new ImageUploadViewModel();
            viewModel.CreatedOn = model.CreatedOn;
            viewModel.Name = model.Name;
            viewModel.UploadedBy = model.UploadedBy;
            viewModel.Data = model.Data;
            viewModel.FileType = model.FileType;
            viewModel.Extension = model.Extension;

            return View("Details", viewModel); ;
        }

        public async Task<IActionResult> DownloadFileFromDatabase(Int32? id)
        {
            var file = await _greenService.DownLoadFileAsync(id);
            return File(file.Data, file.FileType, file.Name + file.Extension);
        }

    }
}