using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiffCost.Models
{
    public class User
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        public string UserAccountId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Editable(false)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Editable(false)]
        public DateTime UpdatedAt { get; set; }
    }
   
}