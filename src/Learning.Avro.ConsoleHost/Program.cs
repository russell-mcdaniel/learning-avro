using System;
using System.IO;
using Chr.Avro;
using Chr.Avro.Abstract;
using Chr.Avro.Representation;
using Chr.Avro.Serialization;
using Learning.Avro.ConsoleHost.Models;

namespace Learning.Avro.ConsoleHost
{
    class Program
    {
        private static void Main()
        {
            RunTestForDateTimeOffset();
            RunTestForRecord();
            RunTestForSchemaCompatibility();
        }

        private static void RunTestForDateTimeOffset()
        {
            /*
            var dtoIn = DateTimeOffset.Now;
            var dtoStr = dtoIn.ToString("O");
            var dtoOut = DateTimeOffset.Parse(dtoStr);

            var loc = DateTime.Now;
            var utc = loc.ToUniversalTime();

            var dtoFromLocal = new DateTimeOffset(loc);
            var dtoFromUtc = new DateTimeOffset(utc);

            Console.WriteLine((dtoFromUtc == dtoFromLocal) ? "==" : "!=");
            */

            var schema = new StringSchema();

            var serializer = new BinarySerializerBuilder()
                .BuildSerializer<DateTimeOffset>(schema);

            var deserializer = new BinaryDeserializerBuilder()
                .BuildDeserializer<DateTimeOffset>(schema);

            var dtoIn = DateTimeOffset.Now;
            var bytes = serializer.Serialize(dtoIn);
            var dtoOut = deserializer.Deserialize(bytes);

            Console.WriteLine();
            Console.WriteLine($"Serialized:    {dtoIn.ToString("O")}");
            Console.WriteLine($"Deserialized:  {dtoOut.ToString("O")}");
        }

        private static void RunTestForRecord()
        {
            Schema schema;

            using (var stream = new FileStream("./Schemas/Widget.avsc", FileMode.Open))
            {
                var reader = new JsonSchemaReader();
                schema = reader.Read(stream);
            }

            var serializer = new BinarySerializerBuilder()
                .BuildSerializer<Widget>(schema);

            var deserializer = new BinaryDeserializerBuilder()
                .BuildDeserializer<Widget>(schema);

            var widgetIn = new Widget
            {
                Id = 1010,
                Name = "One Thousand Ten",
                Cost = 3829.7401m,
                Lifetime = TimeSpan.FromMilliseconds(987654321),
                GlobalId = new Guid("21d45c13-76b1-459d-8571-ba0ad0fa27de"),
                CreatedAt = DateTime.UtcNow,
                CreatedAtLocal = DateTimeOffset.Now
            };

            var bytes = serializer.Serialize(widgetIn);
            var widgetOut = deserializer.Deserialize(bytes);

            Console.WriteLine();
            Console.WriteLine($"Serialized widget to {bytes.Length} bytes.");
            Console.WriteLine($"Deserialized widget {widgetOut.Id} (\"{widgetOut.Name}\").");
        }

        private static void RunTestForSchemaCompatibility()
        {
            var widgetSchema = GetSchema("Widget");
            var widgetSerializer = new BinarySerializerBuilder().BuildSerializer<Widget>(widgetSchema);
            var widgetDeserializer = new BinaryDeserializerBuilder().BuildDeserializer<Widget>(widgetSchema);

            var widgetPrimeSchema = GetSchema("WidgetPrime");
            var widgetPrimeSerializer = new BinarySerializerBuilder().BuildSerializer<WidgetPrime>(widgetPrimeSchema);
            var widgetPrimeDeserializer = new BinaryDeserializerBuilder().BuildDeserializer<WidgetPrime>(widgetPrimeSchema);

            // Deserialize a widget prime from a widget.
            var widgetIn = new Widget
            {
                Id = 1010,
                Name = "One Thousand Ten",
                Cost = 3829.7401m,
                Lifetime = TimeSpan.FromMilliseconds(987654321),
                GlobalId = new Guid("21d45c13-76b1-459d-8571-ba0ad0fa27de"),
                CreatedAt = DateTime.UtcNow,
                CreatedAtLocal = DateTimeOffset.Now
            };

            var widgetBytes = widgetSerializer.Serialize(widgetIn);
            var widgetPrimeOut = widgetPrimeDeserializer.Deserialize(widgetBytes);

            Console.WriteLine();
            Console.WriteLine($"Serialized widget to {widgetBytes.Length} bytes.");
            Console.WriteLine($"Deserialized widget prime {widgetPrimeOut.Id} (\"{widgetPrimeOut.Name}\").");

            // Deserialize a widget from a widget prime.
            var widgetPrimeIn = new WidgetPrime
            {
                Id = 2020,
                Name = "Two Thousand Twenty",
                Description = "This is a brief description of the premium widget model.",
                Cost = 7401.3829m,
                GlobalId = new Guid("8cb8db7f-c790-4872-a905-86dd3e04d787"),
                CreatedAt = DateTime.UtcNow,
                CreatedAtLocal = DateTimeOffset.Now,
                ShelfLife = TimeSpan.FromMilliseconds(123456789)
            };

            var widgetPrimeBytes = widgetPrimeSerializer.Serialize(widgetPrimeIn);
            var widgetOut = widgetDeserializer.Deserialize(widgetPrimeBytes);

            Console.WriteLine();
            Console.WriteLine($"Serialized widget prime to {widgetPrimeBytes.Length} bytes.");
            Console.WriteLine($"Deserialized widget {widgetOut.Id} (\"{widgetOut.Name}\").");
        }

        private static Schema GetSchema(string name)
        {
            Schema schema;

            using (var stream = new FileStream($"./Schemas/{name}.avsc", FileMode.Open))
            {
                var reader = new JsonSchemaReader();
                schema = reader.Read(stream);
            }

            return schema;
        }
    }
}
