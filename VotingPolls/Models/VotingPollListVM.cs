using System.ComponentModel.DataAnnotations;
using VotingPolls.Data;

namespace VotingPolls.Models
{
    public class VotingPollListVM 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Question { get; set; }


        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

        //public User User { get; set; }

    }
}
