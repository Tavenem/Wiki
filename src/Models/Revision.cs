﻿using System.Text.Json.Serialization;
using Tavenem.DiffPatchMerge;

namespace Tavenem.Wiki;

/// <summary>
/// A particular revision of a wiki item.
/// </summary>
public class Revision : IEquatable<Revision>
{
    /// <summary>
    /// An optional comment supplied for this revision (e.g. to explain the changes).
    /// </summary>
    public string? Comment { get; }

    /// <summary>
    /// <para>
    /// A delta-formatted string representing the revision (except for milestones, which contain the
    /// full text).
    /// </para>
    /// <para>
    /// <see langword="null"/> when <see cref="IsDeleted"/> is <see langword="true"/>
    /// </para>
    /// </summary>
    /// <remarks>
    /// The revision recorded directly on a <see cref="Page"/> has its <see cref="Delta"/> set to
    /// <see langword="null"/> when <see cref="IsMilestone"/> is <see langword="true"/>, to avoid
    /// duplicating the current content.
    /// </remarks>
    public string? Delta { get; }

    /// <summary>
    /// The ID of the user who made this revision.
    /// </summary>
    public string Editor { get; }

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
    /// The timestamp of this revision, in UTC.
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset Timestamp => new(TimestampTicks, TimeSpan.Zero);

    /// <summary>
    /// The timestamp of this revision, in UTC Ticks.
    /// </summary>
    public long TimestampTicks { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Revision"/>.
    /// </summary>
    /// <param name="editor">
    /// The ID of the user who made this revision.
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
    public Revision(
        string editor,
        string? previousText = null,
        string? newText = null,
        string? comment = null)
    {
        TimestampTicks = DateTimeOffset.UtcNow.Ticks;

        Editor = editor;
        Comment = comment;

        if (string.IsNullOrWhiteSpace(newText))
        {
            IsDeleted = !string.IsNullOrWhiteSpace(previousText);
        }
        else if (string.IsNullOrWhiteSpace(previousText))
        {
            IsMilestone = true;
            Delta = newText;
        }
        else
        {
            var revision = DiffPatchMerge.Revision.GetRevison(previousText, newText);
            if (!revision.Patches.Any(x => x.Operation == DiffOperation.Deleted))
            {
                Delta = revision.ToString();
            }
            else
            {
                var deletionLength = revision.Patches.Where(x => x.Operation == DiffOperation.Deleted).Sum(x => x.Length);
                if (deletionLength < previousText.Length * 0.75)
                {
                    Delta = revision.ToString();
                }
                else
                {
                    var insertionLength = revision.Patches.Where(x => x.Operation == DiffOperation.Inserted).Sum(x => x.Length);
                    if (insertionLength >= (previousText.Length - deletionLength) * 3)
                    {
                        IsMilestone = true;
                        Delta = newText;
                    }
                    else
                    {
                        Delta = revision.ToString();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Revision"/>.
    /// </summary>
    /// <param name="editor">
    /// The ID of the user who made this revision.
    /// </param>
    /// <param name="delta">
    /// <para>
    /// A delta-formatted string representing the revision (except for milestones, which contain
    /// the full text).
    /// </para>
    /// <para>
    /// <see langword="null"/> when <paramref name="isDeleted"/> is <see langword="true"/>
    /// </para>
    /// </param>
    /// <param name="isDeleted">
    /// Whether the item was marked as deleted by this revision.
    /// </param>
    /// <param name="isMilestone">
    /// Whether the item's contents were entirely replaced by this revision.
    /// </param>
    /// <param name="comment">
    /// An optional comment supplied for this revision (e.g. to explain the changes).
    /// </param>
    /// <param name="timestampTicks">
    /// The timestamp of this revision, in UTC Ticks.
    /// </param>
    /// <remarks>
    /// Note: this constructor is most useful for deserialization. The other constructors are more
    /// suited to creating a new instance, as they will automatically generate an appropriate ID.
    /// </remarks>
    [JsonConstructor]
    public Revision(
        string editor,
        string? delta,
        bool isDeleted,
        bool isMilestone,
        string? comment,
        long timestampTicks)
    {
        if (isDeleted && isMilestone)
        {
            throw new ArgumentException($"{nameof(isDeleted)} and {nameof(isMilestone)} cannot both be true", nameof(isDeleted));
        }

        Editor = editor;
        IsDeleted = isDeleted;
        IsMilestone = isMilestone;
        Comment = comment;
        Delta = isDeleted ? null : delta;
        TimestampTicks = timestampTicks;
    }

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
    public static string GetDiff(IReadOnlyList<Revision> revisions, string format = "md")
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
                text = revision.Delta ?? string.Empty;
            }
            else if (string.IsNullOrEmpty(revision.Delta))
            {
                if (!string.IsNullOrEmpty(text))
                {
                    throw new ArgumentException("A revision was incorrectly formatted", nameof(revisions));
                }
            }
            else if (DiffPatchMerge.Revision.TryParse(revision.Delta, out var r))
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
                finalText = revisions[revisions.Count - 1].Delta ?? string.Empty;
            }
            else if (string.IsNullOrEmpty(revisions[revisions.Count - 1].Delta))
            {
                if (!string.IsNullOrEmpty(text))
                {
                    throw new ArgumentException("A revision was incorrectly formatted", nameof(revisions));
                }
            }
            else if (DiffPatchMerge.Revision.TryParse(revisions[revisions.Count - 1].Delta!, out var r))
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
    public static string GetText(IEnumerable<Revision> revisions)
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
                text = revision.Delta ?? string.Empty;
            }
            else if (string.IsNullOrEmpty(revision.Delta))
            {
                if (!string.IsNullOrEmpty(text))
                {
                    throw new ArgumentException("A revision was incorrectly formatted", nameof(revisions));
                }
            }
            else if (DiffPatchMerge.Revision.TryParse(revision.Delta, out var r))
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

    /// <inheritdoc/>
    public bool Equals(Revision? other)
        => other is not null && Editor == other.Editor && TimestampTicks == other.TimestampTicks;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is Revision revision && Equals(revision);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Editor, TimestampTicks);

    /// <summary>
    /// Determine equality of two <see cref="Revision"/> instances.
    /// </summary>
    /// <param name="left">The first <see cref="Revision"/> instance.</param>
    /// <param name="right">The second <see cref="Revision"/> instance.</param>
    /// <returns>
    /// <see langword="true"/> if the instances are equal; otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Revision? left, Revision? right) => EqualityComparer<Revision>.Default.Equals(left, right);

    /// <summary>
    /// Determine inequality of two <see cref="Revision"/> instances.
    /// </summary>
    /// <param name="left">The first <see cref="Revision"/> instance.</param>
    /// <param name="right">The second <see cref="Revision"/> instance.</param>
    /// <returns>
    /// <see langword="true"/> if the instances are not equal; otherwise <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Revision? left, Revision? right) => !(left == right);
}
