using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VotingPolls.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<VotingPoll> VotingPolls { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Vote> Votes { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Entity<Answer>()
                .HasOne(e => e.VotingPoll)
                .WithMany(e => e.Answers)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder
                .Entity<Vote>()
                .HasOne(e => e.Answer)
                .WithMany(e => e.Votes)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder
                .Entity<Vote>()
                .HasOne(e => e.VotingPoll)
                .WithMany(e => e.Votes)
                .OnDelete(DeleteBehavior.ClientCascade);
        }



    }
}