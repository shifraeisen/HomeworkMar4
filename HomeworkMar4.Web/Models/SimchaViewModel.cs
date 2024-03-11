using HomeworkMar4.Data;

namespace HomeworkMar4.Web.Models
{
    public class SimchaViewModel
    {
        public Simcha Simcha { get; set; }
        public int ContributorCount { get; set; }
        public int ActiveContributors {  get; set; }
        public decimal Total {  get; set; }
    }
}
