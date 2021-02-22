using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SocialMediaTwitterProject.Domain.Entities.Interface;
using SocialMediaTwitterProject.Domain.Repositories.BaseRepo;
using SocialMediaTwitterProject.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Infrastructure.Repositories.BaseRepo
{
   public abstract class BaseRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly ApplicationDbContext _context;
        protected DbSet<T> _table;

        public BaseRepository(ApplicationDbContext context)
        {
            this._context = context;
            this._table = _context.Set<T>();
        }

        public async Task Add(T entity) => await _table.AddAsync(entity);

        public async Task<bool> Any(Expression<Func<T, bool>> expression) => await _table.AnyAsync(expression);

        public void Delete(T entity) => _table.Remove(entity);

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> expression) => await _table.Where(expression).FirstOrDefaultAsync();

        public async Task<List<T>> Get(Expression<Func<T, bool>> expression) => await _table.Where(expression).ToListAsync();

        public async Task<List<T>> GetAll() => await _table.ToListAsync();

        public async Task<T> GetById(int id) => await _table.FindAsync(id);

        public async Task<TResult> GetFilteredFirstOrDefault<TResult>(Expression<Func<T, TResult>> selector,
                                                                      Expression<Func<T, bool>> expression = null,
                                                                      Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
                                                                      Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                                                      bool disableTracing = true)//varlık üzerindeki değişikliklere bakıp savechanges() yolluyoruz.
        {
            IQueryable<T> query = _table; //ilk olarak bana gelecek tabloyu IQueryable<T> tipindeki query nesnesine ye atıyoruz.
            if (disableTracing) query = query.AsNoTracking();//Filtreleme yapıcağız,sadece get işlemi olacağı için tracking'i kapattık.
            if (include != null) query = include(query);//include null degilse eklenicek tabloyu ekliyoruz.
            if (expression != null) query = query.Where(expression);//expression/talep-,istek null değilse ilgili expression'a göre işlem yapıyoruz.
            if (orderby != null) return await orderby(query).Select(selector).FirstOrDefaultAsync();
            else return await query.Select(selector).FirstOrDefaultAsync();
        }

        public async Task<List<TResult>> GetFilteredList<TResult>(Expression<Func<T, TResult>> selector,
                                                                  Expression<Func<T, bool>> expression = null,
                                                                  Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
                                                                  Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                                                  bool disableTracing = true,
                                                                  int pageIndex = 1,
                                                                  int pageSize = 3)
        {
            IQueryable<T> query = _table;
            if (disableTracing) query = query.AsNoTracking(); //https://docs.microsoft.com/en-us/ef/core/querying/tracking
            //AsNoTracking; Entity Framework tarafından uygulamaların performansını optimize etmemize yardımcı olmak için geliştirilmiş bir fonksiyondur. İşlevsel olarak veritabanından sorgu neticesinde elde edilen nesnelerin takip mekanizması ilgili fonksiyon tarafından kırılarak, sistem tarafından izlenmelerine son verilmesini sağlamakta ve böylece tüm verisel varlıkların ekstradan işlenme yahut lüzumsuz depolanma süreçlerine maliyet ayrılmamaktadır.
            if (include != null) query = include(query);//sorguya dahil olacak tabloların eklemesi için (eager loading)
            if (expression != null) query = query.Where(expression); // esnek fileteleme mekanizması
            if (orderby != null) return await orderby(query).Select(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(); //sıralama kriteri var ve ona göre gird oluşturulacaktır.
            else return await query.Select(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();//bir sıralama kriteri yoktur
        }

        public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;

    }
}

