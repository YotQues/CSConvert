using CodeConvert.Abstractions;
using CodeConvert.Exceptions;
using CodeConvert.SharedServices;
using CodeConvert.TypeScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConvert;

internal class CodeConvertionService
{
    IEnumerable<Type>? Types { get; set; }
    string? OutputPath { get; set; }
    HashSet<string> processedTypes = [];
    ICodeConverter? Converter { get; set; }
    public CodeConvertionService WithTypes(IEnumerable<Type> types)
    {
        Types = types;
        return this;
    }

    public CodeConvertionService WithOutputPath(string outputPath)
    {
        OutputPath = outputPath;
        return this;
    }

    public CodeConvertionService WithOutputType(SupportedOutputType outputType)
    {
        Converter = outputType switch
        {
            SupportedOutputType.TypeScript => new TypeScriptConverter(),
            _ => null
        };

        return this;
    }


    public void Convert()
    {
        if(Converter == null) throw new ArgumentNullException(nameof(Converter));
        if (Types == null || !Types.Any()) throw new EmptyTypesException();
        if (string.IsNullOrEmpty(OutputPath)) throw new EmptyOutputPathException();

        foreach (var type in Types)
        {
            if (processedTypes.Contains(type.FullName)) return;

            processedTypes.Add(type.FullName);


            string output = Converter.Convert(type);

            string outputDir = Path.Combine(OutputPath, GetNamespacePath(type.Namespace));

            FileSaver.SaveFile(outputDir, $"{type.Name}.{Converter.FileType}", output);
        }
    }

    static string GetNamespacePath(string namespaceName)
    {
        return namespaceName.Replace('.', Path.DirectorySeparatorChar);
    }



}
