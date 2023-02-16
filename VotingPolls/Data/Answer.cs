using System.ComponentModel.DataAnnotations.Schema;

namespace VotingPolls.Data
{
    public class Answer : BaseEntity
    {
        public string Text { get; set; }
        public int Number { get; set; }
        public List<Vote>? Votes { get; set; }


        [ForeignKey("VotingPollId")]
        public int? VotingPollId { get; set; } // First the VotingPoll needs to be created before the Answer can have its Id


        [ForeignKey("UserId")]
        public string UserId { get; set; }
    }
}
