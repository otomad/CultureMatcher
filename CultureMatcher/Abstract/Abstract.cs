using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CultureInfoMatcher.Abstract;

internal static class Abstract {
	internal static CultureInfo? BestAvailableLocale(IList<CultureInfo> availableLocales, CultureInfo locale) {
		string candidate = locale.Name;
		while (true) {
			if (availableLocales.Select(culture => culture.Name).IndexOf(candidate) > -1) {
				return new(candidate);
			}
			int pos = candidate.LastIndexOf('-');

			if (pos == -1) {
				return null;
			}
			if (pos >= 2 && candidate[pos - 2] == '-') {
				pos -= 2;
			}
			candidate = candidate.Substring(0, pos);
		}
	}

	internal static LookupMatcherResult BestFitMatcher(IList<CultureInfo> availableLocales, IList<CultureInfo> requestedLocales, CultureInfo defaultLocale) {
		CultureInfo? foundLocale = null;
		string? extension = null;
		List<CultureInfo> noExtensionLocales = [];
		Dictionary<CultureInfo, CultureInfo> noExtensionLocaleMap = requestedLocales.Aggregate(
			new Dictionary<CultureInfo, CultureInfo>(),
			(all, l) => {
				CultureInfo noExtensionLocale = new(l.Name.Replace(Utils.UNICODE_EXTENSION_SEQUENCE_REGEX, ""));
				noExtensionLocales.Add(noExtensionLocale);
				all[noExtensionLocale] = l;
				return all;
			}
		);

		LocaleMatchingResult result = Utils.FindBestMatch(noExtensionLocales, availableLocales);
		if (result.matchedSupportedLocale is not null && result.matchedDesiredLocale is not null) {
			foundLocale = result.matchedSupportedLocale;
			extension = noExtensionLocaleMap[result.matchedDesiredLocale].Name.SubstringOrDefault(result.matchedDesiredLocale.Name.Length);
		}

		return new() {
			locale = foundLocale ?? defaultLocale,
			extension = extension,
		};
	}

	internal static string[] CanonicalizeLocaleList(IEnumerable<CultureInfo> cultures) {
		HashSet<CultureInfo> set = new(cultures);
		return set.Select(culture => culture.Name).ToArray();
	}

	internal static string CanonicalizeUValue(string? ukey, string uvalue) {
		// TODO: Implement algorithm for CanonicalizeUValue per https://tc39.es/ecma402/#sec-canonicalizeuvalue
		string lowerValue = uvalue.ToLowerInvariant();
		Utils.Invariant(ukey is not null, "ukey must be defined");
		string canonicalized = lowerValue;
		return canonicalized;
	}

	internal static LookupMatcherResult LookupMatcher(IList<CultureInfo> availableLocales, IList<CultureInfo> requestedLocales, CultureInfo defaultLocale) {
		LookupMatcherResult result = new();
		foreach (CultureInfo locale in requestedLocales) {
			CultureInfo noExtensionLocale = new(locale.Name.Replace(Utils.UNICODE_EXTENSION_SEQUENCE_REGEX, ""));
			CultureInfo? availableLocale = BestAvailableLocale(availableLocales, noExtensionLocale);
			if (availableLocale is not null) {
				result.locale = availableLocale;
				if (locale != noExtensionLocale) {
					result.extension = locale.Name.Substring(noExtensionLocale.Name.Length);
				}
				return result;
			}
		}
		result.locale = defaultLocale;
		return result;
	}

	public static IList<CultureInfo> LookupSupportedLocales(IList<CultureInfo> availableLocales, IList<CultureInfo> requestedLocales) {
		List<CultureInfo> subset = [];
		foreach (CultureInfo locale in requestedLocales) {
			CultureInfo noExtensionLocale = new(locale.Name.Replace(Utils.UNICODE_EXTENSION_SEQUENCE_REGEX, ""));
			CultureInfo? availableLocale = BestAvailableLocale(availableLocales, noExtensionLocale);
			if (availableLocale is not null) {
				subset.Add(availableLocale);
			}
		}
		return subset;
	}

	public static CultureInfo ResolveLocale(
		IList<CultureInfo> requestedLocales,
		IList<CultureInfo> availableLocales,
		MatcherAlgorithm matcher,
		CultureInfo defaultLocale
	) {
		LookupMatcherResult? r;
		if (matcher == MatcherAlgorithm.Lookup)
			r = LookupMatcher(availableLocales, requestedLocales, defaultLocale);
		else
			r = BestFitMatcher(availableLocales, requestedLocales, defaultLocale);
		if (r is null)
			r = new() { locale = defaultLocale, extension = "" };
		return r.Value.locale;
	}
}
