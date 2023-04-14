using VotingPolls.Data;

namespace VotingPolls.Models
{
    public class ResultsVM
    {
        public VotingPollVM VotingPollVM { get; set; }
        public bool UserAlreadyVoted { get; set; }
    }
}
