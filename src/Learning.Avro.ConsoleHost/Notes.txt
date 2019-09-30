================================================================================
Type Mapping
================================================================================

This page documents the type mapping behaviors and recommendations for
the Chr.Avro library:

https://engineering.chrobinson.com/dotnet-avro/internals/mapping


================================================================================
Issues
================================================================================

--------------------------------------------------------------------------------
Open
--------------------------------------------------------------------------------

*** DateTimeOffset loses its offset value.

Using string serialization as recommended, the DateTimeOffset loses its offset
value. Upon tracing through the DateTimeOffsetValues test in the string tests,
it looks like the library attempts to treat DateTime and DateTimeOffets as
equivalent when they are not.

On serialization, the library uses the UtcDateTime property of the offset for
its value which is a DateTime value with no notion of offset. Even if this
were deserialized properly into a DateTimeOffset, the offset would be wrong
if it was deserialized on a host configured for a different time zone than
where the data was captured because it would calculate the offset based on
the time zone of the local host instead of the originating host.

The Chr.Avro unit tests pass because they create DateTimeOffsets from
DateTime values instead of creating DateTimeOffsets natively.

Test 1: Direct String Conversion

* Using DateTimeOffet.ToString("O") and then DateTimeOff.Parse()
  yields the expected result: The offset value is preserved.

Test 2: DateTimeOffset Schema

* Serializing and deserializing a DateTimeOffset value with an
  Avro schema for a single DateTimeOffset value yields a DateTimeOffset
  with the correct local time, but 

Reference:

BinarySerializerBuilder.cs / StringSerializerBuilderCase.BuildDelegate()

  result = Expression.PropertyOrField(result, nameof(DateTimeOffset.UtcDateTime));

BinaryDeserializerBuilder.cs / StringDeserializerBuilderCase.BuildDelegate()

  var parseDateTime = typeof(DateTime)
      .GetMethod(nameof(DateTime.Parse), new[]
      {
          typeof(string),
          typeof(IFormatProvider),
          typeof(DateTimeStyles)
      });

Note usage of the UtcDateTime property to get the value of the DateTimeOffset
instead of just using the value of the DateTimeOffset itself.

This behavior can be effectively be demonstrated with the following code:

  var dtl = DateTime.Now;
  var dtoFromLocal = new DateTimeOffset(dtl);

  var dtu = DateTime.UtcNow;
  var dtoFromUtc = new DateTimeOffset(dtu);

Note that the local and UTC properties are correct in both cases, but in the
latter case, the offset value is 00:00. An equality check shows that these
values are not considered equal. Contrast with the passing Chr.Avro tests.

This raises another question about how DateTimeOffset works. If the local and
UTC times are different, how can the offset be 00:00?

Reference:

* https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset?view=netcore-3.0
* https://docs.microsoft.com/en-us/dotnet/standard/datetime/instantiating-a-datetimeoffset-object?view=netcore-3.0

--------------------------------------------------------------------------------
Closed
--------------------------------------------------------------------------------

*** Serializer cannot be created for decimal field on record.

The logical type cannot be specified directly on the record field. The
decimal schema must be nested on the type property.
