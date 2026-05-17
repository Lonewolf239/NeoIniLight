using NeoIniLight;
using NeoIniLight.Annotations;
using NeoIniLight.Models;

public class NeoIniKeyTest
{
    [NeoIniKey("User", "Age")]
    public int Age { get; set; }

    [NeoIniKey("User", "IsAdmin")]
    public bool IsAdmin { get; set; }
}

class NeoIniLightDemo
{
    private const string TestFile = "demo_config.ini";

    private static async Task Main()
    {
        Console.CursorVisible = false;
        Console.Clear();
        Console.WriteLine("NeoIniLight v1.0 Demonstration\n");

        BasicCreationDemo();
        SectionsDemo();
        KeysValuesDemo();
        ClampAndAutoAddDemo();
        SearchAndRenameDemo();
        OptionsAndPresetsDemo();
        await AsyncOperationsDemo();
        AutoFeaturesDemo();
        EventsDemo();
        PerformanceModeDemo();
        AttributeAndGeneratorDemo();

        Console.Write("Press Y to cleanup file: ");
        if (Console.ReadKey().Key == ConsoleKey.Y)
        {
            Console.WriteLine();
            CleanupDemo();
        }

        Console.WriteLine("\n\nDemonstration completed!");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        Console.Clear();
    }

    private static NeoIniDocument CreateDocument() => new NeoIniDocument(TestFile);

    private static NeoIniDocument CreateDocumentWithOptions(NeoIniOptions options) => new NeoIniDocument(TestFile, options);

    private static async Task<NeoIniDocument> CreateDocumentAsync(NeoIniOptions options = null)
    {
        options ??= new NeoIniOptions();
        return await NeoIniDocument.CreateAsync(TestFile, options);
    }

    private static void BasicCreationDemo()
    {
        Console.Clear();
        Console.WriteLine("1. FILE CREATION WITH DEFAULTS");
        using var ini = CreateDocument();

        Console.WriteLine($"File created: {TestFile}");
        Console.WriteLine("All features use default settings:");
        Console.WriteLine($"- AutoAdd: {ini.UseAutoAdd}");
        Console.WriteLine($"- AutoSave: {ini.UseAutoSave}");
        Console.WriteLine($"- AutoBackup: {ini.UseAutoBackup}");
        Console.WriteLine($"- AutoSaveInterval: {ini.AutoSaveInterval}");
        Console.WriteLine($"- SaveOnDispose: {ini.SaveOnDispose}");
        Console.WriteLine($"- AllowEmptyValues: {ini.AllowEmptyValues}");

        Console.WriteLine("\nCurrent raw INI view:");
        Console.WriteLine(ini.ToString());

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void SectionsDemo()
    {
        Console.Clear();
        Console.WriteLine("2. WORKING WITH SECTIONS");
        using var ini = CreateDocument();

        Console.WriteLine($"Section 'User' exists: {ini.SectionExists("User")}");

        ini.AddSection("User");
        ini.AddSection("Database");
        Console.WriteLine("Added sections: User, Database");

        var sections = ini.GetAllSections();
        Console.WriteLine($"All sections ({sections.Length}): {string.Join(", ", sections)}");

        ini.RenameSection("Database", "DbConfig");
        Console.WriteLine("Section 'Database' renamed to 'DbConfig'");

        sections = ini.GetAllSections();
        Console.WriteLine($"Sections after rename ({sections.Length}): {string.Join(", ", sections)}");

        ini.RemoveSection("DbConfig");
        Console.WriteLine("Section 'DbConfig' removed");

        sections = ini.GetAllSections();
        Console.WriteLine($"Remaining sections ({sections.Length}): {string.Join(", ", sections)}");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void KeysValuesDemo()
    {
        Console.Clear();
        Console.WriteLine("3. KEYS AND VALUES");
        using var ini = CreateDocument();

        ini.AddKey("User", "Name", "John Doe");
        ini.AddKey("User", "Age", 30);
        ini.AddKey("User", "IsAdmin", true);
        ini.AddKey("User", "Salary", 75000.50);
        ini.AddKey("User", "Bio", "Line1\r\nLine2\r\nLine3");
        Console.WriteLine("Added keys to User section (including multiline Bio)");

        string name = ini.GetValue("User", "Name", "Unknown");
        int age = ini.GetValue("User", "Age", 0);
        bool isAdmin = ini.GetValue("User", "IsAdmin", false);
        double salary = ini.GetValue("User", "Salary", 0.0);
        string bio = ini.GetValue("User", "Bio", string.Empty);

        Console.WriteLine($"Name: {name}");
        Console.WriteLine($"Age: {age}");
        Console.WriteLine($"Admin: {isAdmin}");
        Console.WriteLine($"Salary: {salary}");
        Console.WriteLine("Bio (with line breaks preserved):");
        Console.WriteLine(bio);

        ini.SetValue("User", "Age", 31);
        Console.WriteLine("Age updated to 31");

        Console.WriteLine($"Key 'Name' exists: {ini.KeyExists("User", "Name")}");
        Console.WriteLine($"Key 'Email' exists: {ini.KeyExists("User", "Email")}");

        string email = ini.GetValue("User", "Email", "no@email.com");
        Console.WriteLine($"Email (auto-added if AutoAdd enabled): {email}");

        var keys = ini.GetAllKeys("User");
        Console.WriteLine($"Keys in User ({keys.Length}): {string.Join(", ", keys)}");

        ini.RenameKey("User", "Name", "FullName");
        Console.WriteLine("Key 'Name' renamed to 'FullName'");

        keys = ini.GetAllKeys("User");
        Console.WriteLine($"Keys in User after rename ({keys.Length}): {string.Join(", ", keys)}");

        ini.RemoveKey("User", "Salary");
        Console.WriteLine("Key 'Salary' removed");

        keys = ini.GetAllKeys("User");
        Console.WriteLine($"Keys in User after removal ({keys.Length}): {string.Join(", ", keys)}");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void ClampAndAutoAddDemo()
    {
        Console.Clear();
        Console.WriteLine("3.1. CLAMP & AUTO-ADD BEHAVIOR");
        using var ini = CreateDocument();

        Console.WriteLine("Writing values out of allowed range:");
        ini.SetValueClamped("ClampDemo", "Volume", 0, 100, 200);
        ini.SetValueClamped("ClampDemo", "Brightness", 0, 100, -10);

        int clampedVolume = ini.GetValue("ClampDemo", "Volume", 50);
        int clampedBrightness = ini.GetValue("ClampDemo", "Brightness", 50);
        Console.WriteLine($"Clamped Volume [0..100]: {clampedVolume}");
        Console.WriteLine($"Clamped Brightness [0..100]: {clampedBrightness}");

        Console.WriteLine("\nDemonstrating AutoAdd = false (Safe preset):");
        using var safeIni = CreateDocumentWithOptions(NeoIniOptions.Safe);
        Console.WriteLine($"Safe.AutoAdd: {safeIni.UseAutoAdd}");
        int missingValue = safeIni.GetValue("NonExisting", "Key", 123);
        Console.WriteLine($"GetValue on missing key (AutoAdd=false): {missingValue}");
        Console.WriteLine($"SectionExists('NonExisting'): {safeIni.SectionExists("NonExisting")}");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void SearchAndRenameDemo()
    {
        Console.Clear();
        Console.WriteLine("3.2. SEARCH & RENAME DEMO");
        using var ini = CreateDocument();

        ini.SetValue("SearchDemo", "Path", @"C:\Games\MyGame");
        ini.SetValue("SearchDemo", "Mode", "Debug");
        ini.SetValue("SearchDemo", "Description", "Game config for debug mode");

        var results = ini.Search("game");
        Console.WriteLine($"Search for 'game' found {results.Count} entries:");
        foreach (var result in results)
            Console.WriteLine($"  [{result.Section}] {result.Key} = {result.Value}");

        Console.WriteLine("\nFind key 'Mode' in all sections:");
        var found = ini.FindKey("Mode");
        foreach (var kv in found)
            Console.WriteLine($"  Section [{kv.Key}] -> Mode = {kv.Value}");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void OptionsAndPresetsDemo()
    {
        Console.Clear();
        Console.WriteLine("4. OPTIONS & PRESETS");

        using (var def = CreateDocumentWithOptions(NeoIniOptions.Default))
        {
            Console.WriteLine("Default options:");
            PrintOptions(def);
        }

        using (var safe = CreateDocumentWithOptions(NeoIniOptions.Safe))
        {
            Console.WriteLine("\nSafe options:");
            PrintOptions(safe);
        }

        using (var perf = CreateDocumentWithOptions(NeoIniOptions.Performance))
        {
            Console.WriteLine("\nPerformance options:");
            PrintOptions(perf);
        }

        using (var buffered = CreateDocumentWithOptions(NeoIniOptions.BufferedAutoSave(5)))
        {
            Console.WriteLine("\nBufferedAutoSave(5) options:");
            PrintOptions(buffered);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void PrintOptions(NeoIniDocument ini)
    {
        Console.WriteLine($"- AutoAdd: {ini.UseAutoAdd}");
        Console.WriteLine($"- AutoSave: {ini.UseAutoSave}");
        Console.WriteLine($"- AutoBackup: {ini.UseAutoBackup}");
        Console.WriteLine($"- AutoSaveInterval: {ini.AutoSaveInterval}");
        Console.WriteLine($"- SaveOnDispose: {ini.SaveOnDispose}");
        Console.WriteLine($"- AllowEmptyValues: {ini.AllowEmptyValues}");
    }

    private static async Task AsyncOperationsDemo()
    {
        Console.Clear();
        Console.WriteLine("5. ASYNCHRONOUS OPERATIONS");
        using var ini = await CreateDocumentAsync();

        await ini.AddSectionAsync("Settings");
        await ini.SetValueAsync("Settings", "Theme", "Dark");
        await ini.SetValueAsync("Settings", "Volume", 80);
        Console.WriteLine("Settings section added asynchronously");

        string theme = await ini.GetValueAsync("Settings", "Theme", "Light");
        int volume = await ini.GetValueAsync("Settings", "Volume", 50);
        Console.WriteLine($"Theme: {theme}, Volume: {volume}%");

        int clampedVolume = await ini.GetValueClampedAsync("Settings", "Volume", 0, 100, 50);
        Console.WriteLine($"Clamped Volume [0..100]: {clampedVolume}");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void AutoFeaturesDemo()
    {
        Console.Clear();
        Console.WriteLine("6. AUTOMATIC FEATURES");
        using var ini = CreateDocument();

        ini.SetValue("Database", "Host", "localhost");
        ini.SetValue("Database", "Port", 5432);
        ini.SaveFile();
        Console.WriteLine("Manual save completed");

        ini.Reload();
        Console.WriteLine("Data reloaded from file");
        Console.WriteLine($"Database Host: {ini.GetValue("Database", "Host", "")}");

        ini.AutoSaveInterval = 3;
        Console.WriteLine($"Auto-save every {ini.AutoSaveInterval} operations");

        Console.Write("Adding logs: ");
        ini.AutoSave += (_, _) => Console.Write("SAVED ");
        for (int i = 1; i <= 6; i++)
        {
            ini.SetValue("Logs", $"Entry{i}", $"Log message {i}");
            if (i % 3 != 0) Console.Write(".");
        }
        Console.WriteLine("Auto-save triggered");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void EventsDemo()
    {
        Console.Clear();
        Console.WriteLine("7. EVENTS AND ACTIONS DEMONSTRATION");
        using var ini = CreateDocument();

        ini.KeyAdded += (_, e) => Console.WriteLine($"NEW KEY ADDED: [{e.Section}] {e.Key} = '{e.Value}'");
        ini.KeyChanged += (_, e) => Console.WriteLine($"KEY CHANGED: [{e.Section}] {e.Key} = '{e.Value}'");
        ini.KeyRemoved += (_, e) => Console.WriteLine($"KEY REMOVED: [{e.Section}] {e.Key}");

        ini.SectionAdded += (_, e) => Console.WriteLine($"NEW SECTION: [{e.Section}]");
        ini.SectionRemoved += (_, e) => Console.WriteLine($"SECTION REMOVED: [{e.Section}]");
        ini.SectionRenamed += (_, e) => Console.WriteLine($"SECTION RENAMED: [{e.OldSection}] -> [{e.NewSection}]");
        ini.DataCleared += (_, _) => Console.WriteLine("ALL DATA CLEARED");
        ini.AutoSave += (_, _) => Console.WriteLine("AUTO-SAVING FILE...");
        ini.Saved += (_, _) => Console.WriteLine("SAVING FILE...");
        ini.Loaded += (_, _) => Console.WriteLine("FILE LOADED!");
        ini.SearchCompleted += (_, e) => Console.WriteLine($"SEARCH COMPLETED: '{e.SearchPattern}' -> {e.MatchesCount} matches");

        Console.WriteLine("Demonstrating Events/Actions:");

        Console.WriteLine("1. Added EventsDemo section");
        ini.AddSection("EventsDemo");

        Console.WriteLine("2. Added Counter key");
        ini.AddKey("EventsDemo", "Counter", 0);

        Console.WriteLine("3. Changed Counter value");
        ini.SetValue("EventsDemo", "Counter", 42);

        Console.WriteLine("4. Added Status key");
        ini.AddKey("EventsDemo", "Status", "Active");

        Console.WriteLine("5. Removed Status key");
        ini.RemoveKey("EventsDemo", "Status");

        Console.WriteLine("6. Renamed section EventsDemo -> EventsDemoRenamed");
        ini.RenameSection("EventsDemo", "EventsDemoRenamed");

        Console.WriteLine("7. Manual save triggered");
        ini.SaveFile();

        Console.WriteLine("8. Search pattern 'counter'");
        var search = ini.Search("counter");

        Console.WriteLine("\nEvents demonstration completed!");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void PerformanceModeDemo()
    {
        Console.Clear();
        Console.WriteLine("8. PERFORMANCE MODE");
        Console.WriteLine("Performance mode: all automatic features disabled, caller manages saves.");

        using var perf = CreateDocumentWithOptions(NeoIniOptions.Performance);
        Console.WriteLine($"- AutoSave: {perf.UseAutoSave}");
        Console.WriteLine($"- AutoBackup: {perf.UseAutoBackup}");
        Console.WriteLine($"- AutoAdd: {perf.UseAutoAdd}");
        Console.WriteLine($"- SaveOnDispose: {perf.SaveOnDispose}");

        Console.WriteLine("\nWriting several values without autosave:");
        for (int i = 0; i < 5; i++)
            perf.SetValue("Perf", $"Key{i}", i);

        Console.WriteLine("Data not on disk until SaveFile() is called explicitly.");
        perf.SaveFile();
        Console.WriteLine("Manual SaveFile() completed.");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void AttributeAndGeneratorDemo()
    {
        Console.Clear();
        Console.WriteLine("9. ATTRIBUTE & SOURCE GENERATOR DEMO");

        using var ini = CreateDocument();

        Console.WriteLine("Initial content:");
        Console.WriteLine(ini.ToString());

        ini.SetValue("User", "Age", 25);
        ini.SetValue("User", "IsAdmin", true);

        Console.WriteLine("\nAfter setting values manually:");
        Console.WriteLine(ini.ToString());

        NeoIniKeyTest cfg = ini.Get<NeoIniKeyTest>();
        Console.WriteLine($"\nLoaded via NeoIniKeyTest (Get<T>):");
        Console.WriteLine($"User/Age = {cfg.Age}");
        Console.WriteLine($"User/IsAdmin = {cfg.IsAdmin}");

        cfg.Age += 5;
        cfg.IsAdmin = false;
        ini.Set(cfg);

        Console.WriteLine($"\nUpdated cfg.Age to {cfg.Age}, cfg.IsAdmin to {cfg.IsAdmin} and saved via Set<T>().");

        Console.WriteLine("\nAfter generator-based Set<T>:");
        Console.WriteLine(ini.ToString());

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void CleanupDemo()
    {
        Console.Clear();
        Console.WriteLine("10. COMPLETE CLEANUP");
        using var ini = CreateDocument();

        Console.WriteLine("Final content before cleanup:");
        Console.WriteLine(ini.ToString());

        ini.DeleteFileWithData();
        Console.WriteLine("File deleted from disk + memory cleared");
    }
}
