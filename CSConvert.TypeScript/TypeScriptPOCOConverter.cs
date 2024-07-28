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

  static string ConvertToTypeScript(Type type)
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

  static string ConvertClassToTypeScript(Type type)
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


    var genericParams = !type.ContainsGenericParameters ? "" : $"<{string.Join(", ", type.GetGenericArguments().Select(t => t.Name.Split("`")[0]))}>";
    var imports = string.Join("", nestedTypes.Select(t => $"import type {{ {t} }} from \"./{t}.ts\"\n"));
    var renderedMembers = string.Join("", members.Select(m => $"\n\t{m}"));
    var typeName = type.Name.Split('`')[0];


    string baseResult = $"{imports}\nexport interface {typeName}{genericParams} {{{renderedMembers}\n}}";

    return baseResult;
  }

  static string ConvertEnumToTypeScript(Type type)
  {
    var members = Enum.GetNames(type).Select(name => $"\n\t{name} = {(int)Enum.Parse(type, name)}");

    return $"export enum {type.Name} {{{string.Join(",", members)}\n}}";
  }

  static string? GetTypeScriptType(Type type, HashSet<string> nestedTypes)
  {
    switch (Type.GetTypeCode(type))
    {
      case TypeCode.Int32:
      case TypeCode.Int64:
      case TypeCode.Double:
      case TypeCode.Single:
      case TypeCode.Decimal:
        return TypeScriptTypes.Number;
      case TypeCode.String:
        return TypeScriptTypes.String;
      case TypeCode.Boolean:
        return TypeScriptTypes.Boolean;
      default:
        return GetNoneNativeTypeScriptType(type, nestedTypes);
    }
  }

  private static string? GetNoneNativeTypeScriptType(Type type, HashSet<string> nestedTypes)
  {
    if (type.IsGenericTypeParameter)
    {
      return type.Name.Split("`")[0];
    }
    if (type.IsGenericType)
    {
      var genericArguments = string.Join(", ", type.GetGenericArguments().Select(arg => GetTypeScriptType(arg, nestedTypes)));

      string typeName;

      if (typeof(IDictionary).IsAssignableFrom(type))
      {
        typeName = $"Record<{genericArguments}>";
      }
      else if (typeof(IEnumerable).IsAssignableFrom(type))
      {
        typeName = $"Array<{genericArguments}>";
      }
      else
      {
        typeName = $"{type.Name.Split('`')[0]}<{genericArguments}>";
      }

      return typeName;
    }
    else if (typeof(Array).IsAssignableFrom(type))
    {
      return $"Array<{GetTypeScriptType(type.GetElementType(), nestedTypes)}>";
    }
    else if (typeof(DateTime).IsAssignableFrom(type))
    {
      return TypeScriptTypes.Date;
    }
    else if (type.IsEnum)
    {
      return type.Name;
    }
    else if (type.GetCustomAttribute<DataContractAttribute>() != null)
    {
      if (!nestedTypes.Contains(type.Name))
        nestedTypes.Add(type.Name);
      return type.Name;
    }
    else
    {
      return null;
    }
  }

  public string Convert(Type type, out string fileName)
  {
    var result = Convert(type);
    fileName = $"{type.Name.Split('`')[0]}.{FileType}";
    return result;
  }

  public Func<Type, bool> GetTypesFilter() => t => t.GetCustomAttribute<DataContractAttribute>() != null;
}
