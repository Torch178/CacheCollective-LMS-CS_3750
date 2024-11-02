namespace RazorPagesMovie.Models
{
    public class PaymentDetails
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public decimal? total { get; set; }
        public DateTime createdDate { get; set; }
        public string payMethod { get; set; }
        public Semesters semester { get; set; }
        public PayStatus status { get; set; }

        //Constructor
        public PaymentDetails(int userId, decimal? total, DateTime createdDate, string payMethod)
        {
            this.userId = userId;
            this.total = total;
            this.createdDate = createdDate;
            this.payMethod = payMethod;
            this.status = PayStatus.Pending;
            this.semester = FindSemester(createdDate);

        }

        public async Task<PayStatus> SetPayStatus(int value, RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            PayStatus ps = new PayStatus();
            switch (value)
            {
                case 0:
                    ps = PayStatus.Paid;
                    break;
                case 1:
                    ps = PayStatus.Pending;
                    break;
                case 2:
                    ps = PayStatus.Declined;
                    break;
                case 3:
                    ps = PayStatus.Refund;
                    break;
                case 4:
                    ps = PayStatus.Test;
                    break;
                default:
                    ps = PayStatus.Test;
                    break;
            }
            this.status = ps;
            await context.SaveChangesAsync();
            return this.status;
        }

        //Calcualte Semester enum based on Payment date
        public Semesters FindSemester(DateTime createdDate)
        {
            int day = createdDate.Day;
            int month = createdDate.Month;

            //Handle cases where semester months overlap
            if (month == 8)
            {
                if (day > 16) return Semesters.Fall;
                else return Semesters.Summer;
            }
            else if (month == 12)
            {
                if (day > 13) return Semesters.Spring;
                else return Semesters.Fall;
            }
            else if (month == 4)
            {
                if (day > 25) return Semesters.Summer;
                else return Semesters.Spring;
            }
            //Handle cases where month is in deterministic range for semesters
            else if (month >= 9 && month <= 11) return Semesters.Fall;
            else if (month >= 1 && month <= 3) return Semesters.Spring;
            else return Semesters.Summer;
        }

        public string DisplaySemester()
        {
            string output = "";
            switch ((int)this.semester)
            {
                case 0:
                    output += "Spring";
                    break;
                case 1:
                    output += "Summer";
                    break;
                case 2:
                    output += "Fall";
                    break;

            }
            output += this.createdDate.ToString(" YYYY");
            return output;
        }

        public PayStatus GetPayStatus(int value)
        {
            PayStatus ps = new PayStatus();
            switch (value)
            {
                case 0:
                    ps = PayStatus.Paid;
                    break;
                case 1:
                    ps = PayStatus.Pending;
                    break;
                case 2:
                    ps = PayStatus.Declined;
                    break;
                case 3:
                    ps = PayStatus.Refund;
                    break;
                case 4:
                    ps = PayStatus.Test;
                    break;
                default:
                    ps = PayStatus.Test;
                    break;
            }
            return ps;
        }

        public override string ToString()
        {
            string output = "";
            output += "Payment Id: " + this.Id.ToString() + "\t";
            output += "Amount Paid: " + this.total.ToString() + "\t";
            output += "Pay Method: " + "************" + this.payMethod + "\n";
            return output;
        }
    }
}