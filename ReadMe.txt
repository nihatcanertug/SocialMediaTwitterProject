



1. SocialMediaTwitterProject ad�nda bir Blank Solution a��l�r.

2.1 DDD yap�s�na uygun  ilk olarak SocialMediaTwitterProject.Domain ad�nda Class library (.NetCore)  Projesi a��l�r.

	2.2. Enums,Entities,Repositories ve UnitOfWork klas�rleri a��l�r.

	2.3. Enums klas�r�n�n i�ine Status s�n�f� eklenir.

		public enum Status { Active = 1, Modified = 2, Passive = 3 }

	2.4. Entities klas�r�n�n i�ine Concrete ve Interface klas�rleri a��l�r ve interface'e IBaseEntity interface'i eklenir.

	 public interface IBaseEntity
    {
        DateTime CreateDate { get; set; }
        DateTime? ModifiedDate { get; set; }
        DateTime? DeleteDate { get; set; }
        Status Status { get; set; }
    }

    2.5. Concrete klas�r� a��l�r ve AppRole isminde bir s�n�f a��larak kal�t�m olarak IdentityRole s�n�f� verilir.Bunun i�in usinglere Microsoft.AspNetCore.Identity paketi y�klenir.Son olarak IBaseEntity kal�t�m verilerek baz� propertylere Encapsulation i�lemi ger�ekle�tirilir.

     public class AppRole:IdentityRole<int>,IBaseEntity
    {
        private DateTime _createDate = DateTime.Now;
        public DateTime CreateDate { get => _createDate; set => _createDate = value; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        private Status _status = Status.Active;
        public Status Status { get => _status; set => _status = value; }
    }
    2.6. S�ras�yla AppUser,Follow,Like,Mention,Share,Tweet varl�k s�n�flar� Concrete klas�r�ne eklenir.

    2.7. Repositories klas�r�ne BaseRepo ve EntityTypeRepo ad�nda 2 klas�r a��l�r ve Base reponun i�ine  b�t�n CRUD operasyonlar�nda kullanaca��m�z method'lar� yazaca��m�z IRepository interface'ini a��yoruz.

    public interface IRepository<T>where T : class,IBaseEntity //di�er interfacelere kal�t�m verice�imiz i�in T type ge�iyoruz.
    {
        Task<List<T>> GetAll();
        Task<List<T>> Get(Expression<Func<T,bool>>expression);
        Task<T> GetById(int id);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> expression);
        Task<bool> Any(Expression<Func<T, bool>> expression);
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<TResult> GetFilteredFirstOrDefault<TResult>(
               Expression<Func<T, TResult>> selector,
               Expression<Func<T, bool>> expression = null,
               Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
               Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
               bool disableTracing = true);
        Task<List<TResult>> GetFilteredList<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracing = true,
            int pageIndex = 1,
            int pageSize = 3);
    }
    2.8. EntityTypeRepo klas�r� a�m��t�k DIP gere�i varl�klar�n interfacelerini a��caz ve T type olarak belirledi�imiz i�in Concrete'leri verece�iz b�ylece Ba��ml�l��� tersine d�nd�rm�� olaca��z.

    public interface IAppUserRepository : IRepository<AppUser> { }

    2.9. UnitOfWork klas�r� a��l�r ve i�ine IUnitOfWork interface'i a��l�r.Amac�m�z i�erisine bu projede UnitOfWork pattern'�na dahil etmek istedi�imiz repository'leri yaz�ca��z.

     public interface IUnitOfWork: IAsyncDisposable
    {
        //Repository'lerde a�t���m�z ve kullanmak istedi�imiz interfaceleri ekliyoruz.
        IAppUserRepository AppUserRepository { get; }
        IFollowRepository FollowRepository { get; }
        ILikeRepository LikeRepository { get; }
        IMentionRepository MentionRepository { get; }
        IShareRepository ShareRepository { get; }
        ITweetRepository TweetRepository { get; }

        Task Commit(); //Ba�ar�l� bir i�lemin sonucunda �al��t�r�l�r.��lemin ba�lamas�ndan itibaren t�m de�i�ikliklerin veri taban�na uygulanmas�n� temin eder.
        Task ExecuteSqlRaw(string sql, params object[] parameters); //Mevcut sql sorgular�m�z� do�rudan veri taban�nda y�r�tmek i�in kullan�lan bir method.
    }

3. DDD yap�s�na uygun ikinci olarak SocialMediaTwitterProject.Infrastructure ad�nda Class library (.NetCore)  Projesi a��l�r.
   
   3.1. S�ras�yla Context,Mapping,Repositories ve UnitOfWork klas�rleri a��l�r.

   3.2. Mapping klas�r�n�n i�ine Abstract ve Concrete klas�rleri a��l�r ve Abstract'a BaseMap s�n�f� eklenir.

    public abstract class BaseMap<T> : IEntityTypeConfiguration<T> where T : class, IBaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.CreateDate).IsRequired(true);
            builder.Property(x => x.ModifiedDate).IsRequired(false);
            builder.Property(x => x.DeleteDate).IsRequired(false);
            builder.Property(x => x.Status).IsRequired(true);
        }
    }

    3.3. Concrete s�n�f�na varl�klar�n hepsinin map'ini class olarak ekledik ve BaseMap'den kal�t�m verdik.

    public class AppUserMap : BaseMap<AppUser>
    {
        public override void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserName).IsRequired(true);
            builder.Property(x => x.NormalizedUserName).HasMaxLength(30);
            builder.Property(x => x.Name).HasMaxLength(30).IsRequired(true);
            builder.Property(x => x.ImagePath).HasMaxLength(30).IsRequired(true);

            builder.HasMany(x => x.Tweets).WithOne(x => x.AppUser).HasForeignKey(x => x.AppUserId);
            builder.HasMany(x => x.Likes).WithOne(x => x.AppUser).HasForeignKey(x => x.AppUserId);
            builder.HasMany(x => x.Mentions).WithOne(x => x.AppUser).HasForeignKey(x => x.AppUserId);
            builder.HasMany(x => x.Shares).WithOne(x => x.AppUser).HasForeignKey(x => x.AppUserId);

            builder.HasMany(x => x.Followers).WithOne(x => x.Follower).HasForeignKey(x => x.FollowerId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Followings).WithOne(x => x.Following).HasForeignKey(x => x.FollowingId).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }

    3.4. Context klas�r�ne ApplicationDbContext s�n�f� a��l�r.

     public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<Mention> Mentions { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TweetMap());
            builder.ApplyConfiguration(new MentionMap());
            builder.ApplyConfiguration(new ShareMap());
            builder.ApplyConfiguration(new LikeMap());
            builder.ApplyConfiguration(new FollowMap());
            builder.ApplyConfiguration(new AppUserMap());
            builder.ApplyConfiguration(new AppRoleMap());
            base.OnModelCreating(builder);
        }

        3.5. Domain katman�nda interfaceler i�erisinde tan�mlanm�� repository'ler burada concrete edilir.BaseRepo ve EntityTypeRepo ad�nda 2 klas�r a��l�r ve i�lerine repository class'lar� eklenir.BaseRepository di�er concrete s�n�flara kal�t�m verice�i i�in T type olarak eklenir.

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
                                                                      bool disableTracing = true)//varl�k �zerindeki de�i�ikliklere bak�p savechanges() yolluyoruz.
        {
            IQueryable<T> query = _table; //ilk olarak bana gelecek tabloyu IQueryable<T> ye at�yoruz.
            if (disableTracing) query = query.AsNoTracking();//Filtreleme yap�ca��z,sadece get i�lemi olaca�� i�in tracking'i kapatt�k.
            if (include != null) query = include(query);//include null degilse eklenicek tabloyu ekliyoruz.
            if (expression != null) query = query.Where(expression);
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
            //AsNoTracking; Entity Framework taraf�ndan uygulamalar�n performans�n� optimize etmemize yard�mc� olmak i�in geli�tirilmi� bir fonksiyondur. ��levsel olarak veritaban�ndan sorgu neticesinde elde edilen nesnelerin takip mekanizmas� ilgili fonksiyon taraf�ndan k�r�larak, sistem taraf�ndan izlenmelerine son verilmesini sa�lamakta ve b�ylece t�m verisel varl�klar�n ekstradan i�lenme yahut l�zumsuz depolanma s�re�lerine maliyet ayr�lmamaktad�r.
            if (include != null) query = include(query);//sorguya dahil olacak tablolar�n eklemesi i�in (eager loading)
            if (expression != null) query = query.Where(expression); // blog projesinde kulland���m�z esnek fileteleme mekanizmas�
            if (orderby != null) return await orderby(query).Select(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(); //s�ralama kriteri var ve ona g�re gird olu�turulacakt�r.
            else return await query.Select(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();//bir s�ralama kriteri yoktur
        }

        public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;

    }

    3.6. EntityTypeRepo klas�r�ne Concrete repository'ler eklenir ve interfaceleri Baserepository ile birlikte kal�t�m olarak eklenir.

     public class AppUserRepository:BaseRepository<AppUser>,IAppUserRepository
    {
        public AppUserRepository(ApplicationDbContext context):base(context){}
    }

    3.7. UnitOfWork klas�r� a��l�r. Domain katman�nda aray�z olarak tan�mlanm�� IUnitOfWork.cs s�n�f� burada concrete edilir.

     public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db) => this._db = db ?? throw new ArgumentNullException("database can not to be null");


        private IAppUserRepository _appUserRepository;
        public IAppUserRepository AppUserRepository
        {
            get { return _appUserRepository ?? (_appUserRepository = new AppUserRepository(_db)); }

            //Singleton 
            //get
            //{
            //    if (_appUserRepository == null) _appUserRepository = new AppUserRepository(_db);

            //    return _appUserRepository;
            //}
        }

        private IFollowRepository _followRepository;
        public IFollowRepository FollowRepository { get => _followRepository ?? (_followRepository = new FollowRepository(_db)); }

        private ILikeRepository _likeRepository;
        public ILikeRepository LikeRepository { get => _likeRepository ?? (_likeRepository = new LikeRepository(_db)); }

        private IMentionRepository _mentionRepository;
        public IMentionRepository MentionRepository { get => _mentionRepository ?? (_mentionRepository = new MentionRepository(_db)); }

        private IShareRepository _shareRepository;
        public IShareRepository ShareRepository { get => _shareRepository ?? (_shareRepository = new ShareRepository(_db)); }

        private ITweetRepository _tweetRepository;
        public ITweetRepository TweetRepository { get => _tweetRepository ?? (_tweetRepository = new TweetRepository(_db)); }

        public async Task Commit() => await _db.SaveChangesAsync();

        public async Task ExecuteSqlRaw(string sql, params object[] paramters) => await _db.Database.ExecuteSqlRawAsync(sql, paramters);

        private bool isDisposing = false;

        public async ValueTask DisposeAsync()
        {
            if (!isDisposing)
            {
                isDisposing = true;
                await DisposeAsync(true);
                GC.SuppressFinalize(this); //Nesnemizin tamam�yla temizlenmesini sa�layacak. (https://stackoverflow.com/questions/151051/when-should-i-use-gc-suppressfinalize)
            }
        }

        private async Task DisposeAsync(bool disposing)
        {
            if (disposing) await _db.DisposeAsync();
        }
    }

    4.   DDD yap�s�na uygun  ���nc� olarak SocialMediaTwitterProject.Application ad�nda Class Library (.Core) Projesi a��l�r.

        4.1. Models klas�r� a��l�r.Bu klas�r�n i�ine Data Transfer Objelerimiz ve View Models saklayaca��z.

        4.2. DTOs klas�r� a��l�r ve i�ine DTO classlar� eklenir.

        public class AddMentionDTO //Business i�lerimiz i�in DTO'lar olu�turuyoruz.
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int AppUserId { get; set; }
        public int TweetId { get; set; }
        public DateTime CreateDate { get; set; }
    }

     public class EditProfileDTO
    {
        //Business Domain ihtiya�lar�m�za g�re haz�rlad���m�z veri transfer objelerimiz ef ��renmeye ba�lad���m�z ilk g�nden beri kulland���m�z attribute baz�nda �artlar i�erebilirler. Eski projelerimizde �rne�in CMS projesinde bir prototype hem entity hemde DTO gibi kullan�yorduk.
        public int Id { get; set; }
        [Required(ErrorMessage = "You must to type into name")]
        public string Name { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
    }

     public class FollowDTO
    {
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
        public bool isExsist { get; set; }
    }

     public class LikeDTO
    {
        public int AppUserId { get; set; }
        public int TweetId { get; set; }
        public bool isExsist { get; set; }  //Bir daha like'layamad��� i�in �nceden like at�p atmad���n� kontrol ediyoruz.
    }

     public class LoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        //Remeber me
        //public bool RememberMe { get; set; }
    }

     public class ProfileSummaryDTO
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public int TweetCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }
        public string ImagePath { get; set; }
    }

        4.3. Mapper klas�r� a��l�r.Mapping s�n�f� a��l�r. Bu s�n�f Profile.cs s�n�f�ndan miras al�r. Lakin bu s�n�ftan yararlanamak i�in a�a��daki "AutoMapper" paketini y�klenmeniz gerekmektedir.AutoMapper & AutoMapper.Extensions.Microsoft.DependencyInjection paketleri y�klenilir.

        public Mapping()
        {
            CreateMap<AppUser, RegisterDTO>().ReverseMap();
            CreateMap<AppUser, LoginDTO>().ReverseMap();
            CreateMap<AppUser, EditProfileDTO>().ReverseMap();
            CreateMap<AppUser, ProfileSummaryDTO>().ReverseMap();

            CreateMap<Follow, FollowDTO>().ReverseMap();
            CreateMap<Like, LikeDTO>().ReverseMap();
            CreateMap<Mention, AddMentionDTO>().ReverseMap();
        }

        4.4 IoC klas�r� a��l�r ve i�ine DependencyInjection s�n�f� eklenir.Projedeki ba��ml� s�n�flar burada register ve resolve edilir. Burada built-in container i�erisinde bu conrainer'in bize verdi�i yap�lar ile register ve resolve i�lemlerimiz ger�ekle�tiricez.

         public static class DependencyInjection
    {
        public static IServiceCollection Register_ResolverServices(this IServiceCollection services)
        {
            //registration
            services.AddAutoMapper(typeof(Mapping));

            //reseolve
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IMentionService, MentionService>();
            services.AddScoped<ITweetService, ITweetService>();

            //"AddIdentity" s�n�f� i�in Microsoft.AspNetCore.Identity paketi indirilir.
            services.AddIdentity<AppUser, AppRole>(x=> {
                x.SignIn.RequireConfirmedAccount = false;
                x.SignIn.RequireConfirmedEmail = false;
                x.SignIn.RequireConfirmedPhoneNumber = false;
                x.User.RequireUniqueEmail = false;
                x.Password.RequiredLength = 3;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }
    }

        4.5. Services klas�r� a��l�r.

        4.6. Services => Interfaces klas�r� a��l�r. UI katman�nda kullan�lmak �zere i�lerimizi kar��layacak servis aray�zleri olu�turulur.

         public interface IAppUserService
    {
        Task DeleteUser(params object[] parameters);
        Task<IdentityResult> Register(RegisterDTO registerDTO);
        Task<SignInResult> LogIn(LoginDTO loginDTO);
        Task LogOut();


        Task<int> GetUserIdFromName(string name);
        Task<EditProfileDTO> GetById(int id);
        Task EditUser(EditProfileDTO editProfileDTO);
        Task<ProfileSummaryDTO> GetByUserName(string userName);

        Task<List<FollowListVM>> UsersFollowers(int id, int pageIndex);
        Task<List<FollowListVM>> UsersFollowings(int id, int pageIndex);
    }

	    4.7. Services => Concrete klas�r� a��l�r. Olu�turulan servisler i�erisindeki methodlara yetenekleri kazand�r�l�r.

         public class AppUserService : IAppUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFollowService _followService;

        public AppUserService(IUnitOfWork unitOfWork,
                              IMapper mapper,
                              UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              IFollowService followService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._followService = followService;
        }

        public async Task DeleteUser(params object[] parameters) => await _unitOfWork.ExecuteSqlRaw("spDeleteUsers {0}", parameters);

        public async Task EditUser(EditProfileDTO editProfileDTO)
        {
            AppUser user = await _unitOfWork.AppUserRepository.GetById(editProfileDTO.Id);

            if (user != null)
            {
                if (editProfileDTO.Image != null)
                {
                    using var image = Image.Load(editProfileDTO.Image.OpenReadStream());
                    image.Mutate(x => x.Resize(256, 256));
                    image.Save("wwwroot/images/users/" + Guid.NewGuid().ToString() + ".jpg");
                    user.ImagePath = ("/images/users/" + Guid.NewGuid().ToString() + ".jpg");
                }

                if (editProfileDTO.Password != null)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, editProfileDTO.Password);
                    await _userManager.UpdateAsync(user);
                }

                if (editProfileDTO.UserName != null)
                {
                    var isUserNameExsist = _userManager.FindByNameAsync(editProfileDTO.UserName);
                    if (isUserNameExsist == null)
                    {
                        await _userManager.SetUserNameAsync(user, editProfileDTO.UserName);
                        //user.UserName = editProfileDTO.UserName;
                        //await _userManager.UpdateAsync(user);
                    }
                }

                if (editProfileDTO.Email != null)
                {
                    var isEmailExsist = _userManager.FindByEmailAsync(editProfileDTO.Email);
                    if (isEmailExsist == null) await _userManager.SetEmailAsync(user, editProfileDTO.Email);
                }

                if (editProfileDTO.Name != null)
                {
                    user.Name = editProfileDTO.Name;
                }

                _unitOfWork.AppUserRepository.Update(user);
                await _unitOfWork.Commit();
            }
        }

        public async Task<EditProfileDTO> GetById(int id)
        {
            AppUser user = await _unitOfWork.AppUserRepository.GetById(id);

            return _mapper.Map<EditProfileDTO>(user);
        }

        public async Task<ProfileSummaryDTO> GetByUserName(string userName)
        {
            var user = await _unitOfWork.AppUserRepository.GetFilteredFirstOrDefault(
                selector: x => new ProfileSummaryDTO
                {
                    UserName = x.UserName,
                    Name = x.Name,
                    ImagePath = x.ImagePath,
                    TweetCount = x.Tweets.Count,
                    FollowerCount = x.Followers.Count,
                    FollowingCount = x.Followings.Count,
                },
                expression: x => x.UserName == userName);

            return user;
        }

        public async Task<int> GetUserIdFromName(string name)
        {
            var user = await _unitOfWork.AppUserRepository.GetFilteredFirstOrDefault(
                selector: x => x.Id,
                expression: x => x.Name == name);

            return user;
        }

        public async Task<SignInResult> LogIn(LoginDTO loginDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDTO.UserName, loginDTO.Password, false, false);

            return result;
        }

        public async Task LogOut() => await _signInManager.SignOutAsync();

        public async Task<IdentityResult> Register(RegisterDTO registerDTO)
        {
            var user = _mapper.Map<AppUser>(registerDTO);

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded) await _signInManager.SignInAsync(user, isPersistent: false);
            return result;
        }


        public async Task<List<FollowListVM>> UsersFollowers(int id, int pageIndex)
        {
            List<int> followers = await _followService.Followers(id);

            var followersList = await _unitOfWork.AppUserRepository.GetFilteredList(
                selector: x => new FollowListVM
                {
                    Id = x.Id,
                    ImagePath = x.ImagePath,
                    UserName = x.UserName,
                    Name = x.Name
                },
                expression: x => followers.Contains(x.Id),
                include: x => x.Include(x => x.Followers),
                pageIndex: pageIndex);

            return followersList;
        }

        public async Task<List<FollowListVM>> UsersFollowings(int id, int pageIndex)
        {
            List<int> followings = await _followService.Followings(id);

            var followingsList = await _unitOfWork.AppUserRepository.GetFilteredList(
                selector: x => new FollowListVM
                {
                    Id = x.Id,
                    ImagePath = x.ImagePath,
                    UserName = x.UserName,
                    Name = x.Name
                },
                expression: x => followings.Contains(x.Id),
                include: x => x.Include(x => x.Followings),
                pageIndex: pageIndex);

            return followingsList;
        }
    }

    4.7. Extensions klas�r�n�n i�ine  ClaimsPrincipalExtensions ad�nda bir s�n�f a��l�r ve kullan�c�dan al�nan bilgilerin belirli �artlara uymas� sa�lan�r.

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserEmail(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.Email);

        public static int GetUserId(this ClaimsPrincipal principal) => Convert.ToInt32(principal.FindFirstValue(ClaimTypes.NameIdentifier));

        public static string GetUserName(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.Name);

        public static bool IsCurrentUser(this ClaimsPrincipal principal, string id)
        {
            var currentUserId = GetUserId(principal).ToString();

            return string.Equals(currentUserId, id);
        }
    }

    4.8. Validations klas�r� a��l�r.LoginValidation,RegisterValidation,TweetValidation s�n�flar� a��larak kurallar yaz�l�r.
   
        public class LoginValidation : AbstractValidator<LoginDTO>  //Validation olarak kullanabilmek i�in AbstractValidator s�n�f�n� kal�t�m olarak verdik.
    {
        public LoginValidation()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Enter a Username");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Enter a Password");
        }
    }

    public class RegisterValidation: AbstractValidator<RegisterDTO>
    {
        public RegisterValidation()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Enter e mail address").EmailAddress().WithMessage("Please type into a valid email address");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Pleasee enter a password");
            RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x=> x.Password).WithMessage("Password do not macth");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name can't be empty.").MinimumLength(3).MaximumLength(20).WithMessage("Minimum 3, maximum 20 character");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("User name can't be empty").MinimumLength(3).MaximumLength(20).WithMessage("Minimum 3, maxsimum 20 character");
        }
    }

    public class TweetValidation : AbstractValidator<AddTweetDTO>
    {
        public TweetValidation() => RuleFor(x => x.Text).NotEmpty().WithMessage("Can not be empty").MaximumLength(256).WithMessage("Must be less then 256 character");

    }

    4.9. IoC container klas�r�m�zdeki DependencyInjection s�n�f�na yazd���m�z validationlar� Resolve ediyoruz.
       
       //Validation Resolver
            services.AddTransient<IValidator<RegisterDTO>, RegisterValidation>();
            services.AddTransient<IValidator<LoginDTO>, LoginValidation>();
            services.AddTransient<IValidator<AddTweetDTO>, TweetValidation>();


    5. DDD yap�s�na uygun  d�rd�n� ve son olarak SocialMediaTwitterProject.Presentation ad�nda Asp .Net Core Web Application projesi a��l�r.

    5.1. A�a��daki katmanlar Presentation katman�na referance olarak verilir.

		5.1.1. SocialMediaProject.Infrastructure

		5.1.2. SocialMediaProject.Application

	5.2. A�a��daki paketler y�klenir.

		5.2.1. Microsoft.EntityFrameworkCore.SqlServer

	5.3. Startup.cs => ConfigureService() methoduna ba��ml�l��a neden olacak s�n�flar register edilir.

		5.3.1. Application katman�ndaki Register_ResolveService.cs eklenir.

		5.3.2. ApplicationDbContext.cs s�n�f� register edilir.

	5.4. appsettings.json dosyas�na "ConnectionString" yaz�l�r.

    5.5. Third Party (Ioc) Containerlardan biri olan AutofacContainer isimli bir klas�r a��l�r.Module s�n�f� kal�t�m verilerek �zellikler kullan�ma haz�r hale getirilir.Son olarak Autofac(6.1.0) paketi y�klenir.

     public class AutofacContainer:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppUserService>().As<IAppUserService>().InstancePerLifetimeScope();
            builder.RegisterType<FollowService>().As<IFollowService>().InstancePerLifetimeScope();
            builder.RegisterType<LikeService>().As<ILikeService>().InstancePerLifetimeScope();
            builder.RegisterType<TweetService>().As<ITweetService>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            
           
            builder.RegisterType<LoginValidation>().As<IValidator<LoginDTO>>().InstancePerLifetimeScope();
            builder.RegisterType<RegisterValidation>().As<IValidator<RegisterDTO>>().InstancePerLifetimeScope();
            builder.RegisterType<TweetValidation>().As<IValidator<AddTweetDTO>>().InstancePerLifetimeScope();
        }
    }

    5.6. Olu�turdu�umuz bu container'� kullanabilmek i�in program.cs de �a��r�yoruz.

     public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new AutofacContainer());
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

	5.7. Migration i�lemi yap�l�r.�ncellikle package manager console a��larak Add-Migration InitialCreate ve ard�ndan Update-Database yaz�l�p migration i�lemi tamamlan�r.

    5.8. AccountController'da Register,Login,LogOut,EditProfile i�lemleri i�in asenkron olarak metodlar�n� yazd�k daha sonra actionlar�n Viewlar�n� ekledik.

      public class AccountController : Controller
    {
        private readonly IAppUserService _userService;

        public AccountController(IAppUserService appUserService) => _userService = appUserService;

        #region Registration
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Register(registerDTO);

                if (result.Succeeded) return RedirectToAction("Index", "Home");

                foreach (var item in result.Errors) ModelState.AddModelError(string.Empty, item.Description);                        
            }

            return View(registerDTO);
        }
        #endregion

        #region Login
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction(nameof(HomeController.Index), "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LogIn(model);

                if (result.Succeeded) return RedirectToLocal(returnUrl);

                ModelState.AddModelError(String.Empty, "Invalid login attempt..!");
            }
            return View();
        }
        
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            else return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion

        #region Logout
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _userService.LogOut();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion

        #region EditProfile
        public async Task<IActionResult> EditProfile(string userName)
        {
            if (userName == User.Identity.Name)
            {
                var user = await _userService.GetById(User.GetUserId());

                if (user == null) return NotFound();

                return View(user);
            }
            else return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileDTO model, IFormFile file)
        {
            //model.Image = file;
            await _userService.EditUser(model);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion
     }

     5.9. Login i�lemi i�in view k�sm�n� ekledik.

     @model EditProfileDTO
@{
    ViewData["Title"] = "EditProfile";
}

<div class="row">
    <div class="col-sm-10">
        <div class="card">
            <div class="card-header">
                <h4 class="card-title">Edit Profile</h4>
            </div>
            <div class="card-body">
                <form method="post" asp-controller="Account" asp-action="EditProfile" enctype="multipart/form-data">
                    <input hidden asp-for="Id" />
                    <div asp-validation-summary="ModelOnly" class="text-danger"></div>
                    <div class="form-group">
                        <label asp-for="Email"></label>
                        <input class="form-control" asp-for="Email" placeholder="Email" />
                        <span asp-validation-for="Email" class="text-danger"></span>
                    </div>
                    <div class="form-group">
                        <label asp-for="UserName"></label>
                        <input class="form-control" asp-for="UserName" placeholder="User Name" />
                        <span asp-validation-for="UserName" class="text-danger"></span>
                    </div>
                    <div class="form-group">
                        <label asp-for="Name"></label>
                        <input class="form-control" asp-for="Name" placeholder="Name" />
                        <span asp-validation-for="Name" class="text-danger"></span>
                    </div>
                    <div class="form-group">
                        <label asp-for="Password"></label>
                        <input class="form-control" asp-for="Password" placeholder="Password" />
                        <span asp-validation-for="Password" class="text-danger"></span>
                    </div>

                    <div class="form-group">
                        <label>Choose File</label>
                        <input class="form-control" asp-for="Image" />
                    </div>

                    <div class="form-group float-right">
                        <button type="submit" class="btn btn-lg btn-primary">Edit Profile</button>
                    </div>
                </form>
            </div>
            <div class="card-footer">
                <a asp-controller="Account" asp-action="LogIn" class="btn btn-block btn-dark">Log In</a>
            </div>
        </div>
    </div>
</div>


     5.10. Views klas�r�n�n i�ine ViewComponents isminde klas�r a��l�r ve ProfileSummary ad�nda bir s�n�f a��l�p ViewComponent kal�t�m olarak verilir ve GetByUserName methodu ile gelen string tipindeki userName nesnesinin �zelliklerini kullanabiliyoruz.
        
     public class ProfileSummary:ViewComponent
    {
        private readonly IAppUserService _appUserService;

        public ProfileSummary(IAppUserService appUserService) => this._appUserService = appUserService;

        public async Task<IViewComponentResult> InvokeAsync(string userName) => View(await _appUserService.GetByUserName(userName));
    }

    5.11. Models klas�r�ne ViewComponents ad�nda klas�r a��l�r ve AddTweet,FollowUser ve ProfileSummary ad�nda s�n�flar a��l�r ve ViewComponent s�n�f� kal�t�m verilerek �zellikler kullan�ma haz�r hale getirilir.

    public class AddTweet : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            int userId = Convert.ToInt32(claim.Value);
            var tweet = new AddTweetDTO();
            tweet.AppUserId = userId;
            return View(tweet);
        }
    }

     public class FollowUser : ViewComponent
    {
        private readonly IAppUserService _userService;
        private readonly IFollowService _followService;

        public FollowUser(IAppUserService appUserService,
                          IFollowService followService)
        {
            this._userService = appUserService;
            this._followService = followService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userName)
        {
            int userId = await _userService.GetUserIdFromName(userName);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            int followerId = Convert.ToInt32(claim.Value);

            var followModel = new FollowDTO { FollowerId = followerId, FollowingId = userId };
            followModel.isExsist = await _followService.IsFollowing(followModel);

            return View(followModel);
        }
    }

     public class ProfileSummary : ViewComponent
    {
        private readonly IAppUserService _appUserService;

        public ProfileSummary(IAppUserService appUserService) => this._appUserService = appUserService;

        public async Task<IViewComponentResult> InvokeAsync(string userName) => View(await _appUserService.GetByUserName(userName));
    }

    5.12. Daha sonra Shared->Components alt�nda ViewComponentlerin Default ad�nda Viewlar� eklenir.
  
@model AddTweetDTO

<form id="formAddTweet" enctype="multipart/form-data">
    <div asp-validation-summary="All" class="text-danger"></div>
    <input hidden asp-for="AppUserId" />
    <div id="tweetValidation" role="alert"></div>
    <div class="form-group">
        <textarea name="Text" id="Text" placeholder="Share what you've been up to" class="form-control"></textarea>
    </div>
    <div class="form-group">
        <label>Choose Image</label>
        <input type="file" id="Image" name="Image" style="display:none" data-toggle="tooltip" accept=".jpg, .jpeg, .png" />
    </div>
    <div class="form-group">
        <button type="button" id="btnSendTweet" class="btn btn-outline-info" value="Post"></button>
    </div>
</form>

@model FollowDTO

<input hidden asp-for="FollowerId" />
<input hidden asp-for="FollowingId" />

@{
    if (Model.isExsist)
    {
        <button id="UnFollow" onclick="Follow(@Model.isExsist.ToString().ToLower())" class="btn btn-info btn-sm">
            <i class="fa fa-minus"></i>Unfollow
        </button>
    }
    else
    {
        <button id="Follow" onclick="Follow(@Model.isExsist.ToString().ToLower())" class="btn btn-info btn-sm">
            <i class="fa fa-plus"></i>Follow
        </button>
    }
}

@inject SignInManager<AppUser> SignInManager
@using Microsoft.AspNetCore.Identity
@model ProfileSummaryDTO

<h4>Profile Summary</h4>
<div class="card">
    <div class="card-header">
        <img src="@Model.ImagePath" class="card-img-top" width="260" height="260" />
    </div>
    <div class="card-body">
        <h3 class="card-title">@Model.Name</h3>
        <h3 class="card-title">@Model.UserName</h3>

        @if (SignInManager.IsSignedIn(User) && User.Identity.Name != Model.UserName)
        {
            @await Component.InvokeAsync("FollowUser", new { UserName = @Model.UserName })
        }
        else if (SignInManager.IsSignedIn(User) && User.Identity.Name == Model.UserName)
        {
            <a asp-controller="Account" asp-action="EditProfile" asp-route-username="@User.Identity.Name">Settings</a>
        }

        <ul class="list-group">
            <li class="list-group-item">@Model.TweetCount - Tweets</li>
            <li class="list-group-item" id="FollowersCount">
                @Model.FollowerCount - Followers
                <a asp-controller="Profile" asp-action="Followers" asp-route-username="@Model.UserName">Followers</a>
            </li>
            <li class="list-group-item" id="FollowingsCount">
                @Model.FollowingCount - Followings
                <a asp-controller="Profile" asp-action="Followings" asp-route-username="@Model.UserName">Followings</a>
            </li>
        </ul>
    </div>
</div>
<script>
    var FollowersCount = @Model.FollowerCount;
    var FollowingsCount = @Model.FollowingCount;
</script>


    5.13. Shared klas�r�ne partial Viewlar eklenir.

    
@using Microsoft.AspNetCore.Identity
@inject SignInManager<AppUser> SignInManager


<nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
    <div class="container">
        <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">YMS5177_GraduationalProject.Presentation</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="mx-auto">
            <form asp-controller="Search" asp-action="Index" method="get">
                <input class="form-control" type="search" name="userName" placeholder="Search" onkeypress="keypress(event)" aria-label="Search" />
                @*<button class="btn btn-outline-success" type="submit" onclick="keypress(event)">Search</button>*@
            </form>
        </div>
        <div class="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse">
            <ul class="navbar-nav ml-2">
                @if (SignInManager.IsSignedIn(User))
                {
                    <li class="nav-item">
                        <form method="post" asp-controller="Account" asp-action="LogOut">
                            <button type="submit" class="btn btn-primary">Log Out</button>
                        </form>
                    </li>
                }
                else
                {
                    <li class="nav-item">
                        <a class="btn btn-primary" asp-area="" asp-controller="Account" asp-action="Register">Sign Up</a>
                    </li>
                    <li class="nav-item">
                        <a class="btn btn-primary" asp-area="" asp-controller="Account" asp-action="Login">Sign In</a>
                    </li>
                }
            </ul>
        </div>
    </div>
</nav>



@using Microsoft.AspNetCore.Identity
@inject SignInManager<AppUser> SignInManager


<ul class="list-group">
    <li class="list-group-item">
        <a asp-controller="Home" asp-action="Index">Home</a>
    </li>
    @if (SignInManager.IsSignedIn(User))
    {
        <li class="list-group-item">
            <a asp-controller="Profile" asp-action="Details" asp-route-username="@User.Identity.Name">Profile</a>
        </li>
        <li class="list-group-item">
            <a asp-controller="Account" asp-action="EditProfile" asp-route-username="@User.Identity.Name">Settings</a>
        </li>
    }
</ul>





    5.14. Controllers klas�r�ne ProfileController ad�nda bir Controller a��l�r ve ki�inin profili hakk�nda actionlar yaz�l�r.

    public class ProfileController : Controller
    {
        private readonly IAppUserService _appUserService;

        public ProfileController(IAppUserService appUserService) => this._appUserService = appUserService;

        public IActionResult Index() => View();

        public IActionResult Details(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }

        public IActionResult Followings(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Followings(string userName, int pageIndex)
        {
            var findUser = await _appUserService.GetUserIdFromName(userName);

            if (findUser > 0)
            {
                var followings = await _appUserService.UsersFollowings(findUser, pageIndex);

                return Json(followings, new JsonSerializerSettings());
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Followers(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Followers(string userName, int pageIndex)
        {
            var findUser = await _appUserService.GetUserIdFromName(userName);

            if (findUser > 0)
            {
                var followers = await _appUserService.UsersFollowers(findUser, pageIndex);

                return Json(followers, new JsonSerializerSettings());
            }
            else
            {
                return NotFound();
            }
        }
    }

    5.15. Yazd���m�z bu actionlar�n viewlar�n� ekliyoruz.

    @{
    ViewData["Title"] = "Details";
}

@await Component.InvokeAsync("ProfileSummary", new { UserName = @ViewBag.userName })

    5.16. Followers methodunun View'� yaz�l�r ve get-userlist isminde js dosyas� a��larak script kod yaz�l�r.

    
@{
    ViewData["Title"] = "Followers";
}

<div class="card">
    <div class="card-header">Followers</div>
    <div class="card-body">
        <ul class="list-group-item" id="UserResult">
        </ul>
    </div>
</div>

@section Scripts {
    <script>
        var userName = '@ViewBag.userName';
        var controllerName = '@ViewContext.RouteData.Values["Controller"].ToString()';
        var actionName = '@ViewContext.RouteData.Values["Action"].ToString()';
    </script>
    <script src="~/js/get-userlist.js"></script>
}

    import { error } from "jquery";

$(document).ready(function () {
    var pageIndex = 1;
    loadUserResults(pageIndex, userName, controllerName, actionName);

    $(window).scroll(function () {
        if ($(window).scrollTop() + $(window).height() > $(document).height() - 50) {
            pageIndex = pageIndex + 1;
            loadUserResults(pageIndex, userName, controllerName, actionName)
        }
    });
});


function loadUserResults(pageIndex, userName, controllerName, actionName) {
    $.ajax({
        url: "/" + controllerName + "/" + actionName,
        type: "POST",
        async: true,
        dataType: "json",
        data: { userName: userName, pageIndex = pageIndex },
        success: function (result) {
            console.log(result);
            var html = "";
            if (result) {
                $.each(result, function (key, item) {
                    html += '<li class="list-group-item"><img src="' + item.ImagePath + '" alt="" width="25" height="25"><a href="/profile/' + item.UserName + '">' + item.UserName + '</li>'
                });
                if (pageIndex == 1) {
                    $('#UserResult').html(html);
                }
                else {
                    $('#UserResult').append(html);
                }
            }
        },
        error: function (errorMessage) {
            $('#UserResult').html('<li><p class="text-center">There were not no result found</p></li>')
        }
    });
}

/*
 <li class="list-group-item">
    <a href="/profile/beast"> Beast
 </li>
 */

    5.17. site.js a��larak Follow i�lemi i�in fonksiyon yaz�l�r.

    import { error } from "jquery";


function Follow(isExist) {
    var model = {
        FollowerId: parseInt($("#FollowerId").val()),
        FollowingId: parseInt($("#FollowingId").val()),
        isExsist: isExist
    };

    $.ajax({
        data: { FollowerId: model.FollowerId, FollowingId: model.FollowingId, isExist: model.isExsist },
        type: "POST",
        url: "/Follow/Follow/",
        dataType: "JSON",
        success: function (result) {
            if (result == "Success") {
                if (!isExist) {
                    $("#Follow").replaceWith('<button onclick="Follow(true)" id="UnFollow" class="btn btn-info btn-sm">Unfollow</button>');
                    FollowersCount = FollowersCount + 1;
                    $("#FollowersCount").replaceWith('<li id="FollowersCount"><strong>' + FollowersCount + '</strong> - Followers</li>')
                }
                else {
                    $("#UnFollow").replaceWith('<button onclick="Follow(false)" id="Follow" class="btn btn-info btn-sm">Follow</button>');
                    FollowersCount = FollowersCount - 1;
                    $("#FollowersCount").replaceWith('<li id="FollowersCount"><strong>' + FollowersCount + '</strong> - Followers</li>')
                }
            }
        }
    });
}

$(document).ready(function () {

    $("#btnSendTweet").click(function (e) {

        var formData = new FormData();

        formData.append("AppUserId", JSON.stringify(parseInt($("#AppUserId").val())));
        formData.append("Text", $("#Text").val());
        formData.append("Image", $("#Image").val());

        $.ajax({
            data: formData,
            type: "POST",
            url: "/Tweet/AddTweet",
            success: function (result) {
                if (result == "Success") {
                    document.getElementById("Text").value = "";
                    $("#tweetValidation").addClass("alert alert-success").text("Send Successfully..!");
                    $("#tweetValidation").alert();
                    $("#tweetValidation").fadeOut(2000, 2000).slideDown(800, function () { });
                }
                else {
                    $("#tweetValidation").addClass("alert alert-danger").text("Error Occured..!");
                    $("#tweetValidation").alert();
                    $("#tweetValidation").fadeOut(2000, 2000).slideDown(800, function () { });
                }
            },
            error: function (result) {
                $("#tweetValidation").addClass("alert alert-success").text(result.responseText);
                $("#tweetValidation").alert();
                $("#tweetValidation").fadeOut(2000, 2000).slideDown(800, function () { });
            }
        });

    });

});

    