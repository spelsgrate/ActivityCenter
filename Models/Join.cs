using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class Join
    {
        [Key]
        public int JoinId { get; set; }
        public int UserId { get; set; }
        public int OccasionId { get; set; }
        public User SingleUser { get; set; }
        public Occasion AnOccasion { get; set; }
    }
}