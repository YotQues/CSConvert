using CodeConvert.Abstractions;
using CodeConvert.TypeScript;
using System.Reflection;
using System.Runtime.Serialization;

namespace CodeConvert.Tests;



public class TypeScriptPOCOConverterTest
{
    Type[] types = Assembly.GetAssembly(typeof(TypeScriptPOCOConverterTest)).GetTypes().Where(t => t.GetCustomAttribute<DataContractAttribute>() != null).ToArray();
    ICodeConverter converter = new TypeScriptPOCOConverter();


    [Theory]
    [InlineData(typeof(DemoClass), DemoClass.expectedResult)]
    [InlineData(typeof(DemoRecord), DemoRecord.expectedResult)]
    [InlineData(typeof(DemoStruct), DemoStruct.expectedResult)]
    [InlineData(typeof(DemoEnum), DemoEnumExpected.expectedResult)]
    public void HandlesObjectTypes(Type type, string expected)
    {
        var processedTypes = new HashSet<string>();

        string actual = converter.Convert(types.First(t => t.Name == type.Name)).Trim();

        Assert.Equal(expected.Trim(), actual);
    }

    [Theory]
    [InlineData(typeof(DemoNumberClass), DemoNumberClass.expectedResult)]
    [InlineData(typeof(DemoStringClass), DemoStringClass.expectedResult)]
    public void HandlesNativeTypes(Type type, string expected)
    {
        var processedTypes = new HashSet<string>();

        string actual = converter.Convert(types.First(t => t.Name == type.Name)).Trim();

        Assert.Equal(expected.Trim(), actual);
    }

    [Theory]
    [InlineData(typeof(DemoGenericClass<>), DemoGeneric.expectedResultClass)]
    [InlineData(typeof(DemoGenericCollections<>), DemoGeneric.expectedResultCollections)]
    public void HandlesGenerics(Type type, string expected)
    {
        var processedTypes = new HashSet<string>();

        string actual = converter.Convert(types.First(t => t.Name == type.Name)).Trim();

        Assert.Equal(expected.Trim(), actual);
    }

    [Theory]
    [InlineData(typeof(DemoDictClass), DemoDictClass.expectedResult)]
    [InlineData(typeof(DemoArrayClass), DemoArrayClass.expectedResult)]
    [InlineData(typeof(DemoListClass), DemoListClass.expectedResult)]
    [InlineData(typeof(DemoHashSetClass), DemoHashSetClass.expectedResult)]
    [InlineData(typeof(DemoDeepArrayClass), DemoDeepArrayClass.expectedResult)]
    public void HandlesCollections(Type type, string expected)
    {
         var processedTypes = new HashSet<string>();

        string actual = converter.Convert(types.First(t => t.Name == type.Name)).Trim();

        Assert.Equal(expected.Trim(), actual);

    }
}

[DataContract]
internal class DemoClass
{
    public const string expectedResult = "export interface DemoClass {\n}";
}

[DataContract]
internal record DemoRecord()
{
    public const string expectedResult = "export interface DemoRecord {\n}";
};

[DataContract]
internal struct DemoStruct
{
    public const string expectedResult = "export interface DemoStruct {\n}";
}


[DataContract]
internal class DemoStringClass
{
    public string DemoString { get; set; }
    [IgnoreDataMember]
    public const string expectedResult = "export interface DemoStringClass {\n\tDemoString: string;\n}";
}

[DataContract]
internal class DemoNumberClass
{

    public int DemoInt { get; set; }
    public double DemoDouble { get; set; }
    public float DemoFloat { get; set; }
    public decimal DemoDecimal { get; set; }

    [IgnoreDataMember]
    public const string expectedResult = "export interface DemoNumberClass {\n\tDemoInt: number;\n\tDemoDouble: number;\n\tDemoFloat: number;\n\tDemoDecimal: number;\n}";
}

[DataContract]
internal enum DemoEnum
{
    DemoMember1,
    DemoMember2 = 2,
    DemoMember3 = 2003
}

public static class DemoEnumExpected
{
    public const string expectedResult = "export enum DemoEnum {\n\tDemoMember1 = 0,\n\tDemoMember2 = 2,\n\tDemoMember3 = 2003\n}";
}


[DataContract]
internal class DemoDictClass
{
    public Dictionary<string, int> DemoDict { get; set; } = [];

    [IgnoreDataMember]
    public const string expectedResult = "\nexport interface DemoDictClass {\n\tDemoDict: Record<string, number>;\n}";
}

[DataContract]
internal class DemoArrayClass
{
    public int[] DemoArray { get; set; }
    [IgnoreDataMember]
    public const string expectedResult = "export interface DemoArrayClass {\n\tDemoArray: Array<number>;\n}";
}
[DataContract]
internal class DemoDeepArrayClass
{
    public int[][] DemoArray { get; set; }
    [IgnoreDataMember]
    public const string expectedResult = "export interface DemoDeepArrayClass {\n\tDemoArray: Array<Array<number>>;\n}";
}

[DataContract]
internal class DemoListClass
{
    public List<double> DemoList { get; set; }
    [IgnoreDataMember]
    public const string expectedResult = "export interface DemoListClass {\n\tDemoList: Array<number>;\n}";
}


[DataContract]
internal class DemoHashSetClass
{
    public HashSet<float> DemoHashSet { get; set; }
    [IgnoreDataMember]
    public const string expectedResult = "export interface DemoHashSetClass {\n\tDemoHashSet: Array<number>;\n}";
}


[DataContract]
internal class DemoGenericClass<TGeneric>
{
}

[DataContract]
internal class DemoGenericCollections<TGeneric>
{
    public Dictionary<string, TGeneric> DemoDict { get; set; } = [];
    public IEnumerable<TGeneric> DemoArray1 { get; set; } = [];
    public List<TGeneric> DemoArray2 { get; set; } = [];
    public HashSet<TGeneric> DemoArray3 { get; set; } = [];
    public TGeneric[] DemoArray4 { get; set; } = [];
}


static class DemoGeneric
{
    public const string expectedResultClass = "\nexport interface DemoGenericClass<TGeneric> {\n}";
    public const string expectedResultCollections = "\nexport interface DemoGenericCollections<TGeneric> {\n\tDemoDict: Record<string, TGeneric>;\n\tDemoArray1: Array<TGeneric>;\n\tDemoArray2: Array<TGeneric>;\n\tDemoArray3: Array<TGeneric>;\n\tDemoArray4: Array<TGeneric>;\n}";
}




