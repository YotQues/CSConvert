namespace CodeConvert;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

class Program
{
    static void Main(string[] args)
    {
        string assemblyPath = args[0];
        string outputPath = args[1];

        var assembly = Assembly.LoadFrom(assemblyPath);
        var types = assembly.GetTypes().Where(t => t.GetCustomAttribute<DataContractAttribute>() != null);

        new CodeConvertionService().WithTypes(types).WithOutputType(Abstractions.SupportedOutputType.TypeScript).WithOutputPath("./clients/typescript"); 
    }

    
}

