using System;

namespace Learning.Avro.ConsoleHost.Models
{
    public class WidgetPrime
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Cost { get; set; }

        public Guid GlobalId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTimeOffset CreatedAtLocal { get; set; }

        public TimeSpan ShelfLife { get; set; }
    }
}
