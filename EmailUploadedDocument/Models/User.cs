using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmailUploadedDocument.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        public ICollection<Document> Documents { get; set; }

        public User()
        {
            Documents = new List<Document>();
        }
    }
}