using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
       public class  FutureDateAttribute : ValidationAttribute
    { 
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime)
            {
                if ((DateTime)value >= DateTime.Now)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Activity Date cannot be in the past");
                }
            }
            else
            {
                return new ValidationResult("Not a valid date");
            }

        }

    }
    [Table("occasions")]
    public class Occasion
    {

        [Key]
        public int OccasionId { get; set; }
        [Required(ErrorMessage = "A Name is required")]
        public string Title { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "Please send us your time machine! Date must not be in past!")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Field must not be empty")]
        public TimeSpan Time {get;set;}

        [Required(ErrorMessage = "Field must not be empty")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Field must not be empty")]
        public string DurationType {get;set;}

        [Required(ErrorMessage = "A Description  is required")]
        [MinLength(5, ErrorMessage = "Description must be at least 5 characters long")]
        public string Description { get; set; }


        public User Coordinator { get; set; }
        public int UserID { get; set; }


        public List<Join> Attendees { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.Now;
        public DateTime UpdatedAt { get; set; } =DateTime.Now;
    }
}