namespace StandPoint.Models
{
    public interface IEntityBase<T>
    {
        T Id { get; set; }
        bool HasId { get; }
    }
}
