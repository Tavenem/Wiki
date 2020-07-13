using Newtonsoft.Json;
using System;

namespace NeverFoundry.Wiki.Converters.NewtonsoftJson
{
    /// <summary>
    /// Converts an object to and from JSON.
    /// </summary>
    public class NoConverter : JsonConverter
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter" /> can read JSON.
        /// </summary>
        /// <value><c>true</c> if this <see cref="JsonConverter" /> can read JSON; otherwise, <c>false</c>.</value>
        public override bool CanRead => false;

        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter" /> can write JSON.
        /// </summary>
        /// <value><c>true</c> if this <see cref="JsonConverter" /> can write JSON; otherwise, <c>false</c>.</value>
        public override bool CanWrite => false;

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) => true;

#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            => throw new NotImplementedException();

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            => throw new NotImplementedException();
#pragma warning restore RCS1079 // Throwing of new NotImplementedException.
    }
}
