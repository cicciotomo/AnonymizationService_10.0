using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Porini.Abp.StateMachineEngine
{
    public class StateMachineEventJsonConverter : JsonConverter<Event>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableTo(typeof(Event));
        }

        public override Event Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            reader.Read();
            var eventType = reader.GetString();
            var evt = Activator.CreateInstance(Type.GetType(eventType));
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return (Event)evt;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Event value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("EventTypeName", value.GetType().GetFullNameWithAssemblyName());
            writer.WriteEndObject();
        }
    }
}
