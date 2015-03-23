namespace EasyCrud.Models
{
    public interface IModelDataExchange<TRecordType, TIndexType> where TRecordType : IModel<TIndexType>, new()
    {
        bool IsValidForCreation();
        bool IsValidForEditing();
        void InitializeFromModel(TRecordType input);
        void UpdateModel(TRecordType input);
        bool ContentsEqual(IModelDataExchange<TRecordType, TIndexType> input);
    }
}
