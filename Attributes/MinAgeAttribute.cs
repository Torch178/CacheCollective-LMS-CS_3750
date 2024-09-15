using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Attributes
{
    public class MinAgeAttribute : ValidationAttribute
    {
        private int minAge;

        public MinAgeAttribute(int minAge)
        {
            this.minAge = minAge;
            ErrorMessage = "You must be at least " + this.minAge + " years old!";
        }

        public override bool IsValid(object? value)
        {
            if (value is DateTime birthDate)
            {
                var todayDate = DateTime.Today;
                var age = todayDate.Year - birthDate.Year;

                if (birthDate > todayDate.AddYears(-age)) age--; // Check if birthdate has passed
                if (age < this.minAge) return false;
            }

            return true;
        }
    }
}
