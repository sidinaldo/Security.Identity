using System;

namespace Virtual.Domain.Models
{
    public class Entity
    {
        public int Id { get; set; }
        public Guid IdUsuarioCadastro { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
