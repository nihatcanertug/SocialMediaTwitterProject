



1. SocialMediaTwitterProject adinda bir Blank Solution açilir.

2.1 DDD yapýsýna uygun  ilk olarak SocialMediaTwitterProject.Domain adýnda Class library (.NetCore)  Projesi açýlýr.

	2.2. Enums,Entities,Repositories ve UnitOfWork klasörleri açýlýr.

	2.3. Enums klasörünün içine Status sýnýfý eklenir.

		public enum Status { Active = 1, Modified = 2, Passive = 3 }

	2.4. Entities klasörünün içine Concrete ve Interface klasörleri açýlýr ve interface'e IBaseEntity interface'i eklenir.

	 public interface IBaseEntity
    {
        DateTime CreateDate { get; set; }
        DateTime? ModifiedDate { get; set; }
        DateTime? DeleteDate { get; set; }
        Status Status { get; set; }
    }

    2.5. Concrete klasörü açýlýr ve AppRole isminde bir sýnýf açýlarak kalýtým olarak IdentityRole sýnýfý verilir.Bunun için usinglere Microsoft.AspNetCore.Identity paketi yüklenir.Son olarak IBaseEntity kalýtým verilerek bazý propertylere Encapsulation iþlemi gerçekleþtirilir.

     public class AppRole:IdentityRole<int>,IBaseEntity
    {
        private DateTime _createDate = DateTime.Now;
        public DateTime CreateDate { get => _createDate; set => _createDate = value; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        private Status _status = Status.Active;
        public Status Status { get => _status; set => _status = value; }
    }
    2.6. Sýrasýyla AppUser,Follow,Like,Mention,Share,Tweet varlýk sýnýflarý Concrete klasörüne eklenir.

    2.7. Repositories klasörüne BaseRepo ve EntityTypeRepo adýnda 2 klasör açýlýr ve Base reponun içine  bütün CRUD operasyonlarýnda kullanacaðýmýz method'larý yazacaðýmýz IRepository interface'ini açýyoruz.

    public interface IRepository<T>where T : class,IBaseEntity //diðer interfacelere kalýtým vericeðimiz için T type geçiyoruz.
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
    2.8. EntityTypeRepo klasörü açmýþtýk DIP gereði varlýklarýn interfacelerini açýcaz ve T type olarak belirlediðimiz için Concrete'leri vereceðiz böylece Baðýmlýlýðý tersine döndürmüþ olacaðýz.

    public interface IAppUserRepository : IRepository<AppUser> { }

    2.9. UnitOfWork klasörü açýlýr ve içine IUnitOfWork interface'i açýlýr.Amacýmýz içerisine bu projede UnitOfWork pattern'ýna dahil etmek istediðimiz repository'leri yazýcaðýz.

     public interface IUnitOfWork: IAsyncDisposable
    {
        //Repository'lerde açtýðýmýz ve kullanmak istediðimiz interfaceleri ekliyoruz.
        IAppUserRepository AppUserRepository { get; }
        IFollowRepository FollowRepository { get; }
        ILikeRepository LikeRepository { get; }
        IMentionRepository MentionRepository { get; }
        IShareRepository ShareRepository { get; }
        ITweetRepository TweetRepository { get; }

        Task Commit(); //Baþarýlý bir iþlemin sonucunda çalýþtýrýlýr.Ýþlemin baþlamasýndan itibaren tüm deðiþikliklerin veri tabanýna uygulanmasýný temin eder.
        Task ExecuteSqlRaw(string sql, params object[] parameters); //Mevcut sql sorgularýmýzý doðrudan veri tabanýnda yürütmek için kullanýlan bir method.
    }

3. DDD yapýsýna uygun ikinci olarak SocialMediaTwitterProject.Infrastructure adýnda Class library (.NetCore)  Projesi açýlýr.
   
   3.1. Sýrasýyla Context,Mapping,Repositories ve UnitOfWork klasörleri açýlýr.

   3.2. Mapping klasörünün içine Abstract ve Concrete klasörleri açýlýr ve Abstract'a BaseMap sýnýfý eklenir.

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

    3.3. Concrete sýnýfýna varlýklarýn hepsinin map'ini class olarak ekledik ve BaseMap'den kalýtým verdik.

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

    3.4. Context klasörüne ApplicationDbContext sýnýfý açýlýr.

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

        3.5. Domain katmanýnda interfaceler içerisinde tanýmlanmýþ repository'ler burada concrete edilir.BaseRepo ve EntityTypeRepo adýnda 2 klasör açýlýr ve içlerine repository class'larý eklenir.BaseRepository diðer concrete sýnýflara kalýtým vericeði için T type olarak eklenir.

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
                                                                      bool disableTracing = true)//varlýk üzerindeki deðiþikliklere bakýp savechanges() yolluyoruz.
        {
            IQueryable<T> query = _table; //ilk olarak bana gelecek tabloyu IQueryable<T> ye atýyoruz.
            if (disableTracing) query = query.AsNoTracking();//Filtreleme yapýcaðýz,sadece get iþlemi olacaðý için tracking'i kapattýk.
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
            //AsNoTracking; Entity Framework tarafýndan uygulamalarýn performansýný optimize etmemize yardýmcý olmak için geliþtirilmiþ bir fonksiyondur. Ýþlevsel olarak veritabanýndan sorgu neticesinde elde edilen nesnelerin takip mekanizmasý ilgili fonksiyon tarafýndan kýrýlarak, sistem tarafýndan izlenmelerine son verilmesini saðlamakta ve böylece tüm verisel varlýklarýn ekstradan iþlenme yahut lüzumsuz depolanma süreçlerine maliyet ayrýlmamaktadýr.
            if (include != null) query = include(query);//sorguya dahil olacak tablolarýn eklemesi için (eager loading)
            if (expression != null) query = query.Where(expression); // blog projesinde kullandýðýmýz esnek fileteleme mekanizmasý
            if (orderby != null) return await orderby(query).Select(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(); //sýralama kriteri var ve ona göre gird oluþturulacaktýr.
            else return await query.Select(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();//bir sýralama kriteri yoktur
        }

        public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;

    }

    3.6. EntityTypeRepo klasörüne Concrete repository'ler eklenir ve interfaceleri Baserepository ile birlikte kalýtým olarak eklenir.

     public class AppUserRepository:BaseRepository<AppUser>,IAppUserRepository
    {
        public AppUserRepository(ApplicationDbContext context):base(context){}
    }

    3.7. UnitOfWork klasörü açýlýr. Domain katmanýnda arayüz olarak tanýmlanmýþ IUnitOfWork.cs sýnýfý burada concrete edilir.

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
                GC.SuppressFinalize(this); //Nesnemizin tamamýyla temizlenmesini saðlayacak. (https://stackoverflow.com/questions/151051/when-should-i-use-gc-suppressfinalize)
            }
        }

        private async Task DisposeAsync(bool disposing)
        {
            if (disposing) await _db.DisposeAsync();
        }
    }

    4.   DDD yapýsýna uygun  üçüncü olarak SocialMediaTwitterProject.Application adýnda Class Library (.Core) Projesi açýlýr.

        4.1. Models klasörü açýlýr.Bu klasörün içine Data Transfer Objelerimiz ve View Models saklayacaðýz.

        4.2. DTOs klasörü açýlýr ve içine DTO classlarý eklenir.

        public class AddMentionDTO //Business iþlerimiz için DTO'lar oluþturuyoruz.
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int AppUserId { get; set; }
        public int TweetId { get; set; }
        public DateTime CreateDate { get; set; }
    }

     public class EditProfileDTO
    {
        //Business Domain ihtiyaçlarýmýza göre hazýrladýðýmýz veri transfer objelerimiz ef öðrenmeye baþladýðýmýz ilk günden beri kullandýðýmýz attribute bazýnda þartlar içerebilirler. Eski projelerimizde örneðin CMS projesinde bir prototype hem entity hemde DTO gibi kullanýyorduk.
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
        public bool isExsist { get; set; }  //Bir daha like'layamadýðý için önceden like atýp atmadýðýný kontrol ediyoruz.
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

        4.3. Mapper klasörü açýlýr.Mapping sýnýfý açýlýr. Bu sýnýf Profile.cs sýnýfýndan miras alýr. Lakin bu sýnýftan yararlanamak için aþaðýdaki "AutoMapper" paketini yüklenmeniz gerekmektedir.AutoMapper & AutoMapper.Extensions.Microsoft.DependencyInjection paketleri yüklenilir.

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

        4.4 IoC klasörü açýlýr ve içine DependencyInjection sýnýfý eklenir.Projedeki baðýmlý sýnýflar burada register ve resolve edilir. Burada built-in container içerisinde bu conrainer'in bize verdiði yapýlar ile register ve resolve iþlemlerimiz gerçekleþtiricez.

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

            //"AddIdentity" sýnýfý için Microsoft.AspNetCore.Identity paketi indirilir.
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

        4.5. Services klasörü açýlýr.

        4.6. Services => Interfaces klasörü açýlýr. UI katmanýnda kullanýlmak üzere iþlerimizi karþýlayacak servis arayüzleri oluþturulur.

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

	    4.7. Services => Concrete klasörü açýlýr. Oluþturulan servisler içerisindeki methodlara yetenekleri kazandýrýlýr.

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

    4.7. Extensions klasörünün içine  ClaimsPrincipalExtensions adýnda bir sýnýf açýlýr ve kullanýcýdan alýnan bilgilerin belirli þartlara uymasý saðlanýr.

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

    4.8. Validations klasörü açýlýr.LoginValidation,RegisterValidation,TweetValidation sýnýflarý açýlarak kurallar yazýlýr.
   
        public class LoginValidation : AbstractValidator<LoginDTO>  //Validation olarak kullanabilmek için AbstractValidator sýnýfýný kalýtým olarak verdik.
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

    4.9. IoC container klasörümüzdeki DependencyInjection sýnýfýna yazdýðýmýz validationlarý Resolve ediyoruz.
       
       //Validation Resolver
            services.AddTransient<IValidator<RegisterDTO>, RegisterValidation>();
            services.AddTransient<IValidator<LoginDTO>, LoginValidation>();
            services.AddTransient<IValidator<AddTweetDTO>, TweetValidation>();


    5. DDD yapýsýna uygun  dördünü ve son olarak SocialMediaTwitterProject.Presentation adýnda Asp .Net Core Web Application projesi açýlýr.

    5.1. Aþaðýdaki katmanlar Presentation katmanýna referance olarak verilir.

		5.1.1. SocialMediaProject.Infrastructure

		5.1.2. SocialMediaProject.Application

	5.2. Aþaðýdaki paketler yüklenir.

		5.2.1. Microsoft.EntityFrameworkCore.SqlServer

	5.3. Startup.cs => ConfigureService() methoduna baðýmlýlýða neden olacak sýnýflar register edilir.

		5.3.1. Application katmanýndaki Register_ResolveService.cs eklenir.

		5.3.2. ApplicationDbContext.cs sýnýfý register edilir.

	5.4. appsettings.json dosyasýna "ConnectionString" yazýlýr.

    5.5. Third Party (Ioc) Containerlardan biri olan AutofacContainer isimli bir klasör açýlýr.Module sýnýfý kalýtým verilerek özellikler kullanýma hazýr hale getirilir.Son olarak Autofac(6.1.0) paketi yüklenir.

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

    5.6. Oluþturduðumuz bu container'ý kullanabilmek için program.cs de çaðýrýyoruz.

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

	5.7. Migration iþlemi yapýlýr.Öncellikle package manager console açýlarak Add-Migration InitialCreate ve ardýndan Update-Database yazýlýp migration iþlemi tamamlanýr.

    5.8. AccountController'da Register,Login,LogOut,EditProfile iþlemleri için asenkron olarak metodlarýný yazdýk daha sonra actionlarýn Viewlarýný ekledik.

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

     5.9. Login iþlemi için view kýsmýný ekledik.

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


     5.10. Views klasörünün içine ViewComponents isminde klasör açýlýr ve ProfileSummary adýnda bir sýnýf açýlýp ViewComponent kalýtým olarak verilir ve GetByUserName methodu ile gelen string tipindeki userName nesnesinin özelliklerini kullanabiliyoruz.
        
     public class ProfileSummary:ViewComponent
    {
        private readonly IAppUserService _appUserService;

        public ProfileSummary(IAppUserService appUserService) => this._appUserService = appUserService;

        public async Task<IViewComponentResult> InvokeAsync(string userName) => View(await _appUserService.GetByUserName(userName));
    }

    5.11. Models klasörüne ViewComponents adýnda klasör açýlýr ve AddTweet,FollowUser ve ProfileSummary adýnda sýnýflar açýlýr ve ViewComponent sýnýfý kalýtým verilerek özellikler kullanýma hazýr hale getirilir.

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

    5.12. Daha sonra Shared->Components altýnda ViewComponentlerin Default adýnda Viewlarý eklenir.
  
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


    5.13. Shared klasörüne partial Viewlar eklenir.

    
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





    5.14. Controllers klasörüne ProfileController adýnda bir Controller açýlýr ve kiþinin profili hakkýnda actionlar yazýlýr.

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

    5.15. Yazdýðýmýz bu actionlarýn viewlarýný ekliyoruz.

    @{
    ViewData["Title"] = "Details";
}

@await Component.InvokeAsync("ProfileSummary", new { UserName = @ViewBag.userName })

    5.16. Followers methodunun View'ý yazýlýr ve get-userlist isminde js dosyasý açýlarak script kod yazýlýr.

    
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

    5.17. site.js açýlarak Follow iþlemi için fonksiyon yazýlýr.

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

    
