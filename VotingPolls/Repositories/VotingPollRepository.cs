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

        public async Task<VotingPoll?> GetWithAnswersAndVotesAsync(int? id)
        {
            if (id == null)
                return null;
            var votingPoll = new VotingPoll();
            
                votingPoll = await _context.VotingPolls.FindAsync(id);
                votingPoll.Answers = await _context.Answers.Where(q => q.VotingPollId == id).ToListAsync();
                votingPoll.Votes = await _context.Votes.Where(q => q.VotingPollId == id).ToListAsync();

                foreach (var answer in votingPoll.Answers)
                {
                    answer.Votes = await _context.Votes.Where(q => q.AnswerId == answer.Id).ToListAsync();
                    answer.Author = await _userManager.FindByIdAsync(answer.AuthorId);

                    foreach (var vote in answer.Votes)
                    {
                        vote.Voter = await _userManager.FindByIdAsync(vote.VoterId);
                    }
                }
            
            
            return votingPoll;
        }

        public async Task<List<VotingPoll>> GetUserPolls(string userId)
        {
            var userPolls = await _context.VotingPolls.Where(q => q.OwnerId== userId).ToListAsync();
            return userPolls;
        }
    }
}
