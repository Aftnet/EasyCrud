using System.Linq;

namespace ApiBase.Models
{
    public interface IModelSearch<TRecordType>
    {
        IQueryable<TRecordType> GenerateSearchQuery(IQueryable<TRecordType> input);
        int NumElementsToTake();
        int NumElementsToSkip();
    }
}
