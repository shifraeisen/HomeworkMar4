using HomeworkMar4.Data;

namespace HomeworkMar4.Web.Models
{
    public class ContributionsViewModel
    {
        public string SimchaName { get; set; }
        public int SimchaID { get; set; }
        public List<Contributor> Contributors { get; set; }
        public List<Contribution> CurrentContributions { get; set; }
    }
}
