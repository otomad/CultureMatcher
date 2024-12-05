using System.Globalization;

using CultureInfoMatcher;

CultureInfo[] availables = [new("en-US"), new("zh-CN"), new("zh-TW"), new("ja-JP"), new("ko-KR"), new("nb-NO"), new("vi-VN"), new("id-ID")];
CultureInfo def = new("en-US");

Console.WriteLine(CultureMatcher.Match(new CultureInfo("zh-HK"), availables, def));
Console.WriteLine(CultureMatcher.Match(new CultureInfo("fr-FR"), availables, def));
Console.WriteLine(CultureMatcher.Match(new CultureInfo("ms-MY"), availables, def));
Console.WriteLine(CultureMatcher.Match(new CultureInfo("da-DK"), availables, def));
Console.WriteLine(CultureMatcher.Match([new("fr-FR"), new("vi-VN"), new("ko-KR")], availables, def));
Console.WriteLine(CultureMatcher.Match([new("fr-XX"), new("en-GB")], [new("fr-FR"), new("en-US")], def));
