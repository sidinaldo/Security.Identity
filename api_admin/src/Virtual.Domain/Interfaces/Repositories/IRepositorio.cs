using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Virtual.Domain.Models;

namespace Virtual.Domain.Interfaces
{
    public interface IRepositorio<TEntity> : IDisposable where TEntity : Entity
    {
        Task Criar(TEntity entity);
        Task<TEntity> ObterId(int id);
        Task<List<TEntity>> Obter();
        Task Atualizar(TEntity entity);
        Task Remover(int id);
        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        Task<int> SaveChanges();
        IDbContextTransaction AbrirTransacao();
        void AbortarTransacao(IDbContextTransaction transacao);
        void ComitarTransacao(IDbContextTransaction transacao);
    }
}
