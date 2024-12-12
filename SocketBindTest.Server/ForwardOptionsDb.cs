namespace SocketBindTest.Server;


internal sealed class ForwardOptionsDb
{
    private readonly Dictionary<int, ForwardOptions> _optionContainer = new();

    public void AddOption(ForwardOptions option)
    {
        _optionContainer.Add(option.ForwardPort, option);
    }
    
    public IEnumerable<ForwardOptions> Finds(Predicate<ForwardOptions>? criteria = null)
    {
        using var enumerator = _optionContainer.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var s = enumerator.Current.Value;

            if (criteria == null || criteria(s))
                yield return s;
        }
    }

    public ForwardOptions? GetListen(int forwardPort)
    {
        return Finds(s => s.ForwardPort == forwardPort).FirstOrDefault();
    }
}