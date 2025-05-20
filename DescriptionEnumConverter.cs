namespace PersonalFinanceTrackerAPI;

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DescriptionEnumConverter<TEnum> : JsonConverterFactory
    where TEnum : struct, Enum
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum && typeToConvert.Equals(typeof(TEnum));
    }

    public override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return new InnerConverter();
    }

    private class InnerConverter : JsonConverter<TEnum>
    {
        public override TEnum Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected string for enum {typeToConvert.Name}");
            }

            string? enumString = reader.GetString();

            if (string.IsNullOrEmpty(enumString))
            {
                throw new JsonException($"Null or empty string is not allowed for enum {typeToConvert.Name}");
            }

            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                if (GetDescription(enumValue).Equals(enumString, StringComparison.OrdinalIgnoreCase) ||
                    enumValue.ToString().Equals(enumString, StringComparison.OrdinalIgnoreCase))
                {
                    return enumValue;
                }
            }

            throw new JsonException($"Invalid value '{enumString}' for enum {typeToConvert.Name}");
        }

        public override void Write(
            Utf8JsonWriter writer,
            TEnum value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(GetDescription(value));
        }

        private static string GetDescription(Enum value)
        {
            return value.GetType()
                        .GetMember(value.ToString())
                        .FirstOrDefault()
                        ?.GetCustomAttribute<DescriptionAttribute>()
                        ?.Description ?? value.ToString();
        }
    }
}