using System;
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
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "companyAdmin")]
        [HttpGet]
        public IActionResult Details(int id)
        {           
            var challange = _greenService.Challenges.Where(i => i.Id == id).FirstOrDefault();
            if (challange.Reward == RewardType.Cupon)
            {
                ViewBag.cuponname = _greenService.Cupons.Where(c => c.Id == challange.RewardValue).FirstOrDefault().Name;
            }
          
            return View("CompanyAdmin/Details", challange);            
        }

        [Authorize(Roles = "superAdmin")]
        [HttpGet]
        public IActionResult Index()
        {
            var challanges = _greenService.ChallengesWithCreator.ToList();
            return View("Superadmin/Index", challanges);
        }

        [Authorize(Roles = "superAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(Int32? challengeId)
        {
            if (!await _greenService.DisableChallengeAsync(challengeId))
            {
                TempData["ErrorMessage"] = "A kihívás letiltása sikertelen, próbálja újra!";
                return RedirectToAction("Details", "Challenges");
            }

            TempData["Success"] = "A kihívás sikeresen letiltva!";

            return RedirectToAction("Index", "Challenges");
        }
        

        [Authorize(Roles = "superAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enable(Int32? challengeId)
        {
            if (!await _greenService.EnableChallengeAsync(challengeId))
            {
                TempData["ErrorMessage"] = "A kihívás engedélyezése sikertelen, próbálja újra!";
                return RedirectToAction("Details", "Challenges");
            }

            TempData["Success"] = "A kihívás sikeresen engedélyezve!";

            return RedirectToAction("Index", "Challenges");
        }


        [Authorize(Roles = "companyAdmin")]
        [HttpGet]
        public async Task<IActionResult> OwnCampaigns()
        {
            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);
            var challanges = _greenService.GetOwnChallenges(guest.Id).ToList();
            return View("CompanyAdmin/Index", challanges);
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpGet]
        public async Task<IActionResult> OtherCampaigns()
        {
            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);
            var challanges = _greenService.GetOtherChallenges(guest.Id).ToList();
            return View("CompanyAdmin/OtherChallenges", challanges);
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Int32? challengeId)
        {
            var challange = _greenService.GetChallengeById(challengeId);
            var stream = new MemoryStream(challange.Image);
            IFormFile file = new FormFile(stream, 0, challange.Image.Length, "name", "fileName");

            ChallengeViewModel challengeview = new ChallengeViewModel
            {
                ChallengeName = challange.Name,
                ChallengeDescription = challange.Description,
                ChallengeStartDate = challange.StartDate,
                ChallengeEndDate = challange.EndDate,
                ChallengeImage = file,
                ChallengeQr = challange.QRCode,
                ChallengeSelectedType = challange.Type,
                ChallengeReward = challange.Reward,
                ChallengeRewardValue = challange.RewardValue
            };

            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);
            var cupons = _greenService.GetOwnCupons(guest.Id).ToList();
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
            
            
            ViewBag.cupons = cupons.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.Name,
                    Value = a.Id.ToString()
                };
            }
                );

            ViewBag.rewards = rewards;

            ViewBag.types = types;

            ViewBag.image = _greenService.GetChallenge(challengeview).Id;

            return View("CompanyAdmin/Edit", challengeview);
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ChallengeViewModel challenge, Int32? id)
        {
            if (challenge == null)
                return RedirectToAction("Index", "Home");

            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);

            string inputText = Request.Scheme + "://" + Request.Host.Value + "/api/Challenges/QR/";

            if (!await _greenService.UpdateChallengeAsync(guest.UserName, challenge, id, inputText))
            {
                TempData["ErrorMessage"] = "A kihívás módosítása sikertelen, próbáld újra!";
                return RedirectToAction("Details", "Challenges");
            }

            TempData["Success"] = "A kihívás sikeresen módosítva!";

            return RedirectToAction("Details", "Challenges");
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Int32? id)
        {
            if (!await _greenService.DeleteChallengeAsync(id))
            {
                TempData["ErrorMessage"] = "A kihívás törlése sikertelen! Ellenőrizd, hogy nincs-e már beküldött megoldás!";
                return RedirectToAction("OwnCampaigns", "Challenges");
            }

            TempData["Success"] = "A kihívás sikeresen törölve!";


            return RedirectToAction("OwnCampaigns", "Challenges");
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpGet]
        public async Task<IActionResult> AddChallenge()
        {
            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);
            var cupons = _greenService.GetOwnCupons(guest.Id).ToList();
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

            ViewBag.cupons = cupons.ConvertAll(a =>
           {
               return new SelectListItem()
               {
                   Text = a.Name,
                   Value = a.Id.ToString()
               };
           }
                );
           
              
            ViewBag.rewards = rewards;

            ViewBag.types = types;
            return View("CompanyAdmin/AddChallenge");
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpGet]
        public IActionResult Participants(int challengeid)
        {
            var solutions = _greenService.GetSolutions(challengeid).ToList();

            if (solutions.Count == 0)
            {
                ViewBag.Message = "Erre a kihívásra még nem érkezett megoldás.";
            }
         
            return View("CompanyAdmin/Participants", solutions);
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Add(ChallengeViewModel challenge)
        {
            if (challenge == null)
                return RedirectToAction("Index", "Home");          

            Guest guest = await _userManager.FindByNameAsync(User.Identity.Name);
            
            if (!await _greenService.SaveChallengeAsync(guest.UserName, challenge))
            {
                TempData["ErrorMessage"] = "A kihívás létrehozása sikertelen, kérem próbálja újra!";
                return RedirectToAction("AddChallenge", "Challenges");
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
                            TempData["ErrorMessage"] = "A kihívás létrehozása sikertelen, kérem próbálja újra!";
                            return RedirectToAction("AddChallenge", "Challenges");
                        }
                       
                    }
                }
            }
            TempData["Success"] = "A kihívás sikeresen hozzáadva!";

            return RedirectToAction("OwnCampaigns", "Challenges");
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpGet]
        public async Task<IActionResult> Accept(Int32? challengeId, Int32? userId)
        {
            if (!await _greenService.AcceptChallengeSolution(challengeId, userId)) 
            {
                TempData["ErrorMessage"] = "A megoldás elfogadása sikertelen, kérem próbálja újra!";
                return RedirectToAction("Participants", "Challenges");
            }

            TempData["Success"] = "A megoldás sikeresen elfogadva!";

            return RedirectToAction("Participants", "Challenges");
        }

        [Authorize(Roles = "companyAdmin")]
        [HttpGet]
        public async Task<IActionResult> Decline(Int32? challengeId, Int32? userId)
        {
            if (!await _greenService.DeclineChallengeSolution(challengeId, userId))
            {
                TempData["ErrorMessage"] = "A megoldás elutasítása sikertelen, kérem próbálja újra!";
                return RedirectToAction("Participants", "Challenges");
            }

            TempData["Success"] = "A megoldás sikeresen elutasítva!";

            return RedirectToAction("Participants", "Challenges");
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
