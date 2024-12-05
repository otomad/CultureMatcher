using System.Collections.Generic;
using System.Globalization;

namespace CultureInfoMatcher;

public static class CultureMatcher {
	public static CultureInfo Match(
		IList<CultureInfo> requestedCultures,
		IList<CultureInfo> availableCultures,
		CultureInfo defaultCulture,
		MatcherAlgorithm matcherAlgorithm = MatcherAlgorithm.BestFit
	) {
		return Abstract.Abstract.ResolveLocale(
			requestedCultures,
			availableCultures,
			matcherAlgorithm,
			defaultCulture
		);
	}

	public static CultureInfo Match(
		CultureInfo requestedCulture,
		IList<CultureInfo> availableCultures,
		CultureInfo defaultCulture,
		MatcherAlgorithm matcherAlgorithm = MatcherAlgorithm.BestFit
	) =>
		Match([requestedCulture], availableCultures, defaultCulture, matcherAlgorithm);

	public static IList<CultureInfo> LookupSupportedLocales(IList<CultureInfo> availableLocales, IList<CultureInfo> requestedLocales) =>
		Abstract.Abstract.LookupSupportedLocales(availableLocales, requestedLocales);
}

public enum MatcherAlgorithm {
	Lookup,
	BestFit,
}
