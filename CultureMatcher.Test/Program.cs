using System.Globalization;

using CultureInfoMatcher;

CultureInfo[] availables = [new("en-US"), new("zh-CN"), new("zh-TW"), new("ja-JP"), new("ko-KR"), new("ru-RU"), new("vi-VN"), new("id-ID")];
CultureInfo def = new("en-US");

Console.WriteLine(CultureMatcher.Match(new CultureInfo("zh-CN"), availables, def));
Console.WriteLine(CultureMatcher.Match(new CultureInfo("zh-TW"), availables, def));
Console.WriteLine(CultureMatcher.Match(new CultureInfo("zh-HK"), availables, def));
Console.WriteLine(CultureMatcher.Match(new CultureInfo("fr-FR"), availables, def));
Console.WriteLine(CultureMatcher.Match(new CultureInfo("ms-MY"), availables, def));
Console.WriteLine(CultureMatcher.Match(new CultureInfo("uk-UA"), availables, def));
