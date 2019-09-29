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
        }

        private static void RunTestForDateTimeOffset()
        {
            /*
            var dtoIn = DateTimeOffset.Now;
            var dtoStr = dtoIn.ToString("O");
            var dtoOut = DateTimeOffset.Parse(dtoStr);
 
            var dtl = DateTime.Now;
            var dtoFromLocal = new DateTimeOffset(dtl);

            var dtu = DateTime.UtcNow;
            var dtoFromUtc = new DateTimeOffset(dtu);
            
            // dtoFromLocal != dtoFromUtc
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
                CreatedAtLocal = new DateTimeOffset(DateTime.Now)
            };

            var bytes = serializer.Serialize(widgetIn);
            var widgetOut = deserializer.Deserialize(bytes);

            Console.WriteLine();
            Console.WriteLine($"Serialized widget to {bytes.Length} bytes.");
            Console.WriteLine($"Deserialized widget {widgetOut.Id} (\"{widgetOut.Name}\").");
        }
    }
}
