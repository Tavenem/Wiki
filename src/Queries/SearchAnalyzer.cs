using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.En;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Util;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// Builds an analyzer with the given stop words.
/// </summary>
/// <param name="matchVersion">Lucene version to match - See <see cref="SearchAnalyzer"/></param>
/// <param name="stopWords">stop words</param>
internal class SearchAnalyzer(LuceneVersion matchVersion, CharArraySet stopWords)
    : StopwordAnalyzerBase(matchVersion, stopWords)
{
    /// <summary>
    /// Default maximum allowed token length </summary>
    public const int DEFAULT_MAX_TOKEN_LENGTH = StandardAnalyzer.DEFAULT_MAX_TOKEN_LENGTH;

    /// <summary>
    /// An unmodifiable set containing some common English words that are usually not
    /// useful for searching.
    /// </summary>
    public static readonly CharArraySet STOP_WORDS_SET = StopAnalyzer.ENGLISH_STOP_WORDS_SET;

    /// <summary>
    /// Set maximum allowed token length.  If a token is seen that exceeds this length then it is
    /// discarded. This setting only takes effect the next time tokenStream or tokenStream is
    /// called.
    /// </summary>
    public int MaxTokenLength { set; get; } = DEFAULT_MAX_TOKEN_LENGTH;

    /// <summary>
    /// Builds an analyzer with the default stop words (<see cref="STOP_WORDS_SET"/>.
    /// </summary>
    /// <param name="matchVersion">Lucene version to match - See <see cref="SearchAnalyzer"/></param>
    public SearchAnalyzer(LuceneVersion matchVersion)
        : this(matchVersion, STOP_WORDS_SET) { }

    /// <summary>
    /// Builds an analyzer with the stop words from the given reader.
    /// </summary>
    /// <seealso cref="WordlistLoader.GetWordSet(TextReader, LuceneVersion)"/>
    /// <param name="matchVersion">Lucene version to match - See <see cref="SearchAnalyzer"/></param>
    /// <param name="stopwords"><see cref="TextReader"/> to read stop words from</param>
    public SearchAnalyzer(LuceneVersion matchVersion, TextReader stopwords)
        : this(matchVersion, LoadStopwordSet(stopwords, matchVersion)) { }

    protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
    {
        var src = new UAX29URLEmailTokenizer(m_matchVersion, reader)
        {
            MaxTokenLength = MaxTokenLength
        };
        TokenStream tok = new StandardFilter(m_matchVersion, src);
        tok = new WordDelimiterFilter(
            m_matchVersion,
            tok,
            WordDelimiterFlags.GENERATE_WORD_PARTS
                | WordDelimiterFlags.CATENATE_WORDS
                | WordDelimiterFlags.CATENATE_NUMBERS
                | WordDelimiterFlags.CATENATE_ALL
                | WordDelimiterFlags.PRESERVE_ORIGINAL
                | WordDelimiterFlags.SPLIT_ON_CASE_CHANGE
                | WordDelimiterFlags.SPLIT_ON_NUMERICS
                | WordDelimiterFlags.STEM_ENGLISH_POSSESSIVE,
            null);
        tok = new LowerCaseFilter(m_matchVersion, tok);
        tok = new StopFilter(m_matchVersion, tok, m_stopwords);
        tok = new PorterStemFilter(tok);
        return new TokenStreamComponentsAnonymousClass(this, src, tok);
    }

    private class TokenStreamComponentsAnonymousClass(
        SearchAnalyzer outerInstance,
        UAX29URLEmailTokenizer src,
        TokenStream tok)
        : TokenStreamComponents(src, tok)
    {
        protected override void SetReader(TextReader reader)
        {
            src.MaxTokenLength = outerInstance.MaxTokenLength;
            base.SetReader(reader);
        }
    }
}
