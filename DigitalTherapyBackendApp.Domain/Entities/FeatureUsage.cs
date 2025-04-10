using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class FeatureUsage
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string FeatureName { get; set; }
        public DateTime UsageTime { get; set; }
        public bool IsDeleted { get; set; } = false;

        public virtual User User { get; set; }
    }
}
