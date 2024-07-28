using System.Linq.Expressions;

namespace CSConvert.Abstractions;

public interface ICodeConverter
{
  public Func<Type, bool> GetTypesFilter();
  public string FileType { get; }
  public string Convert(Type type);
  public string Convert(Type type, out string fileName);
}
