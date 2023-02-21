using Microsoft.AspNetCore.Mvc;
using VotingPolls.Data;

namespace VotingPolls.Contracts
{
    public interface IVotingPollRepository : IGenericRepository<VotingPoll>
    {
        Task<List<VotingPoll>> GetUserPolls(string userId);
    }
}
