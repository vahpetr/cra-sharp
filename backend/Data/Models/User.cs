using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models {
    public class User {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength (128), EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength (256), DataType (DataType.Password)]
        public string Password { get; set; }

        public bool IsActivated { get; set; }

        // [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

    }
}