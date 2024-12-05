using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CultureInfoMatcher;

internal static class Extensions {
	/// <inheritdoc cref="string.Join(string, IEnumerable{string})"/>
	public static string Join(this IEnumerable<string> values, string separator) =>
		string.Join(separator, values);

	/// <inheritdoc cref="string.Join(string, IEnumerable{string})"/>
	public static string Join(this IEnumerable<string> values, char separator) =>
		string.Join(separator.ToString(), values);

	/// <inheritdoc cref="List{T}.IndexOf(T)"/>
	public static int IndexOf<T>(this List<T> list, T? item) where T : struct =>
		item is null ? -1 : list.IndexOf(item.Value);

	/// <inheritdoc cref="Array.IndexOf(Array, object)"/>
	public static int IndexOf<T>(this T[] array, T? value) =>
		value is null ? -1 : Array.IndexOf(array, value);

	/// <inheritdoc cref="List{T}.IndexOf(T)"/>
	public static int IndexOf<T>(this IEnumerable<T> list, T? item) =>
		item is null ? -1 : list.ToList().IndexOf(item);

	/// <inheritdoc cref="Regex.Replace(string, string, string)"/>
	public static string Replace(this string input, Regex pattern, string replacement) =>
		pattern.Replace(input, replacement);

	/// <inheritdoc cref="string.Substring(int)"/>
	public static string? SubstringOrDefault(this string str, int startIndex) =>
		startIndex >= 0 && startIndex < str.Length ? str.Substring(startIndex) : default;
}
