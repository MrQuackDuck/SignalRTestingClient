using System.Text.RegularExpressions;

namespace SignalRTestingClient.Models;

public class SignalRArgument
{
    private string name = default!;

    public string Name 
    {
        get => name;
        set => name = ClearString(value);
    }
    public object Content { get; set; } = default!;

    public override string ToString() => Name;

    private string ClearString(string @string)
    {
        @string = Regex.Replace(@string, @"\s+", " ");

        return @string
            .Replace("\n", string.Empty)
            .Replace("\r", string.Empty);
    }
}
