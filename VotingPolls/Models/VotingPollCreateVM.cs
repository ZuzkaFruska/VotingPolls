using Microsoft.Build.Framework;
using VotingPolls.Data;

namespace VotingPolls.Models
{
    public class VotingPollCreateVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Question { get; set; }

        public bool MultipleChoice { get; set; }

        public bool AdditionalAnswers { get; set; }

        [Required]
        public List<Answer> Answers { get; set; }
    }
}
