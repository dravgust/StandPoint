using System;
using System.Collections.Generic;

namespace StandPoint.Models
{
    public class UserEntity : IEntityBase<int>
    {
        public int Id { get; set; }
        public bool HasId => this.Id > 0;
        public string Name { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public bool IsLocket { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ICollection<UserRoleEntity> Roles { get; set; }

        public UserEntity()
        {
            Roles = new List<UserRoleEntity>();
        }
    }
}
