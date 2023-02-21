using VotingPolls.Contracts;
using VotingPolls.Data;

namespace VotingPolls.Repositories
{
    public class VoteRepository : GenericRepository<Vote>, IVoteRepository
    {
        public VoteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
