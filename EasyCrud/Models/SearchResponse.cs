using System.Collections.Generic;

namespace EasyCrud.Models
{
    public class SearchResponse<TResultType>
    {
        public IEnumerable<TResultType> Results { get; set; }
        public int TotalResults { get; set; }
        public int Skipped { get; set; }
    }
}
