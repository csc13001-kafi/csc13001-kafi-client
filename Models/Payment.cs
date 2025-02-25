using System;

namespace kafi.Models
{
    public enum PaymentType
    {
        Cash = 0,
        Banking = 1,
        Momo = 2,
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Canceled = 3
    }

    public class Payment
    {
        public int Id { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }
}
