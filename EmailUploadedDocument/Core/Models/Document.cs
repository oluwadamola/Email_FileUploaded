using System.ComponentModel;

namespace EmailUploadedDocument.Core.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        
        [DisplayName("Document Name")]
        public string DocumentName { get; set; }

        public byte[] Doc { get; set; }

        [DisplayName("Transaction Number")]
        public long TransactionNumber { get; set; }

        public string Extension { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public string DocumentString { get; set; }
    }
}