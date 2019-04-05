using System;

namespace StandPoint.Models
{
    public class ErrorEntity : IEntityBase<int>
    {
        public int Id { get; set; }
        public bool HasId => this.Id > 0;
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
