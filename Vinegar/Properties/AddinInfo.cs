using Mono.Addins;
using Mono.Addins.Description;


[assembly: Addin(
"Vinegar",
    Namespace = "Vinegar",
    Version = VersionInfo.Version
)]

[assembly: AddinName("Vinegar")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinUrl("https://whatever")] //
[assembly: AddinDescription("Vim Vinegar clone for VSMac")]
[assembly: AddinAuthor("Jason Imison")]

public static class VersionInfo
{
    public const string Version = "0.0.0.19";
}
