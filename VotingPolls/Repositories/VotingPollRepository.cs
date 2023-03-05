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

        public async Task<VotingPoll?> GetWithAnswersVotesAndUserAsync(int? id)
        {
            if (id == null)
                return null;
            var votingPoll = new VotingPoll();

            votingPoll = await _context.VotingPolls.FirstAsync(v => v.Id == id); //  FindAsync(id);
            votingPoll.Owner = await _userManager.FindByIdAsync(votingPoll.OwnerId);
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

        public override async Task DeleteAsync(int id)
        {
            _context.ChangeTracker.Clear();
            var answers = await _context.Answers.Where(q => q.VotingPollId == id).ToListAsync(); // entities must be loaded to avoid delete cascade exception
            var votes = await _context.Votes.Where(q => q.VotingPollId == id).ToListAsync();
            var votingPoll = await GetAsync(id);
            _context.VotingPolls.Remove(votingPoll);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }


        public async Task<List<VotingPoll>> GetUserPolls(string userId)
        {
            var userPolls = await _context.VotingPolls.AsNoTracking().Where(q => q.OwnerId== userId).ToListAsync();
            return userPolls;
        }
    }
}
