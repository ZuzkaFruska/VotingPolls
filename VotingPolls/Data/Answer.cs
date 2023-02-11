using System.ComponentModel.DataAnnotations.Schema;

namespace VotingPolls.Data
{
    public class Answer : BaseEntity
    {
        public string Text { get; set; }
        //[ForeignKey("VotingPollId")]
        //public VotingPoll VotingPoll { get; set; }
        public int? VotingPollId { get; set; }
        public List<Vote>? Votes { get; set; }
    }
}
