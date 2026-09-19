namespace MediatorSharp.Tests.Fixtures;

public sealed class CallLog
{
    private readonly List<string> _entries = new();

    public IReadOnlyList<string> Entries => _entries;

    public void Add(string entry) => _entries.Add(entry);
}
