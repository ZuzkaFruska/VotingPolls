using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using VotingPolls.Data;

namespace VotingPolls.Models
{
    public class VoteVM
    {
        public VotingPoll? VotingPoll { get; set; }
        public string VoterId { get; set; }          // User that votes
        public bool UserAlreadyVoted { get; set; }

        public string? NewAnswerValue { get; set; }
        public string Referer { get; set; }


        [Required(ErrorMessage = "At least one answer is required.")]
        public List<int>? UserAnswers { get; set; }
    }
}

