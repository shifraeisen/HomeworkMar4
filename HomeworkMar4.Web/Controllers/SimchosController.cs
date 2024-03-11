using HomeworkMar4.Data;
using HomeworkMar4.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HomeworkMar4.Web.Controllers
{
    public class SimchosController : Controller
    {
        private string _conStr = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund; Integrated Security=true;";
        public IActionResult Index()
        {
            SimchaFundDbMngr mgr = new(_conStr);
            SimchosViewModel vm = new();
            List<Simcha> simchos = mgr.GetAllSimchos();
            foreach (var s in simchos)
            {
                vm.SimchosInfo.Add(new SimchaViewModel
                {
                    Simcha = s,
                    ContributorCount = mgr.GetTotalContributorCount(),
                    ActiveContributors = mgr.GetContributorCountForSimcha(s.Id),
                    Total = mgr.GetTotalforSimcha(s.Id)
                });
            }
            ViewBag.Message = (string)TempData["success-message"] != null ? (string)TempData["success-message"] : null;
            return View(vm);
        }
        [HttpPost]
        public IActionResult AddSimcha(Simcha s)
        {
            if (s != null)
            {
                SimchaFundDbMngr mgr = new(_conStr);
                mgr.AddSimcha(s);
                TempData["success-message"] = "New Simcha Created";
            }
            return Redirect("/simchos");
        }
        public IActionResult Contributions(int simchaID)
        {
            SimchaFundDbMngr mgr = new(_conStr);
            ContributionsViewModel vm = new()
            {
                SimchaName = mgr.GetSimchaNameByID(simchaID),
                SimchaID = simchaID,
                Contributors = mgr.GetContributors(),
                CurrentContributions = mgr.GetContributionsForSimcha(simchaID)
            };
            return View(vm);
            
        }
        [HttpPost]
        public IActionResult AddContributions(List<Contribution> conts, List<int> contIDs)
        {
            conts = conts.Where(c => contIDs.Contains(c.ContributorID)).ToList();
            if(conts.Count > 0)
            {
                SimchaFundDbMngr mgr = new(_conStr);
                mgr.AddContributions(conts);
            }
            TempData["success-message"] = "Simcha Updated Successfully";
            return Redirect("/simchos");
        }
    }
}