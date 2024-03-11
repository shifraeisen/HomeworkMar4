using HomeworkMar4.Data;

namespace HomeworkMar4.Web.Models
{
    public class HistoryViewModel
    {
        public string Name { get; set; }
        public decimal CurrentBalance { get; set; }
        public List<Actn> Actions { get; set; }
    }
}
