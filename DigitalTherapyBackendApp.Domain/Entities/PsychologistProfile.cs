﻿using System;
using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class PsychologistProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? NotificationPreferences { get; set; }
        public User User { get; set; }
        public Guid? InstitutionId { get; set; }
        public string? InstitutionName { get; set; }
        public InstitutionProfile? Institution { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public string? Experience { get; set; }
        public string? LicenseNumber { get; set; }
        public ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
        public bool IsAvailable { get; set; }
        public ICollection<PsychologistAvailabilitySlot> AvailabilitySlots { get; set; } = new List<PsychologistAvailabilitySlot>();
    }
}