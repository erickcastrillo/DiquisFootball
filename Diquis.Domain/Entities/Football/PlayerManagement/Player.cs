using System.ComponentModel.DataAnnotations.Schema;
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;

namespace Diquis.Domain.Entities.Football.PlayerManagement
{
    /// <summary>
    /// Represents a football player including personal data, assigned position,
    /// guardian reference, revision history and a computed current skill snapshot.
    /// </summary>
    public class Player : AuditableEntity
    {
        /// <summary>
        /// Player's given name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Player's family name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Player's date of birth (used to compute ReadableAge()).
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Preferred foot (Left, Right, Both, or None).
        /// </summary>
        public Foot PreferredFoot { get; set; }

        /// <summary>
        /// Shirt number assigned to the player.
        /// </summary>
        public int JerseyNumber { get; set; }

        /// <summary>
        /// Height in centimeters.
        /// </summary>
        public decimal HeightInCm { get; set; }

        /// <summary>
        /// Weight in kilograms.
        /// </summary>
        public decimal WeightInKg { get; set; }

        /// <summary>
        /// Free-form medical notes relevant to the player.
        /// </summary>
        public string MedicalNotes { get; set; }

        /// <summary>
        /// FK to the guardian ApplicationUser (required if player is a minor in your domain).
        /// </summary>
        public Guid GuardianId { get; set; }

        /// <summary>
        /// Navigation to the guardian user account.
        /// </summary>
        public ApplicationUser Guardian { get; set; }

        /// <summary>
        /// FK to the primary playing position.
        /// </summary>
        public Guid PositionId { get; set; }

        /// <summary>
        /// Navigation to the primary position entity.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Revision history for this player. Each <see cref="Revision"/> is a snapshot
        /// that may contain one or more <see cref="PlayerSkill"/> entries (historical skill ratings).
        /// </summary>
        public ICollection<Revision> Revisions { get; set; } = new HashSet<Revision>();

        /// <summary>
        /// Computed set of player skills that belong to the most recent revision.
        /// </summary>
        /// <remarks>
        /// - This property is <see cref="NotMappedAttribute"/> so it is not persisted.
        /// - It returns <c>LastRevision()?.PlayerSkills</c>.
        /// - Use explicit queries against the DbContext to avoid loading entire history for large datasets.
        /// </remarks>
        [NotMapped]
        public IEnumerable<PlayerSkill> CurrentPlayerSkills
        {
            get
            {
                Revision last = LastRevision();
                return last?.PlayerSkills ?? Enumerable.Empty<PlayerSkill>();
            }
        }

        /// <summary>
        /// Returns the latest <see cref="Revision"/> for this player ordered by <c>CreatedOn</c>.
        /// </summary>
        /// <returns>The most recent revision or <c>null</c> if none exist.</returns>
        public Revision LastRevision()
        {
            return Revisions
                .OrderByDescending(r => r.CreatedOn)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns a human-readable representation of the player's age, e.g. "20 years and 5 months",
        /// "1 year and 1 month", or "12 days" when under one month.
        /// </summary>
        /// <returns>Localized-ish age string built from years, months and days.</returns>
        public string ReadableAge()
        {
            DateTime today = DateTime.Today;

            if (DateOfBirth > today)
            {
                return "0 days";
            }

            int years = today.Year - DateOfBirth.Year;
            int months = today.Month - DateOfBirth.Month;
            int days = today.Day - DateOfBirth.Day;

            if (days < 0)
            {
                DateTime prevMonth = today.AddMonths(-1);
                days += DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
                months--;
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            List<string> parts = [];

            if (years > 0)
            {
                parts.Add($"{years} year{(years > 1 ? "s" : "")}");
            }

            if (months > 0)
            {
                parts.Add($"{months} month{(months > 1 ? "s" : "")}");
            }

            // If age is less than one month, show days
            if (years == 0 && months == 0)
            {
                parts.Add($"{days} day{(days != 1 ? "s" : "")}");
            }

            return string.Join(" and ", parts);
        }
    }
}
