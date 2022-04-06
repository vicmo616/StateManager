using System.ComponentModel.DataAnnotations;

namespace StateManagerModels
{
    public class State
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [StringLength(10)]
        public string Abbreviation { get; set; }


    }
}