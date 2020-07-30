using NeverFoundry.DataStorage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeverFoundry.Wiki.Converters
{
    /// <summary>
    /// Converts an <see cref="Article"/> to or from JSON.
    /// </summary>
    public class ArticleConverter : JsonConverter<Article>
    {
        /// <summary>Reads and converts the JSON to an <see cref="Article"/>.</summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        [return: MaybeNull]
        public override Article Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var initialReader = reader;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(IIdItem.IdItemTypeName)
                || !reader.Read()
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            var idItemTypeName = reader.GetString();
            if (string.IsNullOrEmpty(idItemTypeName))
            {
                throw new JsonException();
            }

            if (string.Equals(idItemTypeName, Category.CategoryIdItemTypeName))
            {
                var category = JsonSerializer.Deserialize<Category>(ref initialReader, options);
                reader = initialReader;
                return category;
            }

            if (string.Equals(idItemTypeName, WikiFile.WikiFileIdItemTypeName))
            {
                var file = JsonSerializer.Deserialize<WikiFile>(ref initialReader, options);
                reader = initialReader;
                return file;
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != "id"
                || !reader.Read()
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            var id = reader.GetString();
            if (id is null)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.Title)
                || !reader.Read()
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            var title = reader.GetString();
            if (title is null)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.MarkdownContent)
                || !reader.Read()
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            var markdownContent = reader.GetString();
            if (markdownContent is null)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.WikiLinks)
                || !reader.Read()
                || reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }
            var wikiLinks = JsonSerializer.Deserialize<IReadOnlyCollection<WikiLink>>(ref reader, options);
            if (wikiLinks is null
                || reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.TimestampTicks)
                || !reader.Read()
                || reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }
            var timestampTicks = reader.GetInt64();

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.WikiNamespace)
                || !reader.Read()
                || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            var wikiNamespace = reader.GetString();
            if (wikiNamespace is null)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.IsDeleted)
                || !reader.Read())
            {
                throw new JsonException();
            }
            var isDeleted = reader.GetBoolean();

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.Owner)
                || !reader.Read())
            {
                throw new JsonException();
            }
            var owner = reader.GetString();

            IReadOnlyCollection<string>? allowedEditors;
            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.AllowedEditors)
                || !reader.Read())
            {
                throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.Null)
            {
                allowedEditors = null;
            }
            else if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }
            else
            {
                allowedEditors = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(ref reader, options);
                if (allowedEditors is null
                    || reader.TokenType != JsonTokenType.EndArray)
                {
                    throw new JsonException();
                }
            }

            IReadOnlyCollection<string>? allowedViewers;
            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.AllowedViewers)
                || !reader.Read())
            {
                throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.Null)
            {
                allowedViewers = null;
            }
            else if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }
            else
            {
                allowedViewers = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(ref reader, options);
                if (allowedViewers is null
                    || reader.TokenType != JsonTokenType.EndArray)
                {
                    throw new JsonException();
                }
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.RedirectNamespace)
                || !reader.Read())
            {
                throw new JsonException();
            }
            var redirectNamespace = reader.GetString();

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.RedirectTitle)
                || !reader.Read())
            {
                throw new JsonException();
            }
            var redirectTitle = reader.GetString();

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.IsBrokenRedirect)
                || !reader.Read())
            {
                throw new JsonException();
            }
            var isBrokenRedirect = reader.GetBoolean();

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.IsDoubleRedirect)
                || !reader.Read())
            {
                throw new JsonException();
            }
            var isDoubleRedirect = reader.GetBoolean();

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.Categories)
                || !reader.Read()
                || reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }
            var categories = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(ref reader, options);
            if (categories is null
                || reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException();
            }

            IReadOnlyList<Transclusion>? transclusions;
            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != nameof(Article.Transclusions)
                || !reader.Read())
            {
                throw new JsonException();
            }
            if (reader.TokenType == JsonTokenType.Null)
            {
                transclusions = null;
            }
            else if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }
            else
            {
                transclusions = JsonSerializer.Deserialize<IReadOnlyList<Transclusion>>(ref reader, options);
                if (transclusions is null
                    || reader.TokenType != JsonTokenType.EndArray)
                {
                    throw new JsonException();
                }
            }

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                reader.Read();
            }

            return new Article(
                id,
                idItemTypeName,
                title,
                markdownContent,
                wikiLinks,
                timestampTicks,
                wikiNamespace,
                isDeleted,
                owner,
                allowedEditors,
                allowedViewers,
                redirectNamespace,
                redirectTitle,
                isBrokenRedirect,
                isDoubleRedirect,
                categories,
                transclusions);
        }

        /// <summary>Writes a specified value as JSON.</summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, Article value, JsonSerializerOptions options)
        {
            if (value is Category category)
            {
                JsonSerializer.Serialize(writer, category, typeof(Category), options);
                return;
            }
            if (value is WikiFile file)
            {
                JsonSerializer.Serialize(writer, file, typeof(WikiFile), options);
                return;
            }

            writer.WriteStartObject();

            writer.WriteString(nameof(IIdItem.IdItemTypeName), value.IdItemTypeName);
            writer.WriteString("id", value.Id);
            writer.WriteString(nameof(Article.Title), value.Title);
            writer.WriteString(nameof(Article.MarkdownContent), value.MarkdownContent);

            writer.WritePropertyName(nameof(Article.WikiLinks));
            JsonSerializer.Serialize(writer, value.WikiLinks, options);

            writer.WriteNumber(nameof(Article.TimestampTicks), value.TimestampTicks);
            writer.WriteString(nameof(Article.WikiNamespace), value.WikiNamespace);
            writer.WriteBoolean(nameof(Article.IsDeleted), value.IsDeleted);

            if (string.IsNullOrEmpty(value.Owner))
            {
                writer.WriteNull(nameof(Article.Owner));
            }
            else
            {
                writer.WriteString(nameof(Article.Owner), value.Owner);
            }

            if (value.AllowedEditors is null)
            {
                writer.WriteNull(nameof(Article.AllowedEditors));
            }
            else
            {
                writer.WritePropertyName(nameof(Article.AllowedEditors));
                JsonSerializer.Serialize(writer, value.AllowedEditors, options);
            }

            if (value.AllowedViewers is null)
            {
                writer.WriteNull(nameof(Article.AllowedViewers));
            }
            else
            {
                writer.WritePropertyName(nameof(Article.AllowedViewers));
                JsonSerializer.Serialize(writer, value.AllowedViewers, options);
            }

            if (string.IsNullOrEmpty(value.RedirectNamespace))
            {
                writer.WriteNull(nameof(Article.RedirectNamespace));
            }
            else
            {
                writer.WriteString(nameof(Article.RedirectNamespace), value.RedirectNamespace);
            }

            if (string.IsNullOrEmpty(value.RedirectTitle))
            {
                writer.WriteNull(nameof(Article.RedirectTitle));
            }
            else
            {
                writer.WriteString(nameof(Article.RedirectTitle), value.RedirectTitle);
            }

            writer.WriteBoolean(nameof(Article.IsBrokenRedirect), value.IsBrokenRedirect);
            writer.WriteBoolean(nameof(Article.IsDoubleRedirect), value.IsDoubleRedirect);

            writer.WritePropertyName(nameof(Article.Categories));
            JsonSerializer.Serialize(writer, value.Categories, options);

            if (value.Transclusions is null)
            {
                writer.WriteNull(nameof(Article.Transclusions));
            }
            else
            {
                writer.WritePropertyName(nameof(Article.Transclusions));
                JsonSerializer.Serialize(writer, value.Transclusions, options);
            }

            writer.WriteEndObject();
        }
    }
}
