using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CryptLab.Infrastructure.Extensions;

public class DSAParametersConverter : JsonConverter<DSAParameters>
{
    public override DSAParameters Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var parameters = new DSAParameters();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case nameof(parameters.P):
                        parameters.P = reader.GetBytesFromBase64();
                        break;
                    case nameof(parameters.Q):
                        parameters.Q = reader.GetBytesFromBase64();
                        break;
                    case nameof(parameters.G):
                        parameters.G = reader.GetBytesFromBase64();
                        break;
                    case nameof(parameters.X):
                        parameters.X = reader.GetBytesFromBase64();
                        break;
                    case nameof(parameters.Y):
                        parameters.Y = reader.GetBytesFromBase64();
                        break;
                }
            }
        }

        return parameters;
    }

    public override void Write(Utf8JsonWriter writer, DSAParameters value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteBase64String(nameof(value.P), value.P);
        writer.WriteBase64String(nameof(value.Q), value.Q);
        writer.WriteBase64String(nameof(value.G), value.G);
        writer.WriteBase64String(nameof(value.X), value.X);
        writer.WriteBase64String(nameof(value.Y), value.Y);

        writer.WriteEndObject();
    }
}