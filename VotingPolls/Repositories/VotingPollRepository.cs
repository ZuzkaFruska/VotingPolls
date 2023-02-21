using Microsoft.AspNetCore.Mvc;
using VotingPolls.Contracts;
using VotingPolls.Data;
using VotingPolls.Models;
using VotingPolls.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace VotingPolls.Repositories
{
    public class VotingPollRepository : GenericRepository<VotingPoll>, IVotingPollRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public VotingPollRepository(ApplicationDbContext context,
                                    UserManager<User> userManager) : base(context)
        {
            this._context = context;
            this._userManager = userManager;
        }

        public override async Task<VotingPoll?> GetAsync(int? id)
        {
            if (id == null)
                return null;

            var votingPoll = await _context.VotingPolls.FindAsync(id);
            votingPoll.Answers = await _context.Answers.Where(q => q.VotingPollId == id).ToListAsync();
            votingPoll.Votes = await _context.Votes.Where(q => q.VotingPollId == id).ToListAsync();

            foreach (var answer in votingPoll.Answers)
            {
                answer.Votes = await _context.Votes.Where(q => q.AnswerId == answer.Id).ToListAsync();

                foreach (var vote in answer.Votes)
                {
                    vote.User = await _userManager.FindByIdAsync(vote.UserId);
                }
            }
            
            return votingPoll;
        }

        public async Task<List<VotingPoll>> GetUserPolls(string userId)
        {
            var userPolls = await _context.VotingPolls.Where(q => q.UserId== userId).ToListAsync();
            return userPolls;
        }
    }
}
