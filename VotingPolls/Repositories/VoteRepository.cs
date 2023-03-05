using Microsoft.EntityFrameworkCore;
using VotingPolls.Contracts;
using VotingPolls.Data;

namespace VotingPolls.Repositories
{
    public class VoteRepository : GenericRepository<Vote>, IVoteRepository
    {
        private readonly ApplicationDbContext _context;

        public VoteRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }


        public async Task<List<Vote>> GetUserPollVotesAsync(string userId, int votingPollId)
        {
            var userPollVotes = await _context.Votes.AsNoTracking().Where(v => v.VoterId == userId && v.VotingPollId == votingPollId).ToListAsync();
            return userPollVotes;
        }

    }
}
