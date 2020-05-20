using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A template parameter included in an <see cref="Article"/>.
    /// </summary>
    [Serializable]
    public class ParameterInclusion : ISerializable
    {
        /// <summary>
        /// The default value of the parameter.
        /// </summary>
        public string? DefaultValue { get; }

        /// <summary>
        /// The name of the parameter (may be a numeric index).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The length of the parameter text in the article (including template characters).
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// The index within the markup where the parameter begins (including template characters).
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ParameterInclusion"/>.
        /// </summary>
        /// <param name="name">
        /// <para>
        /// The full name of the article being transcluded.
        /// </para>
        /// <para>
        /// The most recent revision at the time of processing is selected.
        /// </para>
        /// </param>
        /// <param name="position">
        /// The index within the markup where the parameter begins (including template characters).
        /// </param>
        /// <param name="length">
        /// The length of the parameter text in the article (including template characters).
        /// </param>
        /// <param name="defaultValue">
        /// The default value of the parameter.
        /// </param>
        public ParameterInclusion(string name, int position, int length, string? defaultValue = null)
        {
            Name = name;
            Position = position;
            Length = length;
            DefaultValue = defaultValue;
        }

        private ParameterInclusion(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Name), typeof(string)) ?? string.Empty,
            (int?)info.GetValue(nameof(Position), typeof(int)) ?? 0,
            (int?)info.GetValue(nameof(Length), typeof(int)) ?? 0,
            (string?)info.GetValue(nameof(DefaultValue), typeof(string)))
        { }

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Position), Position);
            info.AddValue(nameof(Length), Length);
            info.AddValue(nameof(DefaultValue), DefaultValue);
        }
    }
}
