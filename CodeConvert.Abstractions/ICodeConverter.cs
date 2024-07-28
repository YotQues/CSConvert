namespace CodeConvert.Abstractions;

public interface ICodeConverter
{
    public string FileType { get; }
    public string Convert(Type type);
}
