using HomeworkMar4.Data;
using HomeworkMar4.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeworkMar4.Web.Controllers
{    
    public class ContributorsController : Controller
       
    {
        private string _conStr = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund; Integrated Security=true;";
        public IActionResult Index()
        {
            SimchaFundDbMngr mgr = new(_conStr);
            ViewBag.Message = (string)TempData["success-message"] != null ? (string)TempData["success-message"] : null;

            return View(new ContributorsViewModel
            {
                Contributors = mgr.GetContributors(),
                Total = mgr.GetTotal()
            });
        }
        [HttpPost]
        public IActionResult AddContributor(Contributor c, decimal depositAmount)
        {
            if (c != null)
            {
                SimchaFundDbMngr mgr = new(_conStr);
                mgr.AddContributor(c, depositAmount);
                TempData["success-message"] = "New Contributor Created";
            }
            return Redirect("/contributors");
        }
        [HttpPost]
        public IActionResult Deposit(int contributorID, decimal depositAmount, DateTime date)
        {
            if (depositAmount != 0)
            {
                SimchaFundDbMngr mgr = new(_conStr);
                mgr.AddDeposit(contributorID, depositAmount, date);
                TempData["success-message"] = "Deposit Successfully Recorded";
            }
            return Redirect("/contributors");
        }
        public IActionResult History(int contID)
        {
            SimchaFundDbMngr mgr = new(_conStr);
            HistoryViewModel vm = new()
            {
                Name = mgr.GetContributorNameByID(contID),
                CurrentBalance = mgr.GetBalance(contID),
                Actions = mgr.GetActions(contID).OrderByDescending(a => a.Date).ToList()
            };

            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(Contributor c)
        {
            SimchaFundDbMngr mgr = new(_conStr);
            mgr.EditContributor(c);
            TempData["success-message"] = "Contributor Updated Successfully";
            return Redirect("/contributors");
        }
    }
}
