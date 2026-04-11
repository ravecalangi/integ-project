using System;

namespace Program
{
    public class FuelPrice
    {
        public int Id { get; set; }
        public string FuelType { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string StationName { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}