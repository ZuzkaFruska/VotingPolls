using System.ComponentModel.DataAnnotations;
using VotingPolls.Data;

namespace VotingPolls.Models
{
    public class VoteVM
    {
        public VotingPoll VotingPoll { get; set; } // VotingPoll used only for GET, no need for POST
        public string UserId { get; set; }          // User that votes


        [Required(ErrorMessage = "At least one answer is required.")]
        public List<int>? UserAnswers { get; set; }
    }
}

