using System;
using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class Specialty
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<PsychologistProfile> Psychologists { get; set; } = new List<PsychologistProfile>();
    }
}