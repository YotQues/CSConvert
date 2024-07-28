namespace CodeConvert.Exceptions;

public class EmptyTypesException : Exception
{
    public EmptyTypesException() : base("No types added. Use 'ICodeConverter.WithTypes' method") { }
}
