using System.Linq;

namespace EasyCrud.Models
{
    public interface IModelSearchQuery<TRecordType> : ISearchQuery
    {
        IQueryable<TRecordType> GenerateSearchQuery(IQueryable<TRecordType> input);
    }
}