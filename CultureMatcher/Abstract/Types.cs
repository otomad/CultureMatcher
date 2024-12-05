using System.Globalization;

namespace CultureInfoMatcher.Abstract;

internal struct LookupMatcherResult {
	internal CultureInfo locale;
	internal string? extension;
	internal string? nu;
}

internal struct Keyword {
	internal string key;
	internal string value;
}
