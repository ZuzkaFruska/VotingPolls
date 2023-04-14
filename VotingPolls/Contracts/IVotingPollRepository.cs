using Microsoft.AspNetCore.Mvc;
using VotingPolls.Data;
using VotingPolls.Models;

namespace VotingPolls.Contracts
{
    public interface IVotingPollRepository : IGenericRepository<VotingPoll>
    {
        Task<VotingPollCreateVM> AddAnswerWhileCreateOrEditAsync(VotingPollCreateVM votingPollCreateVM);
        Task AddAnswerWhileVotingAsync(int votingPollId, string newAnswerValue);
        Task ApplyChangesAndSave(VotingPollEditVM votingPollEditVM);
        Task<VotingPoll> GetPollWithAnswersAndVotesAsync(int votingPollId);
        Task<List<VotingPoll>> GetUserPollsAsync(string userId);
        Task<VotingPollCreateVM> GetPollTemplateAsync();
        Task<ResultsVM> GetVotingResults(int votingPollId);
        Task<VotingPollCreateVM> RemoveAnswerWhileCreateOrEditAsync(VotingPollCreateVM votingPollCreateVM, int answerNo);
        

    }
}
