using System.Text;
using NeoIniLight.Annotations;
using NeoIniLight.Models;

namespace NeoIniLight.Tests;

public class NeoIniDocumentBasicTests : IDisposable
{
    private readonly string _tempFile;
    private readonly NeoIniDocument _doc;

    public NeoIniDocumentBasicTests()
    {
        _tempFile = Path.GetTempFileName();
        _doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
    }

    public void Dispose()
    {
        _doc.Dispose();
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
    }

    [Fact]
    public void AddSection_And_Exists_ShouldWork()
    {
        _doc.AddSection("Test");
        Assert.True(_doc.SectionExists("Test"));
        Assert.False(_doc.SectionExists("Missing"));
    }

    [Fact]
    public void AddKey_And_GetValue_ShouldWork()
    {
        _doc.AddKey("Test", "Key1", 123);
        var value = _doc.GetValue<int>("Test", "Key1");
        Assert.Equal(123, value);
    }

    [Fact]
    public void GetValue_WithAutoAdd_ShouldCreateDefault()
    {
        var value = _doc.GetValue("Test", "Key2", 456);
        Assert.Equal(456, value);
        Assert.True(_doc.KeyExists("Test", "Key2"));
    }

    [Fact]
    public void SetValue_ShouldUpdateOrCreate()
    {
        _doc.SetValue("Test", "Key3", "Hello");
        Assert.Equal("Hello", _doc.GetValue<string>("Test", "Key3"));
        _doc.SetValue("Test", "Key3", "World");
        Assert.Equal("World", _doc.GetValue<string>("Test", "Key3"));
    }

    [Fact]
    public void RemoveKey_ShouldDelete()
    {
        _doc.SetValue("Test", "Key4", 1);
        _doc.RemoveKey("Test", "Key4");
        Assert.False(_doc.KeyExists("Test", "Key4"));
    }

    [Fact]
    public void RemoveSection_ShouldDelete()
    {
        _doc.SetValue("Test", "Key5", 1);
        _doc.RemoveSection("Test");
        Assert.False(_doc.SectionExists("Test"));
    }

    [Fact]
    public void ClearSection_ShouldRemoveKeysButKeepSection()
    {
        _doc.SetValue("Test", "Key6", 1);
        _doc.SetValue("Test", "Key7", 2);
        _doc.ClearSection("Test");
        Assert.True(_doc.SectionExists("Test"));
        var keys = _doc.GetAllKeys("Test");
        Assert.NotNull(keys);
        Assert.Empty(keys);
    }

    [Fact]
    public void GetAllSections_ShouldReturnList()
    {
        _doc.AddSection("A");
        _doc.AddSection("B");
        var sections = _doc.GetAllSections();
        Assert.Equal(new[] { "A", "B" }, sections);
    }

    [Fact]
    public void GetAllKeys_ShouldReturnList()
    {
        _doc.SetValue("Test", "KeyA", 1);
        _doc.SetValue("Test", "KeyB", 2);
        var keys = _doc.GetAllKeys("Test");
        Assert.Equal(new[] { "KeyA", "KeyB" }, keys);
    }

    [Fact]
    public void GetSection_ShouldReturnDictionary()
    {
        _doc.SetValue("Test", "K1", 10);
        _doc.SetValue("Test", "K2", 20);
        var dict = _doc.GetSection("Test");
        Assert.Equal(2, dict.Count);
        Assert.Equal("10", dict["K1"]);
        Assert.Equal("20", dict["K2"]);
    }

    [Fact]
    public void FindKey_ShouldReturnMatchingSections()
    {
        _doc.SetValue("Sec1", "shared", 1);
        _doc.SetValue("Sec2", "shared", 2);
        var result = _doc.FindKey("shared");
        Assert.Equal(2, result.Count);
        Assert.Equal("1", result["Sec1"]);
        Assert.Equal("2", result["Sec2"]);
    }

    [Fact]
    public void Search_ShouldFindKeysAndValues()
    {
        _doc.SetValue("Test", "Key1", "Hello World");
        _doc.SetValue("Test", "Key2", "Something else");
        var results = _doc.Search("Hello");
        Assert.Single(results);
        Assert.Equal("Key1", results[0].Key);
        Assert.Equal("Hello World", results[0].Value);
    }

    [Fact]
    public void RenameKey_ShouldWork()
    {
        _doc.SetValue("Test", "Old", 123);
        _doc.RenameKey("Test", "Old", "New");
        Assert.False(_doc.KeyExists("Test", "Old"));
        Assert.True(_doc.KeyExists("Test", "New"));
        Assert.Equal(123, _doc.GetValue<int>("Test", "New"));
    }

    [Fact]
    public void RenameSection_ShouldWork()
    {
        _doc.SetValue("OldSection", "Key", 1);
        _doc.RenameSection("OldSection", "NewSection");
        Assert.False(_doc.SectionExists("OldSection"));
        Assert.True(_doc.SectionExists("NewSection"));
        Assert.Equal(1, _doc.GetValue<int>("NewSection", "Key"));
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalseIfMissing() => Assert.False(_doc.TryGetValue("Missing", "Key", out int _));

    [Fact]
    public void TryGetValue_ShouldReturnTrueIfExists()
    {
        _doc.SetValue("Test", "Key", 42);
        Assert.True(_doc.TryGetValue("Test", "Key", out int value));
        Assert.Equal(42, value);
    }

    [Fact]
    public void GetValueClamped_ShouldClamp()
    {
        _doc.SetValue("Test", "Val", 10);
        var clamped = _doc.GetValueClamped("Test", "Val", 0, 5);
        Assert.Equal(5, clamped);
    }

    [Fact]
    public void AddKeyClamped_ShouldStoreClamped()
    {
        _doc.AddKeyClamped("Test", "Val", 0, 5, 10);
        var value = _doc.GetValue<int>("Test", "Val");
        Assert.Equal(5, value);
    }

    [Fact]
    public void SetValueClamped_ShouldStoreClamped()
    {
        _doc.SetValueClamped("Test", "Val", 0, 5, 10);
        var value = _doc.GetValue<int>("Test", "Val");
        Assert.Equal(5, value);
    }

    [Fact]
    public void Clear_ShouldClearAllData()
    {
        _doc.SetValue("A", "K", 1);
        _doc.Clear();
        var sections = _doc.GetAllSections();
        Assert.NotNull(sections);
        Assert.Empty(sections);
    }

    [Fact]
    public void ToString_ShouldReturnSerializedContent()
    {
        _doc.SetValue("Test", "Key", 123);
        var str = _doc.ToString();
        Assert.Contains("[Test]", str);
        Assert.Contains("Key = 123", str);
    }

    [Fact]
    public void SetValues_BatchOperation_ShouldWork()
    {
        var values = new[]
        {
            new NeoIniValue("Test", "Key1", 1),
            new NeoIniValue("Test", "Key2", 2),
            new NeoIniValue("Test", "Key3", 3)
        };
        _doc.SetValues(values);

        Assert.Equal(1, _doc.GetValue<int>("Test", "Key1"));
        Assert.Equal(2, _doc.GetValue<int>("Test", "Key2"));
        Assert.Equal(3, _doc.GetValue<int>("Test", "Key3"));
    }

    [Fact]
    public async Task SetValuesAsync_BatchOperation_ShouldWork()
    {
        var values = new[]
        {
            new NeoIniValue("Test", "Key1", 1),
            new NeoIniValue("Test", "Key2", 2)
        };
        await _doc.SetValuesAsync(values);

        Assert.Equal(1, _doc.GetValue<int>("Test", "Key1"));
        Assert.Equal(2, _doc.GetValue<int>("Test", "Key2"));
    }
}

public class NeoIniDocumentFileTests : IDisposable
{
    private readonly string _tempFile;
    private readonly NeoIniDocument _doc;

    public NeoIniDocumentFileTests()
    {
        _tempFile = Path.GetTempFileName();
        _doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
    }

    public void Dispose()
    {
        _doc.Dispose();
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
        if (File.Exists(_tempFile + ".backup")) File.Delete(_tempFile + ".backup");
    }

    [Fact]
    public void SaveFile_ShouldWriteToDisk()
    {
        _doc.SetValue("Test", "Key", 123);
        _doc.SaveFile();
        var content = File.ReadAllText(_tempFile);
        Assert.Contains("[Test]", content);
        Assert.Contains("Key = 123", content);
    }

    [Fact]
    public void Reload_ShouldReReadFromDisk()
    {
        _doc.SetValue("Test", "Key", 123);
        _doc.SaveFile();

        using (var tempDoc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false }))
            tempDoc.SetValue("Test", "Key", 456);

        _doc.Reload();
        var value = _doc.GetValue<int>("Test", "Key");
        Assert.Equal(456, value);
    }

    [Fact]
    public void DeleteFile_ShouldRemoveFile()
    {
        _doc.SaveFile();
        Assert.True(File.Exists(_tempFile));
        _doc.DeleteFile();
        Assert.False(File.Exists(_tempFile));
    }

    [Fact]
    public void DeleteFileWithData_ShouldClearData()
    {
        _doc.SetValue("Test", "Key", 123);
        _doc.DeleteFileWithData();
        Assert.False(File.Exists(_tempFile));
        var sections = _doc.GetAllSections();
        if (sections is not null) Assert.Empty(sections);
    }

    [Fact]
    public void DeleteBackup_ShouldRemoveBackup()
    {
        _doc.SetValue("Test", "Key", 123);
        _doc.SaveFile();
        _doc.DeleteBackup();
        Assert.False(File.Exists(_tempFile + ".backup"));
    }
}

public class NeoIniDocumentOptionsTests : IDisposable
{
    private readonly string _tempFile;

    public NeoIniDocumentOptionsTests() => _tempFile = Path.GetTempFileName();

    public void Dispose()
    {
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
        if (File.Exists(_tempFile + ".backup")) File.Delete(_tempFile + ".backup");
    }

    [Fact]
    public void UseAutoAdd_WhenFalse_ShouldNotAddMissingKey()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoAdd = false });
        var value = doc.GetValue("Missing", "Key", 123);
        Assert.Equal(123, value);
        Assert.False(doc.KeyExists("Missing", "Key"));
    }

    [Fact]
    public void AllowEmptyValues_WhenFalse_ShouldThrow()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { AllowEmptyValues = false });
        Assert.Throws<EmptyValueNotAllowedException>(() => doc.SetValue("Test", "Key", ""));
    }

    [Fact]
    public void UseAutoSave_ShouldSaveOnModify()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = true });
        doc.SetValue("Test", "Key", 123);
        Assert.True(File.Exists(_tempFile));
        var content = File.ReadAllText(_tempFile);
        Assert.Contains("Key = 123", content);
    }

    [Fact]
    public void AutoSaveInterval_ShouldBufferSaves()
    {
        var options = NeoIniOptions.BufferedAutoSave(2);
        var doc = new NeoIniDocument(_tempFile, options);
        doc.SetValue("Test", "Key1", 1);
        doc.SetValue("Test", "Key2", 2);
        var content = File.ReadAllText(_tempFile);
        Assert.Contains("Key1 = 1", content);
        Assert.Contains("Key2 = 2", content);
    }

    [Fact]
    public void UseAutoBackup_ShouldCreateBackupOnSave()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoBackup = true });
        doc.SetValue("Test", "Key", 123);
        doc.SaveFile();
        Assert.True(File.Exists(_tempFile + ".backup"));
    }

    [Fact]
    public void SaveOnDispose_ShouldSaveWhenDisposed()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { SaveOnDispose = true });
        doc.SetValue("Test", "Key", 123);
        doc.Dispose();
        Assert.True(File.Exists(_tempFile));
        var content = File.ReadAllText(_tempFile);
        Assert.Contains("Key = 123", content);
    }
}

public class NeoIniDocumentEventsTests : IDisposable
{
    private readonly string _tempFile;
    private readonly NeoIniDocument _doc;

    public NeoIniDocumentEventsTests()
    {
        _tempFile = Path.GetTempFileName();
        _doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
    }

    public void Dispose()
    {
        _doc.Dispose();
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
    }

    [Fact]
    public void KeyAdded_ShouldFire()
    {
        bool fired = false;
        _doc.KeyAdded += (s, e) => fired = true;
        _doc.AddKey("Test", "Key", 1);
        Assert.True(fired);
    }

    [Fact]
    public void KeyChanged_ShouldFire()
    {
        bool fired = false;
        _doc.SetValue("Test", "Key", 1);
        _doc.KeyChanged += (s, e) => fired = true;
        _doc.SetValue("Test", "Key", 2);
        Assert.True(fired);
    }

    [Fact]
    public void KeyRenamed_ShouldFire()
    {
        bool fired = false;
        _doc.SetValue("Test", "Old", 1);
        _doc.KeyRenamed += (s, e) => fired = true;
        _doc.RenameKey("Test", "Old", "New");
        Assert.True(fired);
    }

    [Fact]
    public void KeyRemoved_ShouldFire()
    {
        bool fired = false;
        _doc.SetValue("Test", "Key", 1);
        _doc.KeyRemoved += (s, e) => fired = true;
        _doc.RemoveKey("Test", "Key");
        Assert.True(fired);
    }

    [Fact]
    public void SectionAdded_ShouldFire()
    {
        bool fired = false;
        _doc.SectionAdded += (s, e) => fired = true;
        _doc.AddSection("New");
        Assert.True(fired);
    }

    [Fact]
    public void SectionRemoved_ShouldFire()
    {
        bool fired = false;
        _doc.AddSection("ToRemove");
        _doc.SectionRemoved += (s, e) => fired = true;
        _doc.RemoveSection("ToRemove");
        Assert.True(fired);
    }

    [Fact]
    public void SectionRenamed_ShouldFire()
    {
        bool fired = false;
        _doc.AddSection("Old");
        _doc.SectionRenamed += (s, e) => fired = true;
        _doc.RenameSection("Old", "New");
        Assert.True(fired);
    }

    [Fact]
    public void SectionChanged_ShouldFireOnKeyChange()
    {
        bool fired = false;
        _doc.SetValue("Test", "Key", 1);
        _doc.SectionChanged += (s, e) => fired = true;
        _doc.SetValue("Test", "Key", 2);
        Assert.True(fired);
    }

    [Fact]
    public void DataCleared_ShouldFire()
    {
        bool fired = false;
        _doc.DataCleared += (s, e) => fired = true;
        _doc.Clear();
        Assert.True(fired);
    }

    [Fact]
    public void Saved_ShouldFireOnSave()
    {
        bool fired = false;
        _doc.Saved += (s, e) => fired = true;
        _doc.SaveFile();
        Assert.True(fired);
    }

    [Fact]
    public void Loaded_ShouldFireOnReload()
    {
        bool fired = false;
        _doc.Loaded += (s, e) => fired = true;
        _doc.Reload();
        Assert.True(fired);
    }
}

public class NeoIniDocumentAsyncTests : IDisposable
{
    private readonly string _tempFile;

    public NeoIniDocumentAsyncTests() => _tempFile = Path.GetTempFileName();

    public void Dispose()
    {
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
        if (File.Exists(_tempFile + ".backup")) File.Delete(_tempFile + ".backup");
    }

    [Fact]
    public async Task CreateAsync_ShouldLoadIfAutoLoadTrue()
    {
        var doc = await NeoIniDocument.CreateAsync(_tempFile);
        Assert.NotNull(doc);
        var sections = doc.GetAllSections();
        if (sections is not null) Assert.Empty(sections);
        doc.Dispose();
    }

    [Fact]
    public async Task AddKeyAsync_ShouldWork()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.AddKeyAsync("Test", "Key", 123);
        var value = await doc.GetValueAsync<int>("Test", "Key");
        Assert.Equal(123, value);
    }

    [Fact]
    public async Task SetValueAsync_ShouldWork()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.SetValueAsync("Test", "Key", 456);
        var value = await doc.GetValueAsync<int>("Test", "Key");
        Assert.Equal(456, value);
    }

    [Fact]
    public async Task RemoveKeyAsync_ShouldWork()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.SetValueAsync("Test", "Key", 1);
        await doc.RemoveKeyAsync("Test", "Key");
        Assert.False(await Task.Run(() => doc.KeyExists("Test", "Key")));
    }

    [Fact]
    public async Task RemoveSectionAsync_ShouldWork()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.AddSectionAsync("Test");
        await doc.RemoveSectionAsync("Test");
        Assert.False(await Task.Run(() => doc.SectionExists("Test")));
    }

    [Fact]
    public async Task ClearSectionAsync_ShouldWork()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.SetValueAsync("Test", "Key1", 1);
        await doc.SetValueAsync("Test", "Key2", 2);
        await doc.ClearSectionAsync("Test");
        Assert.True(await Task.Run(() => doc.SectionExists("Test")));
        var keys = await Task.Run(() => doc.GetAllKeys("Test"));
        Assert.NotNull(keys);
        Assert.Empty(keys);
    }

    [Fact]
    public async Task RenameKeyAsync_ShouldWork()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.SetValueAsync("Test", "Old", 1);
        await doc.RenameKeyAsync("Test", "Old", "New");
        var value = await doc.GetValueAsync<int>("Test", "New");
        Assert.Equal(1, value);
    }

    [Fact]
    public async Task RenameSectionAsync_ShouldWork()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.SetValueAsync("Old", "Key", 1);
        await doc.RenameSectionAsync("Old", "New");
        var value = await doc.GetValueAsync<int>("New", "Key");
        Assert.Equal(1, value);
    }

    [Fact]
    public async Task SaveFileAsync_ShouldSave()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.SetValueAsync("Test", "Key", 123);
        await doc.SaveFileAsync();
        var content = await File.ReadAllTextAsync(_tempFile);
        Assert.Contains("Key = 123", content);
    }

    [Fact]
    public async Task ReloadAsync_ShouldReload()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.SetValueAsync("Test", "Key", 123);
        await doc.SaveFileAsync();

        using (var tempDoc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false }))
            tempDoc.SetValue("Test", "Key", 456);

        await doc.ReloadAsync();

        var value = doc.GetValue<int>("Test", "Key");
        Assert.Equal(456, value);
    }

    [Fact]
    public async Task GetValueClampedAsync_ShouldClamp()
    {
        var doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
        await doc.SetValueAsync("Test", "Val", 10);
        var clamped = await doc.GetValueClampedAsync("Test", "Val", 0, 5);
        Assert.Equal(5, clamped);
    }
}

public class NeoIniGeneratorTests : IDisposable
{
    private readonly string _tempFile;
    private readonly NeoIniDocument _doc;

    public NeoIniGeneratorTests()
    {
        _tempFile = Path.GetTempFileName();
        _doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false });
    }

    public void Dispose()
    {
        _doc.Dispose();
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
    }

    public class TestConfig
    {
        [NeoIniKey("Settings", "Name", "DefaultName")]
        public string? Name { get; set; }

        [NeoIniKey("Settings", "Age", 18)]
        public int Age { get; set; }

        [NeoIniKey("Settings", "Enabled", true)]
        public bool Enabled { get; set; }
    }

    [Fact]
    public void Get_ShouldMapFromDocument()
    {
        _doc.SetValue("Settings", "Name", "Alice");
        _doc.SetValue("Settings", "Age", 30);
        _doc.SetValue("Settings", "Enabled", false);

        var config = _doc.Get<TestConfig>();
        Assert.Equal("Alice", config.Name);
        Assert.Equal(30, config.Age);
        Assert.False(config.Enabled);
    }

    [Fact]
    public void Get_ShouldUseDefaultValuesIfMissing()
    {
        var config = _doc.Get<TestConfig>();
        Assert.Equal("DefaultName", config.Name);
        Assert.Equal(18, config.Age);
        Assert.True(config.Enabled);
    }

    [Fact]
    public void Set_ShouldWriteToDocument()
    {
        var config = new TestConfig { Name = "Bob", Age = 25, Enabled = true };
        _doc.Set(config);

        Assert.Equal("Bob", _doc.GetValue<string>("Settings", "Name"));
        Assert.Equal(25, _doc.GetValue<int>("Settings", "Age"));
        Assert.True(_doc.GetValue<bool>("Settings", "Enabled"));
    }
}

public class NeoIniExceptionTests : IDisposable
{
    private readonly string _tempFile;
    private readonly NeoIniDocument _doc;

    public NeoIniExceptionTests()
    {
        _tempFile = Path.GetTempFileName();
        _doc = new NeoIniDocument(_tempFile, new NeoIniOptions { UseAutoSave = false, AllowEmptyValues = true });
    }

    public void Dispose()
    {
        _doc.Dispose();
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
    }

    [Fact]
    public void AddSection_WithInvalidCharacters_ShouldThrow()
    {
        Assert.Throws<UnsupportedIniCharacterException>(() => _doc.AddSection("Section;"));
        Assert.Throws<UnsupportedIniCharacterException>(() => _doc.AddSection("Section="));
        Assert.Throws<UnsupportedIniCharacterException>(() => _doc.AddSection("Section\""));
    }

    [Fact]
    public void AddKey_WithInvalidCharacters_ShouldThrow()
    {
        Assert.Throws<UnsupportedIniCharacterException>(() => _doc.AddKey("Test", "Key;", "value"));
        Assert.Throws<UnsupportedIniCharacterException>(() => _doc.AddKey("Test", "Key=", "value"));
        Assert.Throws<UnsupportedIniCharacterException>(() => _doc.AddKey("Test", "Key\"", "value"));
    }

    [Fact]
    public void AddKey_WithEmptyValue_WhenDisallowed_ShouldThrow()
    {
        var doc2 = new NeoIniDocument(_tempFile, new NeoIniOptions { AllowEmptyValues = false });
        Assert.Throws<EmptyValueNotAllowedException>(() => doc2.AddKey("Test", "Key", ""));
    }

    [Fact]
    public void GetValue_WithInvalidType_ShouldReturnDefaultAndRaiseError()
    {
        _doc.SetValue("Test", "Key", "not an int");
        bool errorRaised = false;
        _doc.Error += (s, e) => errorRaised = true;
        var value = _doc.GetValue<int>("Test", "Key", 42);
        Assert.Equal(42, value);
        Assert.True(errorRaised);
    }

    [Fact]
    public void GetValue_WithValidEnum_ShouldParse()
    {
        _doc.SetValue("Test", "Mode", "Debug");
        var mode = _doc.GetValue<TestEnum>("Test", "Mode", TestEnum.Release);
        Assert.Equal(TestEnum.Debug, mode);
    }

    [Fact]
    public void GetValue_WithInvalidEnum_ShouldReturnDefault()
    {
        _doc.SetValue("Test", "Mode", "InvalidValue");
        var mode = _doc.GetValue<TestEnum>("Test", "Mode", TestEnum.Release);
        Assert.Equal(TestEnum.Release, mode);
    }

    [Fact]
    public void GetValue_WithDateTime_ShouldParse()
    {
        var date = new DateTime(2024, 1, 15, 10, 30, 0);
        _doc.SetValue("Test", "Date", date);
        var result = _doc.GetValue<DateTime>("Test", "Date");
        Assert.Equal(date, result);
    }

    private enum TestEnum
    {
        Debug,
        Release
    }
}
