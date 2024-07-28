namespace CodeConvert.Exceptions;

public class EmptyOutputPathException : Exception
{
    public EmptyOutputPathException() : base("No outputPath added. Use 'ICodeConverter.WithOutputPath' method") { }
}
