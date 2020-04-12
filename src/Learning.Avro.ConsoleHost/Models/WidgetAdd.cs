using System;

namespace Learning.Avro.ConsoleHost.Models
{
    public class WidgetAdd
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Cost { get; set; }

        public TimeSpan Lifetime { get; set; }

        public Guid GlobalId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTimeOffset CreatedAtLocal { get; set; }

        public long SufferingIndex { get; set; }
    }
}
