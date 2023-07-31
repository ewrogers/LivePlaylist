using System.Text;

namespace LivePlaylist.Api.Data;

public class CsvReader : IDisposable
{
    private bool _isDisposed;

    private readonly TextReader _reader;

    public CsvReader(Stream stream, Encoding? encoding = null, bool leaveOpen = false)
    {
        _reader = new StreamReader(stream, encoding ?? Encoding.UTF8, leaveOpen);
    }

    public async Task<IReadOnlyList<string>?> ReadLineAsync()
    {
        CheckIfDisposed();

        var line = await _reader.ReadLineAsync();

        return line is null ? null : QuoteSplit(line).ToList();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing)
    {
        if (_isDisposed)
            return;

        if (isDisposing)
        {
            _reader.Dispose();
        }

        _isDisposed = true;
    }

    private void CheckIfDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(GetType().Name);
    }
    
    // This is a very simple CSV parser that handles quoted values
    // It ensures that commas inside of quotes are not treated as delimiters when splitting
    private static IEnumerable<string> QuoteSplit(string value, char delimiter = ',')
    {
        var buffer = new StringBuilder();
        var isQuoted = false;
        
        foreach (var c in value)
        {
            if (c == '"')
            {
                isQuoted = !isQuoted;
            }
            else if (c == delimiter && !isQuoted)
            {
                yield return buffer.ToString();
                
                buffer.Clear();
                continue;
            }

            buffer.Append(c);
        }
        
        // Add the last field, if any remaining in the buffer
        var lastField = buffer.ToString();
        if (!string.IsNullOrWhiteSpace(lastField))
            yield return lastField;
    }
}
