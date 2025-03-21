using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class DirectMessage
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid? RelationshipId { get; set; } // TherapistPatientRelationship ile bağlantı
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }
        public string MessageType { get; set; } // Text, File, Image
        public string Attachment { get; set; } // Dosya veya resim URL'i
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public TherapistPatientRelationship Relationship { get; set; }
    }
}
