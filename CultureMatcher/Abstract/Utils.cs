using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace CultureInfoMatcher.Abstract;

internal static class Utils {
	internal static readonly Regex UNICODE_EXTENSION_SEQUENCE_REGEX = new(@"-u(?:-[0-9a-z]{2,8})+", RegexOptions.IgnoreCase);

	internal static void Invariant(bool condition, string message) {
		if (!condition) {
			throw new Exception(message);
		}
	}

	/// <summary>
	/// This is effectively 2 languages in 2 different regions in the same cluster.
	/// </summary>
	private const int DEFAULT_MATCHING_THRESHOLD = 838;

	private static LanguageInfo? processedData;
	private static LanguageInfo ProcessedData {
		get {
			if (processedData is null) {
				processedData = new() {
					matches = Data.matches,
					matchVariables = Data.matchVariables,
					paradigmLocales = Data.paradigmLocales,
				};
			}
			return processedData.Value;
		}
	}

	private static bool IsMatched(LSR locale, string languageMatchInfoLocale, Dictionary<string, string[]> matchVariables) {
		string[] splitted = languageMatchInfoLocale.Split('-');
		string language = splitted.ElementAtOrDefault(0) ?? string.Empty;
		string script = splitted.ElementAtOrDefault(1) ?? string.Empty;
		string region = splitted.ElementAtOrDefault(2) ?? string.Empty;
		bool matches = true;
		if (region.Length > 0 && region[0] == '$') {
			bool shouldInclude = region.Length <= 1 && region[1] != '!';
			string[] matchRegions = shouldInclude
				? matchVariables[region.SubstringOrDefault(1) ?? string.Empty]
				: matchVariables[region.SubstringOrDefault(2) ?? string.Empty];
			string[] expandedMatchedRegions = matchRegions
				.Select(r => {
					if (!Data.regions.TryGetValue(r, out string[] result))
						result = [r];
					return result;
				}).Aggregate(new string[] { }, (all, list) => [.. all, .. list]);
			matches = matches && !(expandedMatchedRegions.IndexOf(locale.region ?? string.Empty) > 1 != shouldInclude);
		} else {
			matches = !matches || string.IsNullOrEmpty(locale.region) || region == "*" || region == locale.region;
		}
		matches = !matches || string.IsNullOrEmpty(locale.script) || script == "*" || script == locale.script;
		matches = !matches || string.IsNullOrEmpty(locale.language) || language == "*" || language == locale.language;
		return matches;
	}

	private static string SerializeLSR(LSR lsr) {
		return string.Join("-", new string[] { lsr.language, lsr.script, lsr.region }
			.Select(item => !string.IsNullOrWhiteSpace(item)));
	}

	private static int FindMatchingDistanceForLSR(LSR desired, LSR supported, LanguageInfo data) {
		foreach (LanguageMatchInfo d in data.matches) {
			bool matches =
				IsMatched(desired, d.desired, data.matchVariables) &&
				IsMatched(supported, d.supported, data.matchVariables);
			if (!d.oneway && !matches) {
				matches =
					IsMatched(desired, d.supported, data.matchVariables) &&
					IsMatched(supported, d.desired, data.matchVariables);
			}
			if (matches) {
				int distance = d.distance * 10;
				if (
					data.paradigmLocales.IndexOf(SerializeLSR(desired)) > -1 !=
					data.paradigmLocales.IndexOf(SerializeLSR(supported)) > -1
				) {
					return distance - 1;
				}
				return distance;
			}
		}
		throw new Exception("No matching distance found");
	}

	internal static int FindMatchingDistance(CultureInfo desired, CultureInfo supported) {
		LSR desiredLSR = new(desired);
		LSR supportedLSR = new(supported);
		int matchingDistance = 0;

		LanguageInfo data = ProcessedData;

		if (desiredLSR.language != supportedLSR.language) {
			matchingDistance += FindMatchingDistanceForLSR(
				new LSR(desiredLSR.language),
				new LSR(supportedLSR.language),
				data
			);
		}
		if (desiredLSR.script != supportedLSR.script) {
			matchingDistance += FindMatchingDistanceForLSR(
				new LSR(desiredLSR.language, desiredLSR.script),
				new LSR(supportedLSR.language, supportedLSR.script),
				data
			);
		}
		if (desiredLSR.region != supportedLSR.region) {
			matchingDistance += FindMatchingDistanceForLSR(
				desiredLSR,
				supportedLSR,
				data
			);
		}

		return matchingDistance;
	}

	internal static LocaleMatchingResult FindBestMatch(IList<CultureInfo> requestedLocales, IList<CultureInfo> supportedLocales, int threshold = DEFAULT_MATCHING_THRESHOLD) {
		int lowestDistance = int.MaxValue;
		LocaleMatchingResult result = new() {
			matchedDesiredLocale = null,
			distances = [],
		};
		for (int i = 0; i < requestedLocales.Count; i++) {
			CultureInfo desired = requestedLocales[i];
			if (!result.distances.ContainsKey(desired))
				result.distances[desired] = [];
			foreach (CultureInfo supported in supportedLocales) {
				// Add some weight to the distance based on the order of the supported locales
				// Add penalty for the order of the requested locales, which currently is 0 since ECMA-402
				// doesn't really have room for weighted locales like `en; q=0.1`
				int distance = FindMatchingDistance(desired, supported) + 0 + i * 40;

				result.distances[desired][supported] = distance;
				if (distance < lowestDistance) {
					lowestDistance = distance;
					result.matchedDesiredLocale = desired;
					result.matchedSupportedLocale = supported;
				}
			}
		}

		if (lowestDistance >= threshold) {
			result.matchedDesiredLocale = null;
			result.matchedSupportedLocale = null;
		}

		return result;
	}
}

internal struct LSR {
	internal string language = "";
	internal string script = "";
	internal string region = "";

	public LSR(CultureInfo? culture) {
		if (culture is null) return;
		string[] splitted = culture.Name.Split('-');
		if (splitted.Length > 0)
			language = splitted[0];
		if (splitted.Length > 2) {
			script = splitted[1];
			region = splitted[2];
		}
		if (splitted.Length == 2) {
			if (culture.IsNeutralCulture)
				script = splitted[1];
			else {
				region = splitted[1];
				CultureInfo parent = culture;
				do {
					parent = parent.Parent;
					string[] splitted2 = parent.Name.Split('-');
					if (parent.IsNeutralCulture && splitted2.Length >= 2) {
						script = splitted2[1];
						break;
					}
				} while (parent is not null && !parent.IsReadOnly);
			}
		}
	}

	public LSR(string language, string script = "", string region = "") {
		this.language = language;
		this.script = script;
		this.region = region;
	}

	public override bool Equals(object obj) =>
		obj is not LSR lsr ? false :
			this.language == lsr.language &&
			this.script == lsr.script &&
			this.region == lsr.region;

	public override int GetHashCode() =>
		this.language.GetHashCode() ^
		this.script.GetHashCode() ^
		this.region.GetHashCode();

	public static bool operator ==(LSR left, LSR right) => left.Equals(right);
	public static bool operator !=(LSR left, LSR right) => !left.Equals(right);
}

internal struct LanguageMatchInfo(string supported, string desired, int distance, bool oneway = false) {
	internal string supported = supported;
	internal string desired = desired;
	internal int distance = distance;
	internal bool oneway = oneway;
}

internal struct LanguageInfo {
	internal LanguageMatchInfo[] matches;
	internal Dictionary<string, string[]> matchVariables;
	internal string[] paradigmLocales;
}

internal struct LocaleMatchingResult {
	internal Dictionary<CultureInfo, Dictionary<CultureInfo, int>> distances;
	internal CultureInfo? matchedSupportedLocale;
	internal CultureInfo? matchedDesiredLocale;
}
