using Microsoft.EntityFrameworkCore.Query;
using SocialMediaTwitterProject.Domain.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaTwitterProject.Domain.Repositories.BaseRepo
{
    public interface IRepository<T> where T: class, IBaseEntity //Diğer interfacelere kalıtım vericeğimiz için T type geçiyoruz.
    {
        Task<List<T>> GetAll();
        Task<List<T>> Get(Expression<Func<T, bool>> expression);  //Şarta göre veri çekme işlemi için Get methodunu yazdık.
        Task<T> GetById(int id);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> expression);
        Task<bool> Any(Expression<Func<T, bool>> expression);

        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        //Filtremeler için yani bir sorguya birden cok tablonun girmesi için ve ayarlanmaların yapılması için yazdığımız filtreleme methodları:

        //Bir sorguya 1'den cok tablonun girmesi işlemi için yazdığımız ve 1'den cok Filtreleme işlemini yapan method.
        Task<TResult> GetFilteredFirstOrDefault<TResult>(//Tekil olarak getirme işlemi için bu methodu yazdık.
            Expression<Func<T, TResult>> selector,//selector(seçici) bize bir ya da birden cok filtrelemeyi sağlar.
            Expression<Func<T, bool>> expression = null,//filtreleme yaparken kullanmama durumuna karşı nullable olarak işaretliyoruz.
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,//orderby ile A-z,fiyatına göre,yılına göre gibi filtreleme ile sıralama için kullanıyoruz.
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,//Filtreleme işlemine 1 den fazla tablo girme ihtimaline karşın o tabloları include ediyoruz.
            bool disableTracing = true);

        Task<List<TResult>> GetFilteredList<TResult>(//Listeleyerek filtreleme işlemi yapmak için bu methodu yazdık.
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracing = true,//EFCore ile gelen bu özellik değişiklikleri takip etme ve izleme işini yapar.
            int pageIndex = 1,//Kullanıcı isteğine göre sayfanın kaç tane twit görmek istediğini seçmesini sağladık.Bu işlemde pagination yapmış oluyoruz.
            int pageSize = 3);
    }
}

