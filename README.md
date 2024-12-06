# CultureMatcher

[![NuGet](https://img.shields.io/nuget/v/CultureMatcher?logo=nuget&label=NuGet&color=%23004880)](https://www.nuget.org/packages/CultureMatcher)
[![GitHub](https://img.shields.io/nuget/vpre/CultureMatcher?logo=github&label=GitHub&color=%23181717)](https://github.com/otomad/CultureMatcher)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)][license-url]

[license-url]: http://opensource.org/licenses/MIT

Given a set of cultures an application has translations for and the set of cultures a user requests, find the best matching cultures.

This is the implementation of [LocaleMatcher](https://github.com/tc39/proposal-intl-localematcher) in C# version, and the source code is directly translated from [FormatJS](https://formatjs.io/docs/polyfills/intl-localematcher).

## Usage

```csharp
using System.Globalization;
using CultureInfoMatcher;

CultureMatcher.Match(
    new CultureInfo[] { new CultureInfo("fr-XX"), new CultureInfo("en-GB") },
    new CultureInfo[] { new CultureInfo("fr-FR"), new CultureInfo("en-US") },
    new CultureInfo("en-US")
); // fr-FR
```

Now declare the languages for which the application has translated, and the default language.

```csharp
CultureInfo[] availableCultures = {
    new CultureInfo("en-US"),
    new CultureInfo("zh-CN"),
    new CultureInfo("zh-TW"),
    new CultureInfo("ja-JP"),
    new CultureInfo("ko-KR"),
    new CultureInfo("no-NO"),
    new CultureInfo("vi-VN"),
    new CultureInfo("id-ID"),
};
CultureInfo defaultCulture = new CultureInfo("en-US");
```

```csharp
CultureMatcher.Match(new CultureInfo("zh-HK"), availableCultures, defaultCulture); // zh-TW
```

zh-HK uses traditional Chinese, so it matches zh-TW, even though zh-CN is in front.

```csharp
CultureMatcher.Match(new CultureInfo("fr-FR"), availableCultures, defaultCulture); // en-US
```

French is not in the list, so English is matched.

```csharp
CultureMatcher.Match(new CultureInfo("ms-MY"), availableCultures, defaultCulture); // id-ID
```

Malay and Indonesian are mutually intelligible.

```csharp
CultureMatcher.Match(new CultureInfo("da-DK"), availableCultures, defaultCulture); // no-NO
```

Danish and Norwegian are mutually intelligible.

```csharp
CultureMatcher.Match(new CultureInfo[] {
    new CultureInfo("fr-FR"),
    new CultureInfo("vi-VN"),
    new CultureInfo("ko-KR"),
}, availableCultures, defaultCulture); // vi-VN
```

If someone is proficient in French, fluent in Vietnamese, and knows Korean, even though the list includes both Korean and Vietnamese, it matches Vietnamese which has a higher level of understanding.

## API

Find the best matching culture info with a list of requested cultures.

```csharp
public static CultureInfo Match(
    IList<CultureInfo> requestedCultures,
    IList<CultureInfo> availableCultures,
    CultureInfo defaultCulture,
    MatcherAlgorithm matcherAlgorithm = MatcherAlgorithm.BestFit
);
```

Find the best matching culture info for a single requested culture.

```csharp
public static CultureInfo Match(
    CultureInfo requestedCulture,
    IList<CultureInfo> availableCultures,
    CultureInfo defaultCulture,
    MatcherAlgorithm matcherAlgorithm = MatcherAlgorithm.BestFit
);
```

### Options

#### Matcher Algorithm

- `Lookup` would continue to be the existing `LookupMatcher` implementation within ECMA-402.
- `BestFit` would be implementation-dependent.

## Licence

CultureMatcher is available under the [MIT License][license-url]. See the LICENSE file for more info.

## References
- [proposal-intl-localematcher](https://github.com/tc39/proposal-intl-localematcher)
- [@formatjs/intl-localematcher](https://formatjs.io/docs/polyfills/intl-localematcher)
- [Mutual intelligibility](https://en.wikipedia.org/wiki/Mutual_intelligibility)
