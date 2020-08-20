using Microsoft.Azure.Cosmos;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace NeverFoundry.Wiki.Samples.Cosmos.Data
{
    public class SystemTextJsonCosmosSerializer : CosmosSerializer
    {
        [return: MaybeNull]
        public override T FromStream<T>(Stream stream)
        {
            var array = new byte[stream.Length];
            var span = new Span<byte>(array);
            stream.Read(span);
            stream.Dispose();
            return JsonSerializer.Deserialize<T>(span);
        }

        public override Stream ToStream<T>(T input)
        {
            var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);
            JsonSerializer.Serialize(writer, input, input.GetType());
            return stream;
        }
    }
}