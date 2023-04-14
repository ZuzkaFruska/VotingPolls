using System.ComponentModel.DataAnnotations;
using VotingPolls.Data;

namespace VotingPolls.Models
{
    public class AnswerVM
    {
        public int Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }


        
        [Display(Name = "Answer text")]
        [Required(ErrorMessage = "Answer can't be empty!")]
        //[StringLength(500, MinimumLength = 1, ErrorMessage = "The answer must contain a minimum of 1 and a maximum of 500 characters.")]
        public string Text { get; set; }

        public int Number { get; set; }
        public List<VoteVM>? Votes { get; set; } 
        public int VotingPollId { get; set; } // First the VotingPoll needs to be created before the Answer can have its Id
        public string AuthorId { get; set; }

    }
}
