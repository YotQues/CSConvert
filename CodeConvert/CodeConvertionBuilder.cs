using CodeConvert.Abstractions;
using CodeConvert.Exceptions;
using CodeConvert.SharedServices;
using CodeConvert.TypeScript;
using System.Reflection;
using System.Runtime.Serialization;

namespace CodeConvert;

public class CodeConvertionBuilder /*: IWithTypes, IWithOutputPath, IWithOutputType, IBuilder*/
{
    private CodeConvertionBuilder() { }

    public static CodeConvertionBuilder New()
    {
        return new CodeConvertionBuilder();
    }

    IEnumerable<Type>? Types { get; set; }
    string? OutputPath { get; set; }
    HashSet<string> processedTypes = [];
    ICodeConverter? Converter { get; set; }
    public CodeConvertionBuilder WithTypes(IEnumerable<Type> types)
    {
        Types = types.ToArray();
        return this;
    }

    public CodeConvertionBuilder WithOutputPath(string outputPath)
    {
        OutputPath = outputPath;
        return this;
    }

    public CodeConvertionBuilder WithOutputType(SupportedOutputType outputType)
    {
        Converter = outputType switch
        {
            SupportedOutputType.TypeScriptPOCOs => new TypeScriptPOCOConverter(),
            _ => null
        };

        return this;
    }

    public void Build()
    {
        if (Converter == null) throw new ArgumentNullException(nameof(Converter));

        if (Types == null) throw new EmptyTypesException();
        var types = Types.Where(Converter.GetTypesFilter()).ToArray();
        if (types.Length == 0) throw new EmptyTypesException();

        if (string.IsNullOrEmpty(OutputPath)) throw new EmptyOutputPathException();

        foreach (var type in types)
        {
            if (processedTypes.Contains(type.FullName)) return;

            processedTypes.Add(type.FullName);


            string result = Converter.Convert(type, out string fileName);

            string outputDir = Path.Combine(OutputPath, GetNamespacePath(type.Namespace));

            FileSaver.SaveFile(outputDir, fileName, result);
        }
    }

    static string GetNamespacePath(string namespaceName)
    {
        return namespaceName.Replace('.', Path.DirectorySeparatorChar);
    }


}

public interface IBuilder
{
    void Build();
}

public interface IWithOutputPath
{
    IBuilder WithOutputPath(string outputPath);
}
public interface IWithOutputType
{
    IWithOutputPath WithOutputType(SupportedOutputType outputType);
}
public interface IWithTypes
{
    IWithOutputType WithTypes(IEnumerable<Type> types);
}