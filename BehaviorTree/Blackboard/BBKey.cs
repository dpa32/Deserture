public class BBKey<T>
{
    public readonly string Name;
    public BBKey(string name)
    {
        Name = name;
    }
    public override string ToString() => Name;
}
