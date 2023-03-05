using VotingPolls.Data;

namespace VotingPolls.Models
{
    public class ResultsVM
    {
        public VotingPoll VotingPoll { get; set; }
        public string Referer { get; set; }
        public bool UserAlreadyVoted { get; set; }
    }
}
