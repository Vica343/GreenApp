﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GreenApp.Data;
using GreenApp.Model;
using GreenApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult Details(int id)
        {
            var challange = _greenService.Challenges.Where(i => i.Id == id).FirstOrDefault();
            return View("Details", challange);            
        }

        [HttpGet]
        public IActionResult Index()
        {
            var challanges = _greenService.Challenges.ToList();
            return View("Index", challanges);
        }

        [HttpGet]
        public IActionResult AddChallenge()
        {
            var cupons = _greenService.Cupons.ToList();
            var rewards = new SelectList(Enum.GetValues(typeof(RewardType)).Cast<RewardType>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList(), "Value", "Text");
            var types = new SelectList(Enum.GetValues(typeof(ChallengeType)).Cast<ChallengeType>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList(), "Value", "Text");

            ViewBag.cupons = new SelectList(cupons, "Id", "Name");
            ViewBag.rewards = rewards;
            ViewBag.types = types;
            return View("AddChallenge");
        }

        [HttpGet]
        public IActionResult Participants(int challengeid)
        {
            var solutions = _greenService.GetSolutions(challengeid).ToList();
         
            return View("Participants", solutions);
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
                ModelState.AddModelError("", "A kihívás létrehozása sikertelen, kérem próbálja újra!");
                return View("AddChallenge");
            }

            ViewBag.Message = "A kihívást sikeresen rögzítettük!";
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
                            ModelState.AddModelError("", "A kihívás létrehozása sikertelen, kérem próbálja újra!");
                            return View("AddChallenge");
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

        [HttpGet]
        public async Task<IActionResult> Accept(Int32? challengeId, Int32? userId)
        {
            if (!await _greenService.AcceptChallengeSolution(challengeId, userId)) 
            {
                ModelState.AddModelError("", "A kihívás elfogadása sikertelen, kérem próbálja újra!");
                return View("Participants");
            }

            var solutions = _greenService.GetSolutions(challengeId).ToList();
            ViewBag.Message = "A kihívás megoldása elfogadva.";
            return View("Participants", solutions);
        }

        [HttpGet]
        public async Task<IActionResult> Decline(Int32? challengeId, Int32? userId)
        {
            if (!await _greenService.DeclineChallengeSolution(challengeId, userId))
            {
                ModelState.AddModelError("", "A kihívás elutasítása sikertelen, kérem próbálja újra!");
                return View("Participants");
            }

            var solutions = _greenService.GetSolutions(challengeId).ToList();
            ViewBag.Message = "A kihívás megoldása elutasítva.";
            return View("Participants", solutions);
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

        public ActionResult ChallengeSolutionImage(Int32? challengeId, Int32? userId)
        {
            Byte[] imageContent = _greenService.GetChallangeSolutionImage(challengeId, userId);

            if (imageContent == null)
            {
                return Content("No file name provided");
            }

            return File(imageContent, "image/png");
        }

        public ActionResult QRImage(Int32? challangeId)
        {
            Byte[] imageContent = _greenService.GetQRImage(challangeId);

            if (imageContent == null)
            {
                return Content("No file name provided");
            }

            return File(imageContent, "image/png");
        }
    }
}