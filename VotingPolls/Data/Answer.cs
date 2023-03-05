using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingPolls.Data
{
    public class Answer : BaseEntity
    {
        [StringLength(500, MinimumLength = 1, ErrorMessage = "The answer must contain a minimum of 1 and a maximum of 500 characters.")]
        public string Text { get; set; }

        public int Number { get; set; }
        public List<Vote>? Votes { get; set; }


        
        public int VotingPollId { get; set; } // First the VotingPoll needs to be created before the Answer can have its Id
        public VotingPoll? VotingPoll { get; set; }


        
        public string AuthorId { get; set; }
        public User? Author { get; set; }
    }
}
