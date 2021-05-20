using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBContext.Models
{

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        { }
        public virtual Customer Customer { get; set; }
    }
    public partial class OpenSismDBContext : IdentityDbContext<ApplicationUser>
    {
        public static string _culture = "en";

        public OpenSismDBContext(string culture)
        {
            _culture = culture;
        }
        public OpenSismDBContext(DbContextOptions<OpenSismDBContext> options)
            : base(options)
        {

        }
        public OpenSismDBContext(DbContextOptions<OpenSismDBContext> options, string culture)
           : base(options)
        {
            _culture = culture;
        }



        public virtual DbSet<AppTask> AppTasks { get; set; }
        public virtual DbSet<AppTaskGroup> AppTaskGroups { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Condition> Conditions { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<ContactUs> ContactsUs { get; set; }

        public virtual DbSet<Mail> Mails { get; set; }
        public virtual DbSet<Content> Contents { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerAnswer> CustomerAnswers { get; set; }
        public virtual DbSet<CustomerMessage> CustomerMessages { get; set; }
        public virtual DbSet<CustomerPrediction> CustomerPredictions { get; set; }
        public virtual DbSet<CustomerPrize> CustomerPrizes { get; set; }
        public virtual DbSet<CustomerTask> CustomerTasks { get; set; }
        public virtual DbSet<DailyBonus> DailyBonuses { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<LuckyWheel> LuckyWheels { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MessageGroup> MessageGroups { get; set; }
        public virtual DbSet<MobileAppVersion> MobileAppVersions { get; set; }
        public virtual DbSet<Prize> Prizes { get; set; }
        public virtual DbSet<PrizeStatus> PrizeStatuses { get; set; }
        public virtual DbSet<PrizeType> PrizeTypes { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionOption> QuestionOptions { get; set; }
        public virtual DbSet<SportMatch> SportMatches { get; set; }
        public virtual DbSet<TaskType> TaskTypes { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppTask>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.DisplayName).IsRequired();

                entity.Property(e => e.DisplayNameAr).IsRequired();

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.HasOne(d => d.TaskType)
                    .WithMany(p => p.AppTasks)
                    .HasForeignKey(d => d.TaskTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppTask_TaskType");
            });

            modelBuilder.Entity<AppTaskGroup>(entity =>
            {
                entity.HasOne(d => d.AppTask)
                    .WithMany(p => p.AppTaskGroups)
                    .HasForeignKey(d => d.AppTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppTaskGroup_AppTask");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AppTaskGroups)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppTaskGroup_Group");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            });

            modelBuilder.Entity<Condition>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.DisplayName).IsRequired();

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Value).IsRequired();

            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.DisplayName).IsRequired();

                entity.Property(e => e.DisplayNameAr).IsRequired();

                entity.Property(e => e.Icon).IsRequired();

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Value).IsRequired();

                entity.Property(e => e.ValueAr).IsRequired();
            });

            modelBuilder.Entity<ContactUs>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Subject).IsRequired();

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ContacstUs)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactUs_Customer");
            });

            modelBuilder.Entity<Mail>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Subject).IsRequired();

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Mails)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Mail_Customer");
            });

            modelBuilder.Entity<Content>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasFilter("([UserId] IS NOT NULL)");

                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Customer_City");
            });

            modelBuilder.Entity<CustomerPrize>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerPrizes)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerPrize_Customer");

                entity.HasOne(d => d.Prize)
                    .WithMany(p => p.CustomerPrizes)
                    .HasForeignKey(d => d.PrizeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerPrize_Prize");
            });

            modelBuilder.Entity<CustomerTask>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.HasOne(d => d.AppTask)
                    .WithMany(p => p.CustomerTasks)
                    .HasForeignKey(d => d.AppTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerTask_AppTask");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerTasks)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerTask_Customer");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.DisplayName).IsRequired();

                entity.Property(e => e.DisplayNameAr).IsRequired();

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<LuckyWheel>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.HasOne(d => d.Group)
                    .WithMany(p =>  p.LuckyWheels)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LuckyWheel_Group");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            });

            modelBuilder.Entity<MessageGroup>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.MessageGroups)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_MessageGroup_Group");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.MessageGroups)
                    .HasForeignKey(d => d.MessageId)
                    .HasConstraintName("FK_MessageGroup_Message");
            });

            modelBuilder.Entity<MobileAppVersion>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Qrcode).HasColumnName("QRCode");
            });

            modelBuilder.Entity<Prize>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.DisplayName).IsRequired();

                entity.Property(e => e.DisplayNameAr).IsRequired();

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.PrizeType)
                    .WithMany(p => p.Prizes)
                    .HasForeignKey(d => d.PrizeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prize_PrizeType");
            });

            modelBuilder.Entity<PrizeType>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.DisplayName).IsRequired();

                entity.Property(e => e.DisplayNameAr).IsRequired();

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<TaskType>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            });

            modelBuilder.Entity<SportMatch>(entity =>
            {
                entity.HasIndex(e => e.AppTaskId)
                    .IsUnique()
                    .HasFilter("([AppTaskId] IS NOT NULL)");

                entity.Property(e => e.Created).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.Modified).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            });

            modelBuilder.Entity<AppTask>()
            .HasOne(a => a.SportMatch)
            .WithOne(b => b.AppTask)
            .HasForeignKey<SportMatch>(b => b.AppTaskId);

            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


        public override int SaveChanges()
        {
            Audit();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            Audit();
            return await base.SaveChangesAsync();
        }

        private void Audit()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).Created = DateTime.Now;
                    ((BaseEntity)entry.Entity).IsDeleted = false;
                }
            ((BaseEntity)entry.Entity).Modified = DateTime.Now;
            }
        }

    }


}
