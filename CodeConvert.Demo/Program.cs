using System.Reflection;

namespace CodeConvert.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var assembly = Assembly.GetExecutingAssembly();
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            var asmStr = File.ReadAllText(workingDirectory + "\\CodeConvent.Demo.dll");
            var assembly = Assembly.Load(asmStr);


            Type[] types = assembly.GetTypes();
             CodeConvertionBuilder.New().WithTypes(types).WithOutputType(Abstractions.SupportedOutputType.TypeScriptPOCOs).WithOutputPath($"{projectDirectory}\\Client\\TypeScript").Build();
        }
    }
}
