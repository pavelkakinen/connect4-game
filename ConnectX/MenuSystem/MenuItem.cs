namespace MenuSystem;

public class MenuItem
{
    public string Key { get; set; } = default!;
    public string Value { get; set; } =  default!;
    // thing that should happen...
    
    public Func<string>? MethodToRun { get; set; }

    public override string ToString()
    {
        return $"{Key}) {Value}";
    }

}