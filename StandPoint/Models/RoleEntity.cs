namespace StandPoint.Models
{
    public class RoleEntity : IEntityBase<int>
    {
        public int Id { get; set; }
        public bool HasId => this.Id > 0;
        public string Name { get; set; }
    }
}
