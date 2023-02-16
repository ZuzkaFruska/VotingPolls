using Microsoft.AspNetCore.Mvc;
using VotingPolls.Contracts;
using VotingPolls.Data;
using VotingPolls.Models;
using VotingPolls.Extensions;

namespace VotingPolls.Repositories
{
    public class VotingPollRepository : GenericRepository<VotingPoll>, IVotingPollRepository
    {
        public VotingPollRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
