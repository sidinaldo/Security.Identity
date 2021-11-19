using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Virtual.Data.Context;

using Virtual.Domain.Interfaces;
using Virtual.Domain.Models;

namespace Virtual.Data.Repository
{
    public abstract class Repositorio<TEntity> : IRepositorio<TEntity> where TEntity : Entity, new()
    {
        protected readonly VirtualContext Db;
        private readonly DbSet<TEntity> DbSet;
  

        public Repositorio(VirtualContext db)
        {
            Db = db;
            DbSet = db.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> Buscar(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = DbSet;
            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<TEntity>> Obter()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity> ObterId(int id)
        {
            return await DbSet.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
        }

        public virtual async Task Criar(TEntity entity)
        {
            await DbSet.AddAsync(entity);
            await SaveChanges();
        }

        public virtual async Task Atualizar(TEntity entity)
        {
            DbSet.Update(entity);
            await SaveChanges();
        }

        public virtual async Task Remover(int id)
        {
            DbSet.Remove(new TEntity { Id = id });
            await SaveChanges();
        }

        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Db?.Dispose();
        }

        public IDbContextTransaction AbrirTransacao()
        {
            return Db.Database.BeginTransaction();
        }

        public void AbortarTransacao(IDbContextTransaction transacao)
        {
            transacao.Rollback();
        }

        public void ComitarTransacao(IDbContextTransaction transacao)
        {
            transacao.Commit();
        }
    }
}
