using CodeConvert.Abstractions;
using System.Reflection;
using System.Runtime.Serialization;

namespace CodeConvert.CLI;
class Program
{
    static void Main(string[] args)
    {

        try
        {
            Console.WriteLine(string.Join(", ", args));
            var (assemblyPath, outputPath, outputType) = ParseArguments(args);

            var assembly = Assembly.LoadFrom(assemblyPath);
            var types = assembly.GetTypes().Where(t => t.GetCustomAttribute<DataContractAttribute>() != null);

             CodeConvertionBuilder.New().WithTypes(types).WithOutputType(outputType).WithOutputPath(outputPath).Build();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }


    static Options ParseArguments(string[] args)
    {
        var named = new Dictionary<AllowedArgs, string>();
        string[] allowedArgNames = Enum.GetNames<AllowedArgs>();

        foreach (string arg in args)
        {
            string[] parts = arg.Split("=").ToArray();

            if (parts.Length != 2)
            {
                continue;
            }


            string argName = parts[0].Split('-')[1];
            if (!allowedArgNames.Contains(argName))
            {
                throw new ArgumentException($"{argName} is not a valid argument for CodeConvert");
            }

            AllowedArgs enumMember = Enum.Parse<AllowedArgs>(argName);


            named[enumMember] = parts[1];
        }

        foreach (var arg in Enum.GetValues<AllowedArgs>())
        {
            if (named.ContainsKey(arg)) { continue; }
            throw new ArgumentNullException($"-{Enum.GetName(arg)} is required");
        }


        return new(named[AllowedArgs.AssemblyPath], named[AllowedArgs.OutputPath], Enum.Parse<SupportedOutputType>(named[AllowedArgs.OutputType]));
    }

    record Options(string AssemblyPath, string OutputPath, SupportedOutputType OutputType);

    enum AllowedArgs
    {
        AssemblyPath,
        OutputPath,
        OutputType
    }
}



