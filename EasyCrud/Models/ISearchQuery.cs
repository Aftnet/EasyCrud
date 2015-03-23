namespace EasyCrud.Models
{
    public interface ISearchQuery
    {
        int NumElementsToTake();
        int NumElementsToSkip();
    }
}
