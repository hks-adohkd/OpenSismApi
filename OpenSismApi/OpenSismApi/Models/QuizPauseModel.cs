using System.ComponentModel.DataAnnotations;

namespace OpenSismApi.Models
{
    public class QuizPauseModel
    {
        [Required(ErrorMessage = "ApptaskId is required")]
        public int AppTaskId { get; set; }   

        [Required(ErrorMessage = "Index is required")]
        public int Index { get; set; }

        public int Points { get; set; } 

        public string description { get; set; }
    }
}
