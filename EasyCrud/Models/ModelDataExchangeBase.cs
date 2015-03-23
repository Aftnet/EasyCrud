namespace EasyCrud.Models
{
    public abstract class ModelDataExchangeBase<TRecordType, TIndexType> : IModelDataExchange<TRecordType, TIndexType> where TRecordType : IModel<TIndexType>, new()
    {
        public abstract bool IsValidForCreation();
        public abstract bool IsValidForEditing();
        public abstract void InitializeFromModel(TRecordType input);
        public abstract void UpdateModel(TRecordType input);

        public virtual bool ContentsEqual(IModelDataExchange<TRecordType, TIndexType> input)
        {
            var inputRecord = new TRecordType();
            input.UpdateModel(inputRecord);
            var record = new TRecordType();
            UpdateModel(record);
            var result = inputRecord.ContentsEqual(record);
            return result;
        }
    }
}