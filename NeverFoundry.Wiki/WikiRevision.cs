using NeverFoundry.DataStorage;
using NeverFoundry.DiffPatchMerge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A particular revision of a wiki item.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    public class WikiRevision : IdItem, ISerializable
    {
        /// <summary>
        /// An optional comment supplied for this revision (e.g. to explain the changes).
        /// </summary>
        public string? Comment { get; }

        /// <summary>
        /// The ID of the user who made this revision.
        /// </summary>
        public string Editor { get; }

        /// <summary>
        /// Gets the full title of the item at the tile of this revision (including namespace if the
        /// namespace was not
        /// <see cref="WikiConfig.DefaultNamespace"/>).
        /// </summary>
        public string FullTitle => string.CompareOrdinal(WikiNamespace, WikiConfig.DefaultNamespace) == 0
            ? Title
            : $"{WikiNamespace}:{Title}";

        /// <summary>
        /// Whether the item was marked as deleted by this revision.
        /// </summary>
        public bool IsDeleted { get; }

        /// <summary>
        /// Whether the item's contents were entirely replaced by this revision.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Milestones occur after deletions, or when the revision includes more than 75% deletion
        /// of the previous text and the addition of at least three times as much text as the amount
        /// of the original which remains.
        /// </para>
        /// <para>
        /// The purpose of milestone revisions is to increase the efficiency of the process which
        /// recreates the state of the text at a previous point in time by applying past revisions.
        /// If the text radically changes at any given point in its history, it is more efficient to
        /// store the full text at that point than to process all the incremental revisions prior to
        /// that point.
        /// </para>
        /// </remarks>
        public bool IsMilestone { get; }

        /// <summary>
        /// <para>
        /// The revision, as a delta-formatted string (except for milestones, which contain the full
        /// text).
        /// </para>
        /// <para>
        /// <see langword="null"/> when <see cref="IsDeleted"/> is <see langword="true"/>
        /// </para>
        /// </summary>
        public string? Revision { get; }

        /// <summary>
        /// The timestamp of this revision, in UTC.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public DateTimeOffset Timestamp => new DateTimeOffset(TimestampTicks, TimeSpan.Zero);

        /// <summary>
        /// The timestamp of this revision, in UTC Ticks.
        /// </summary>
        public long TimestampTicks { get; }

        /// <summary>
        /// The title of the item at the time of this revision. Must be non-empty, but may not be
        /// unique.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// A unique ID that identifies this wiki item across revisions.
        /// </summary>
        public string WikiId { get; }

        /// <summary>
        /// The namespace to which this item belonged at the time of this revision. Must be
        /// non-empty.
        /// </summary>
        public string WikiNamespace { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiRevision"/>.
        /// </summary>
        /// <param name="wikiId">
        /// A unique ID that identifies this wiki item across revisions.
        /// </param>
        /// <param name="editor">
        /// The ID of the user who made this revision.
        /// </param>
        /// <param name="title">
        /// The title of the item at the time of this revision. Must be non-empty, but may not be
        /// unique.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace to which this item belonged at the time of this revision. Must be
        /// non-empty.
        /// </param>
        /// <param name="previousText">
        /// The original text, before this revision.
        /// </param>
        /// <param name="newText">
        /// The new text, after the revision.
        /// </param>
        /// <param name="comment">
        /// An optional comment supplied for this revision (e.g. to explain the changes).
        /// </param>
        public WikiRevision(
            string wikiId,
            string editor,
            string title,
            string wikiNamespace,
            string? previousText = null,
            string? newText = null,
            string? comment = null)
        {
            TimestampTicks = DateTimeOffset.UtcNow.Ticks;

            Editor = editor;
            Comment = comment;
            Title = title;
            WikiId = wikiId;
            WikiNamespace = wikiNamespace;

            if (string.IsNullOrWhiteSpace(newText))
            {
                IsDeleted = true;
            }
            else if (string.IsNullOrWhiteSpace(previousText))
            {
                IsMilestone = true;
                Revision = newText;
            }
            else
            {
                var revision = DiffPatchMerge.Revision.GetRevison(previousText, newText);
                if (!revision.Patches.Any(x => x.Operation == DiffOperation.Deleted))
                {
                    Revision = revision.ToString();
                }
                else
                {
                    var deletionLength = revision.Patches.Where(x => x.Operation == DiffOperation.Deleted).Sum(x => x.Length);
                    if (deletionLength < previousText.Length * 0.75)
                    {
                        Revision = revision.ToString();
                    }
                    else
                    {
                        var insertionLength = revision.Patches.Where(x => x.Operation == DiffOperation.Inserted).Sum(x => x.Length);
                        if (insertionLength >= (previousText.Length - deletionLength) * 3)
                        {
                            IsMilestone = true;
                            Revision = newText;
                        }
                        else
                        {
                            Revision = revision.ToString();
                        }
                    }
                }
            }
        }

        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        private WikiRevision(
            string id,
            string wikiId,
            string editor,
            string title,
            string wikiNamespace,
            string revision,
            bool isDeleted,
            bool isMilestone,
            string? comment,
            long timestampTicks) : base(id)
        {
            if (isDeleted && isMilestone)
            {
                throw new ArgumentException($"{nameof(isDeleted)} and {nameof(isMilestone)} cannot both be true", nameof(isDeleted));
            }

            Editor = editor;
            IsDeleted = isDeleted;
            IsMilestone = isMilestone;
            Comment = comment;
            Revision = isDeleted ? null : revision;
            TimestampTicks = timestampTicks;
            Title = title;
            WikiId = wikiId;
            WikiNamespace = wikiNamespace;
        }

        private WikiRevision(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(WikiId), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Editor), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(WikiNamespace), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Revision), typeof(string)) ?? string.Empty,
            (bool?)info.GetValue(nameof(IsDeleted), typeof(bool)) ?? default,
            (bool?)info.GetValue(nameof(IsMilestone), typeof(bool)) ?? default,
            (string?)info.GetValue(nameof(Comment), typeof(string)) ?? string.Empty,
            (long?)info.GetValue(nameof(TimestampTicks), typeof(long)) ?? default)
        { }

        /// <summary>
        /// Gets a diff which represents the final revision in the given sequence.
        /// </summary>
        /// <param name="revisions">
        /// A sequence of revisions to a text.
        /// </param>
        /// <param name="format">
        /// <para>
        /// The format used.
        /// </para>
        /// <para>
        /// Can be either "delta" (the default), "gnu", "md", or "html" (case insensitive).
        /// </para>
        /// <para>
        /// The "delta" format (the default, used if an empty string or whitespace is passed)
        /// renders a compact, encoded string which describes each diff operation. The first
        /// character is '=' for unchanged text, '+' for an insertion, and '-' for deletion.
        /// Unchanged text and deletions are followed by their length only; insertions are followed
        /// by a compressed version of their full text. Each diff is separated by a tab character
        /// ('\t').
        /// </para>
        /// <para>
        /// The "gnu" format renders the text preceded by "- " for deletion, "+ " for addition, or
        /// nothing if the text was unchanged. Each diff is separated by a newline.
        /// </para>
        /// <para>
        /// The "md" format renders the text surrounded by "~~" for deletion, "++" for addition, or
        /// nothing if the text was unchanged. Diffs are concatenated without separators.
        /// </para>
        /// <para>
        /// The "html" format renders the text surrounded by a span with class "diff-deleted" for
        /// deletion, "diff-inserted" for addition, or without a wrapping span if the text was
        /// unchanged. Diffs are concatenated without separators.
        /// </para>
        /// </param>
        /// <returns>
        /// A string representing the final revision in the given sequence.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// A revision was incorrectly formatted; or, the sequence of revisions is not a
        /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
        /// order given.
        /// </exception>
        public static string GetDiff(IReadOnlyList<WikiRevision> revisions, string format = "md")
        {
            if (revisions.Count == 0)
            {
                return string.Empty;
            }
            var text = string.Empty;
            foreach (var revision in revisions.Take(revisions.Count - 1))
            {
                if (revision.IsDeleted)
                {
                    text = string.Empty;
                }
                else if (revision.IsMilestone)
                {
                    text = revision.Revision ?? string.Empty;
                }
                else if (string.IsNullOrEmpty(revision.Revision))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        throw new ArgumentException("A revision was incorrectly formatted", nameof(revisions));
                    }
                }
                else if (DiffPatchMerge.Revision.TryParse(revision.Revision, out var r))
                {
                    text = r.Apply(text);
                }
                else
                {
                    throw new ArgumentException("A revision was incorrectly formatted", nameof(revisions));
                }
            }
            var finalText = string.Empty;
            if (!revisions[revisions.Count - 1].IsDeleted)
            {
                if (revisions[revisions.Count - 1].IsMilestone)
                {
                    finalText = revisions[revisions.Count - 1].Revision ?? string.Empty;
                }
                else if (string.IsNullOrEmpty(revisions[revisions.Count - 1].Revision))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        throw new ArgumentException("A revision was incorrectly formatted", nameof(revisions));
                    }
                }
                else if (DiffPatchMerge.Revision.TryParse(revisions[revisions.Count - 1].Revision!, out var r))
                {
                    finalText = r.Apply(text);
                }
                else
                {
                    throw new ArgumentException("A revision was incorrectly formatted", nameof(revisions));
                }
            }
            return Diff.GetWordDiff(text, finalText).ToString(format);
        }

        /// <summary>
        /// Gets the text as it appeard after the given set of <paramref name="revisions"/>.
        /// </summary>
        /// <param name="revisions">
        /// A sequence of revisions to a text.
        /// </param>
        /// <returns>
        /// The text as it appeard after the given set of <paramref name="revisions"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// A revision was incorrectly formatted; or, the sequence of revisions is not a
        /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
        /// order given.
        /// </exception>
        public static string GetText(IEnumerable<WikiRevision> revisions)
        {
            var text = string.Empty;
            foreach (var revision in revisions)
            {
                if (revision.IsDeleted)
                {
                    text = string.Empty;
                }
                else if (revision.IsMilestone)
                {
                    text = revision.Revision ?? string.Empty;
                }
                else if (string.IsNullOrEmpty(revision.Revision))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        throw new ArgumentException("A revision was incorrectly formatted", nameof(revisions));
                    }
                }
                else if (DiffPatchMerge.Revision.TryParse(revision.Revision, out var r))
                {
                    text = r.Apply(text);
                }
                else
                {
                    throw new ArgumentException("A revision was incorrectly formatted", nameof(revisions));
                }
            }
            return text;
        }

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(WikiId), WikiId);
            info.AddValue(nameof(Editor), Editor);
            info.AddValue(nameof(Title), Title);
            info.AddValue(nameof(WikiNamespace), WikiNamespace);
            info.AddValue(nameof(Revision), Revision);
            info.AddValue(nameof(IsDeleted), IsDeleted);
            info.AddValue(nameof(IsMilestone), IsMilestone);
            info.AddValue(nameof(Comment), Comment);
            info.AddValue(nameof(TimestampTicks), TimestampTicks);
        }
    }
}
