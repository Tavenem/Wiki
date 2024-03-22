using SmartComponents.LocalEmbeddings;
using System.Diagnostics.CodeAnalysis;

namespace Tavenem.Wiki.Queries;

internal readonly struct PageSearchInfo(PageTitle title, EmbeddingI1 embedding, int line, string? owner, string? uploader) : IEquatable<PageSearchInfo>
{
    public EmbeddingI1 Embedding { get; } = embedding;

    public int Line { get; } = line;

    public string? Owner { get; } = owner;

    public PageTitle Title { get; } = title;

    public string? Uploader { get; } = uploader;

    public bool Equals(PageSearchInfo other) => Title.Equals(other.Title);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PageSearchInfo other && Equals(other);

    public override int GetHashCode() => Title.GetHashCode();
}
