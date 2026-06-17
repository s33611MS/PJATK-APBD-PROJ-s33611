using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Entities.Auth;

namespace PJATK_APBD_PROJ_s33611.Data;

public class DatabaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<IndividualClient> IndividualClients { get; set; }
    public DbSet<CompanyClient> CompanyClients { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractPayment> ContractPayments { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Software> Software { get; set; }
    public DbSet<SoftwareCategory> SoftwareCategories { get; set; }
    public DbSet<SoftwareVersion> SoftwareVersions { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<Token> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureClients(modelBuilder);
        ConfigureContracts(modelBuilder);
        ConfigureSubscriptions(modelBuilder);
        ConfigureAuth(modelBuilder);
    }
    
    private static void ConfigureClients(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .UseTptMappingStrategy();

        modelBuilder.Entity<IndividualClient>()
            .HasIndex(x => x.Pesel)
            .IsUnique();

        modelBuilder.Entity<CompanyClient>()
            .HasIndex(x => x.Krs)
            .IsUnique();
    }

    private static void ConfigureContracts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contract>()
            .HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contract>()
            .HasOne(x => x.Software)
            .WithMany()
            .HasForeignKey(x => x.SoftwareId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ContractPayment>()
            .HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureSubscriptions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>()
            .HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SubscriptionPayment>()
            .HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureAuth(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Login).IsUnique();
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Token)
            .WithOne(t => t.User)
            .HasForeignKey<Token>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Token>()
            .HasIndex(x => x.RefreshToken).IsUnique();
    }
}