using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GreenApp.Model;
using GreenApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace GreenApp.Controllers
{
    public class ChallengesController : BaseController
    {
        private readonly UserManager<Guest> _userManager;

        public ChallengesController(IGreenService greenService, ApplicationState applicationState, UserManager<Guest> userManager)
            : base(greenService, applicationState)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var challanges = _greenService.Challenges;
            //return View("Index", challanges.ToList());
            return View("AddChallenge");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // védelem XSRF támadás ellen
        public async Task<IActionResult> Add(ChallengeViewModel challenge)
        {
            if (challenge == null)
                return RedirectToAction("Index", "Home");
            


            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);
            
            if (!await _greenService.SaveChallengeAsync(guest.UserName, challenge))
            {
                ModelState.AddModelError("", "A foglalás rögzítése sikertelen, kérem próbálja újra!");
                return View("Index");
            }

            ViewBag.Message = "A foglalását sikeresen rögzítettük!";
            if (challenge.ChallengeSelectedType == Data.ChallengeType.QRCode)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    string inputText = Request.Scheme + "://" + Request.Host.Value + "/api/Challenges/QR/" + _greenService.GetChallenge(challenge).Id;
                    QRCodeGenerator oQRCodeGenerator = new QRCodeGenerator();
                    QRCodeData oQRCodeData = oQRCodeGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                    QRCode oQRCode = new QRCode(oQRCodeData);
                    using (Bitmap oBitmap = oQRCode.GetGraphic(20))
                    {
                        oBitmap.Save(ms, ImageFormat.Png);
                        challenge.ChallengeQr = ms.ToArray();

                        if (!_greenService.UpdateChallange(challenge))
                        {
                            ModelState.AddModelError("", "A foglalás rögzítése sikertelen, kérem próbálja újra!");
                            return View("Index");
                        }
                        ViewBag.QrCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            return View("Result", challenge);
        }


        [HttpGet]
        public IActionResult QRGenerate(ChallengeViewModel challange, int id)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                string inputText = Request.Scheme + "://" + Request.Host.Value + "/api/Challenges/QR/" + id;
                QRCodeGenerator oQRCodeGenerator = new QRCodeGenerator();
                QRCodeData oQRCodeData = oQRCodeGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                QRCode oQRCode = new QRCode(oQRCodeData);
                using (Bitmap oBitmap = oQRCode.GetGraphic(20))
                {
                    oBitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QrCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

            return View("AddChallenge", challange);
        }

        public async Task<IActionResult> Save(ChallengeViewModel challenge)
        {
            var id = _greenService.GetChallenge(challenge).Id;
            var file = await _greenService.SaveQRAsync(id);
            return File(file, "image/png");
        }


        public ActionResult ChallengeImage(Int32? challangeId)
        {
            Byte[] imageContent = _greenService.GetChallangeImage(challangeId);

            if(imageContent == null)
            {
                return Content("No file name provided");
            }

            return File(imageContent, "image/png");
        }
    }
}
