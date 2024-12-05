using System.Collections.Generic;
using System.Globalization;

using static CultureInfoMatcher.Abstract.Abstract;

namespace CultureInfoMatcher;

/// <summary>
/// Given a set of cultures an application has translations for and the set of cultures a user requests, find the best matching cultures.
/// </summary>
public static class CultureMatcher {
	/// <summary>
	/// Find the best matching culture info with a list of requested cultures.
	/// </summary>
	/// <param name="requestedCultures">List of culture info requested by the system or user.</param>
	/// <param name="availableCultures">List of culture info available for the application.</param>
	/// <param name="defaultCulture">The fallback general culture info matched when the languages are not mutually intelligible.</param>
	/// <param name="matcherAlgorithm">Matcher algorithm.</param>
	/// <returns>The best matching culture info.</returns>
	public static CultureInfo Match(
		IList<CultureInfo> requestedCultures,
		IList<CultureInfo> availableCultures,
		CultureInfo defaultCulture,
		MatcherAlgorithm matcherAlgorithm = MatcherAlgorithm.BestFit
	) {
		return ResolveLocale(
			requestedCultures,
			availableCultures,
			matcherAlgorithm,
			defaultCulture
		);
	}

	/// <summary>
	/// Find the best matching culture info for a single requested culture.
	/// </summary>
	/// <param name="requestedCulture">The culture info requested by the system or user.</param>
	/// <param name="availableCultures">List of culture info available for the application.</param>
	/// <param name="defaultCulture">The fallback general culture info matched when the languages are not mutually intelligible.</param>
	/// <param name="matcherAlgorithm">Matcher algorithm.</param>
	/// <returns>The best matching culture info.</returns>
	public static CultureInfo Match(
		CultureInfo requestedCulture,
		IList<CultureInfo> availableCultures,
		CultureInfo defaultCulture,
		MatcherAlgorithm matcherAlgorithm = MatcherAlgorithm.BestFit
	) =>
		Match([requestedCulture], availableCultures, defaultCulture, matcherAlgorithm);

	/// <summary>
	/// Lookup supported cultures.
	/// </summary>
	/// <param name="availableCultures">List of culture info available for the application.</param>
	/// <param name="requestedCultures">The culture info requested by the system or user.</param>
	/// <returns>Supported cultures.</returns>
	public static IList<CultureInfo> LookupSupportedCultures(IList<CultureInfo> availableCultures, IList<CultureInfo> requestedCultures) =>
		LookupSupportedLocales(availableCultures, requestedCultures);
}

/// <summary>
/// Matcher algorithm.
/// </summary>
public enum MatcherAlgorithm {
	/// <summary>
	/// <see cref="Lookup" /> would continue to be the existing <c>LookupMatcher</c> implementation within ECMA-402.
	/// </summary>
	Lookup,
	/// <summary>
	/// <see cref="BestFit" /> would be implementation-dependent.
	/// </summary>
	BestFit,
}
