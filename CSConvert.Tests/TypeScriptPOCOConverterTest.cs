using CSConvert.Abstractions;
using CSConvert.TypeScript;
using System.Reflection;
using System.Runtime.Serialization;

namespace CSConvert.Tests;

public sealed class TypeScriptPOCOConverterTest : CodeConverterTestBase
{
    override protected Type[] types { get; set; } = Assembly.GetAssembly(typeof(TypeScriptPOCOConverterTest)).GetTypes().Where(t => t.GetCustomAttribute<DataContractAttribute>() != null).ToArray();
    protected override ICodeConverter converter { get; } = new TypeScriptPOCOConverter();

    public TypeScriptPOCOConverterTest()
    {
    }

    [Theory]
    [InlineData(typeof(DemoClass), ExpectedTypeScriptOutput.DemoClass)]
    [InlineData(typeof(DemoRecord), ExpectedTypeScriptOutput.DemoRecord)]
    [InlineData(typeof(DemoStruct), ExpectedTypeScriptOutput.DemoStruct)]
    [InlineData(typeof(DemoEnum), ExpectedTypeScriptOutput.DemoEnum)]
    [InlineData(typeof(DemoNestedTypeClass), ExpectedTypeScriptOutput.DemoNestedTypeClass)]
    public void HandlesObjectTypes(Type type, string expected)
    {
        TestConverter(type, expected);
    }

    [Theory]
    [InlineData(typeof(DemoNumberClass), ExpectedTypeScriptOutput.DemoNumberClass)]
    [InlineData(typeof(DemoStringClass), ExpectedTypeScriptOutput.DemoStringClass)]
    public void HandlesNativeTypes(Type type, string expected)
    {
        TestConverter(type, expected);
    }

    [Theory]
    [InlineData(typeof(DemoGenericClass<>), ExpectedTypeScriptOutput.DemoGenericClass)]
    [InlineData(typeof(DemoGenericCollections<>), ExpectedTypeScriptOutput.DemoGenericCollectionsClass)]
    public void HandlesGenerics(Type type, string expected)
    {
        TestConverter(type, expected);
    }

    [Theory]
    [InlineData(typeof(DemoDictClass), ExpectedTypeScriptOutput.DemoDictClass)]
    [InlineData(typeof(DemoArrayClass), ExpectedTypeScriptOutput.DemoArrayClass)]
    [InlineData(typeof(DemoListClass), ExpectedTypeScriptOutput.DemoListClass)]
    [InlineData(typeof(DemoHashSetClass), ExpectedTypeScriptOutput.DemoHashSetClass)]
    [InlineData(typeof(DemoDeepArrayClass), ExpectedTypeScriptOutput.DemoDeepArrayClass)]
    public void HandlesCollections(Type type, string expected)
    {
        TestConverter(type, expected);
    }
}

public static class ExpectedTypeScriptOutput
{

    public const string DemoClass = "export interface DemoClass {\n}";
    public const string DemoRecord = "export interface DemoRecord {\n}";
    public const string DemoStruct = "export interface DemoStruct {\n}";
    public const string DemoStringClass = "export interface DemoStringClass {\n\tDemoString: string;\n}";
    public const string DemoNumberClass = "export interface DemoNumberClass {\n\tDemoInt: number;\n\tDemoDouble: number;\n\tDemoFloat: number;\n\tDemoDecimal: number;\n}";
    public const string DemoEnum = "export enum DemoEnum {\n\tDemoMember1 = 0,\n\tDemoMember2 = 2,\n\tDemoMember3 = 2003\n}";
    public const string DemoDictClass = "\nexport interface DemoDictClass {\n\tDemoDict: Record<string, number>;\n}";
    public const string DemoArrayClass = "export interface DemoArrayClass {\n\tDemoArray: Array<number>;\n}";
    public const string DemoDeepArrayClass = "export interface DemoDeepArrayClass {\n\tDemoArray: Array<Array<number>>;\n}";
    public const string DemoListClass = "export interface DemoListClass {\n\tDemoList: Array<number>;\n}";
    public const string DemoHashSetClass = "export interface DemoHashSetClass {\n\tDemoHashSet: Array<number>;\n}";
    public const string DemoGenericClass = "\nexport interface DemoGenericClass<TGeneric> {\n}";
    public const string DemoGenericCollectionsClass = "\nexport interface DemoGenericCollections<TGeneric> {\n\tDemoDict: Record<string, TGeneric>;\n\tDemoArray1: Array<TGeneric>;\n\tDemoArray2: Array<TGeneric>;\n\tDemoArray3: Array<TGeneric>;\n\tDemoArray4: Array<TGeneric>;\n}";
    public const string DemoNestedTypeClass = "import type { DemoEnum } from \"./DemoEnum.ts\";\nimport type { DemoGenericClass } from \"./DemoGenericClass.ts\";\n\nexport interface DemoNestedTypeClass {\n\tDemoEnum: DemoEnum;\n\tDemoDict: Record<string, DemoGenericClass<number>>;\n}";
}

