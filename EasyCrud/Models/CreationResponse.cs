namespace EasyCrud.Models
{
    public class CreationResponse<TIndexType>
    {
        public TIndexType Id { get; set; }

        public CreationResponse() { }

        public CreationResponse(TIndexType id)
        {
            Id = id;
        }
    }
}
