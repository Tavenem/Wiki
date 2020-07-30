using Fido2NetLib.Objects;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeverFoundry.Wiki.MvcSample.Data
{
    public class FidoStoredCredential
    {
        public string? Username { get; set; }
        public byte[]? UserId { get; set; }
        public byte[]? PublicKey { get; set; }
        public byte[]? UserHandle { get; set; }
        public uint SignatureCounter { get; set; }
        public string? CredType { get; set; }
        public DateTime RegDate { get; set; }
        public Guid AaGuid { get; set; }

        [JsonIgnore, NotMapped]
        public PublicKeyCredentialDescriptor? Descriptor
        {
            get => string.IsNullOrWhiteSpace(DescriptorJson) ? null : JsonSerializer.Deserialize<PublicKeyCredentialDescriptor>(DescriptorJson);
            set => DescriptorJson = JsonSerializer.Serialize(value);
        }
        public string? DescriptorJson { get; set; }
    }
}
