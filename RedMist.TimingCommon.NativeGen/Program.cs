using RedMist.TimingCommon.NativeGen;

// Usage: dotnet run --project RedMist.TimingCommon.NativeGen -- <output-dir> [kotlin-package]
// Writes <output-dir>/kotlin, <output-dir>/swift and <output-dir>/fixtures.
if (args.Length < 1)
{
    Console.Error.WriteLine("usage: NativeGen <output-dir> [kotlin-package]");
    return 1;
}

var output = Path.GetFullPath(args[0]);
var kotlinPackage = args.Length > 1 ? args[1] : "com.bigmissionmotorsports.redmist.core.model";

var catalog = TypeCatalog.Build();
new KotlinEmitter(catalog, kotlinPackage).Write(Path.Combine(output, "kotlin"));
new SwiftEmitter(catalog).Write(Path.Combine(output, "swift"));
new FixtureWriter(catalog).Write(Path.Combine(output, "fixtures"));

Console.WriteLine($"{catalog.Classes.Count} classes, {catalog.Enums.Count} enums -> {output}");
foreach (var c in catalog.Classes)
    Console.WriteLine($"  {c.Name}{(c.IsKeyed ? "" : " (map)")}{(c.PatchOf != null ? $" patches {c.PatchOf.Name}" : "")}: {c.Props.Count} members");
return 0;
