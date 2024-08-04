using System.Runtime.Serialization;

namespace CSConvert.Tests;
[DataContract]
internal class DemoClass
{
}

[DataContract]
internal record DemoRecord()
{
};

[DataContract]
internal struct DemoStruct
{
}


[DataContract]
internal class DemoStringClass
{
    public string DemoString { get; set; }
}

[DataContract]
internal class DemoNumberClass
{

    public int DemoInt { get; set; }
    public double DemoDouble { get; set; }
    public float DemoFloat { get; set; }
    public decimal DemoDecimal { get; set; }

}

[DataContract]
internal enum DemoEnum
{
    DemoMember1,
    DemoMember2 = 2,
    DemoMember3 = 2003
}

[DataContract]
internal class DemoDictClass
{
    public Dictionary<string, int> DemoDict { get; set; } = [];

}

[DataContract]
internal class DemoArrayClass
{
    public int[] DemoArray { get; set; }
}
[DataContract]
internal class DemoDeepArrayClass
{
    public int[][] DemoArray { get; set; }
}

[DataContract]
internal class DemoListClass
{
    public List<double> DemoList { get; set; }
}


[DataContract]
internal class DemoHashSetClass
{
    public HashSet<float> DemoHashSet { get; set; }
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

[DataContract]
internal class DemoNestedTypeClass {
    public DemoEnum DemoEnum { get; set; }
    public Dictionary<string, DemoGenericClass<int>> DemoDict { get; set; }
}

