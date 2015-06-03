namespace EasyCrud.Models
{
    public interface IModelDataExchange<TRecordType, TIndexType> where TRecordType : IModel<TIndexType>, new()
    {
        bool IsValidForCreation();
        bool IsValidForEditing();
        void InitializeFromModel(TRecordType input);
        void UpdateModel(TRecordType input);
        TIndexType GetKey();
        void SetKey(TIndexType value);
        bool ContentsEqual(IModelDataExchange<TRecordType, TIndexType> input);
    }
}
