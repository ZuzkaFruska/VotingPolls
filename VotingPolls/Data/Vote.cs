using System.ComponentModel.DataAnnotations.Schema;

namespace VotingPolls.Data
{
    public class Vote : BaseEntity
    {
        public string UserId { get; set; }

        [ForeignKey("VotingPollId")]
        public int VotingPollId { get; set; }

        [ForeignKey("AnswerId")]
        public int AnswerId { get; set; }
    }
}
