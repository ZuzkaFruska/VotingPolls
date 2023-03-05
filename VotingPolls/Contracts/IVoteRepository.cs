using VotingPolls.Data;

namespace VotingPolls.Contracts
{
    public interface IVoteRepository : IGenericRepository<Vote>
    {
        Task<List<Vote>> GetUserPollVotesAsync(string userId, int votingPollId);
    }
}
