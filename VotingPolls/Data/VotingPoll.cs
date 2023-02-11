using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingPolls.Data
{
    public class VotingPoll : BaseEntity
    {
        public string Name { get; set; }
        public string Question { get; set; }
        public bool MultipleChoice { get; set; }
        public bool AdditionalAnswers { get; set; }
        public List<Answer> Answers { get; set; }
        public List<Vote> Votes { get; set; }


        [ForeignKey("OwnerId")]
        public User User { get; set; }
        public string OwnerId { get; set; }
    }
}
