using System;
using System.Linq;

namespace MileageStats.Web.Models
{
    public class FillupListViewModel
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public IGrouping<Tuple<int, string>, FillupViewModel> Fillups { get; set; } 
    }
}