namespace StandPoint.Models
{
    public class UserRoleEntity : IEntityBase<int>
    {
        public int Id { get; set; }
        public bool HasId => this.Id > 0;
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public virtual RoleEntity Role { get; set; }
    }
}
