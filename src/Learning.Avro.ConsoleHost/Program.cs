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
            RunTestForRecord();
        }

        private static void RunTestForRecord()
        {
            Schema schema;

            using (var stream = new FileStream("./Schemas/Widget.avsc", FileMode.Open))
            {
                var reader = new JsonSchemaReader();
                schema = reader.Read(stream);
            }

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

            var serializer = new BinarySerializerBuilder()
                .BuildSerializer<Widget>(schema);

            var bytes = serializer.Serialize(widgetIn);

            Console.WriteLine($"Serialized widget to {bytes.Length} bytes.");

            var deserializer = new BinaryDeserializerBuilder()
                .BuildDeserializer<Widget>(schema);

            var widgetOut = deserializer.Deserialize(bytes);

            Console.WriteLine($"Deserialized widget {widgetOut.Id} (\"{widgetOut.Name}\").");
        }
    }
}
