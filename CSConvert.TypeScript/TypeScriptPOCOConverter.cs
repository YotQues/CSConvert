using CSConvert.Abstractions;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace CSConvert.TypeScript;

public class TypeScriptPOCOConverter : ICodeConverter
{
    private const string FILE_TYPE = "ts";
    public string FileType { get => FILE_TYPE; }

    public string Convert(Type scriptType)
    {

        string tsDefinition = ConvertToTypeScript(scriptType);
        return tsDefinition;
    }

    private static string ConvertToTypeScript(Type type)
    {
        if (type.IsEnum)
        {
            return ConvertEnumToTypeScript(type);
        }
        else
        {
            return ConvertClassToTypeScript(type);
        }
    }

    private static string ConvertClassToTypeScript(Type type)
    {
        var nestedTypes = new HashSet<string>();
        var properties = type.GetProperties().Where(p => p.GetCustomAttribute<IgnoreDataMemberAttribute>() == null).ToArray();
        List<string> members = [];

        foreach (var p in properties)
        {
            var propTypeName = GetTypeScriptType(p.PropertyType, nestedTypes);
            if (propTypeName == null) continue;
            members.Add($"{p.Name + (Nullable.GetUnderlyingType(p.PropertyType) != null ? "?" : "")}: {propTypeName};");
        }


        var genericParams = !type.ContainsGenericParameters ? "" : $"<{string.Join(", ", type.GetGenericArguments().Select(GetTypeName))}>";
        var imports = string.Join("", nestedTypes.OrderBy(t => t).Select(t => $"import type {{ {t} }} from \"./{t}.ts\";\n"));
        var renderedMembers = string.Join("", members.Select(m => $"\n\t{m}"));
        var typeName = GetTypeName(type);

        string baseResult = $"{imports}\nexport interface {typeName}{genericParams} {{{renderedMembers}\n}}";

        return baseResult;
    }

    private static string ConvertEnumToTypeScript(Type type)
    {
        var members = Enum.GetNames(type).Select(name => $"\n\t{name} = {(int)Enum.Parse(type, name)}");

        return $"export enum {type.Name} {{{string.Join(",", members)}\n}}";
    }

    private static string? GetTypeScriptType(Type type, HashSet<string> nestedTypes)
    {
        if(type.IsEnum)
        {
            return GetNoneNativeTypeScriptType(type, nestedTypes); 
        }
        
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Int32 or TypeCode.Int64 or TypeCode.Double or TypeCode.Single or TypeCode.Decimal => TypeScriptTypes.Number,
            TypeCode.String => TypeScriptTypes.String,
            TypeCode.Boolean => TypeScriptTypes.Boolean,
            _ => GetNoneNativeTypeScriptType(type, nestedTypes),
        };
    }

    private static string? GetNoneNativeTypeScriptType(Type type, HashSet<string> nestedTypes)
    {
        if (IsDataContract(type, nestedTypes))
        {
            return HandleDataContract(type, nestedTypes);
        }

        if (type.IsGenericTypeParameter)
        {
            return GetTypeName(type);
        }

        if (type.IsGenericType)
        {
            return HandleGenericType(type, nestedTypes);
        }

        if (typeof(Array).IsAssignableFrom(type))
        {
            return $"Array<{GetTypeScriptType(type.GetElementType(), nestedTypes)}>";
        }

        if (typeof(DateTime).IsAssignableFrom(type))
        {
            return TypeScriptTypes.Date;
        }

        return null;
    }

    private static string HandleDataContract(Type type, HashSet<string> nestedTypes)
    {
        if (type.IsGenericType)
        {
            return HandleGenericType(type, nestedTypes);
        }

        return GetTypeName(type);
    }

    private static bool IsDataContract(Type type, HashSet<string> nestedTypes)
    {
        bool isDataContract = type.GetCustomAttribute<DataContractAttribute>() != null;
        string typeName = GetTypeName(type);

        if (isDataContract && !nestedTypes.Contains(typeName))
        {
            nestedTypes.Add(typeName);
        }

        return isDataContract;
    }

    private static string GetTypeName(Type type) => type.Name.Split('`')[0];

    private static string HandleGenericType(Type type, HashSet<string> nestedTypes)
    {
        string genericArguments = string.Join(", ", type.GetGenericArguments().Select(arg => GetTypeScriptType(arg, nestedTypes)));

        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            return $"Record<{genericArguments}>";
        }
        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            return $"Array<{genericArguments}>";
        }

        return $"{GetTypeName(type)}<{genericArguments}>";

    }

    public string Convert(Type type, out string fileName)
    {
        string result = Convert(type);
        fileName = $"{type.Name.Split('`')[0]}.{FileType}";
        return result;
    }

    public Func<Type, bool> GetTypesFilter() => t => t.GetCustomAttribute<DataContractAttribute>() != null;
}
