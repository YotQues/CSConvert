namespace CodeConvert.TypeScript;

internal static class TypeScriptTypes
{
    public const string String = "string";
    public const string Number = "number";
    public const string Boolean = "boolean";
    public const string Object = "object";
    public const string Any = "any";
    public const string Null = "null";
    public const string Undefined = "undefined";
    public const string Date = "Date";
    public static string Array(string genericType) => $"Array<{genericType}>";
    public static string Record(params string[] genericTypes) => $"Record<{genericTypes[0]}, {genericTypes[1]}>";
}
