using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LabsTRVD.Json
{
    /// <summary>
    /// JSON конвертор для DateTime у європейському форматі: dd.MM.yyyy HH:mm:ss
    /// </summary>
    public class EuropeanDateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "dd.MM.yyyy HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateString = reader.GetString();

            if (string.IsNullOrWhiteSpace(dateString))
                throw new JsonException("DateTime value is null or empty");

            if (DateTime.TryParseExact(
                dateString,
                DateFormat,
                CultureInfo.GetCultureInfo("uk-UA"),
                DateTimeStyles.AssumeLocal,
                out DateTime result))
            {
                return DateTime.SpecifyKind(result, DateTimeKind.Local);
            }
            if (DateTime.TryParse(
                dateString,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out result))
            {
                return result;
            }

            throw new JsonException($"Unable to convert \"{dateString}\" to DateTime. Expected format: {DateFormat} or ISO 8601");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            string formattedDate = value.ToString(DateFormat, CultureInfo.GetCultureInfo("uk-UA"));
            writer.WriteStringValue(formattedDate);
        }
    }

    /// <summary>
    /// JSON конвертор для nullable DateTime у європейському форматі: dd.MM.yyyy HH:mm:ss
    /// </summary>
    public class EuropeanNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private const string DateFormat = "dd.MM.yyyy HH:mm:ss";
        private readonly EuropeanDateTimeConverter _innerConverter = new();

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            return _innerConverter.Read(ref reader, typeof(DateTime), options);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                string formattedDate = value.Value.ToString(DateFormat, CultureInfo.GetCultureInfo("uk-UA"));
                writer.WriteStringValue(formattedDate);
            }
        }
    }
}