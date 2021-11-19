using Virtual.Data.Context;
using Virtual.Domain.Interfaces.Repositories;
using Virtual.Domain.Models;

namespace Virtual.Data.Repository
{
    public class TimeRepositorio : Repositorio<Time>, ITimeRepositorio
    {
        public TimeRepositorio(VirtualContext db) : base(db)
        { }
    }
}
