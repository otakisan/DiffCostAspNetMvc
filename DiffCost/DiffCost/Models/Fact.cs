using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiffCost.Models
{
    public class Fact
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ProjectName { get; set; }

        public string FactText { get; set; }

        public float ManDay { get; set; }

        [Required]
        [Editable(false)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Editable(false)]
        public DateTime UpdatedAt { get; set; }
    }
}