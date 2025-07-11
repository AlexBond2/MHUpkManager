using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using UpkManager.Helpers;
using UpkManager.Constants;

namespace UpkManager.Models.UpkFile.Properties
{
    public static class UnrealPropertyFactory
    {
        public static UnrealPropertyValueBase Create(string type)
        {
            if (CoreRegistry.TryGetProperty(type, out var prop))
                return new UnrealPropertyCoreValue(prop);

            return type.ToLower() switch
            {
                "int" => new UnrealPropertyIntValue(),
                "angle" => new UnrealPropertyIntValue(),
                "bool" => new UnrealPropertyBoolValue(),
                "float" => new UnrealPropertyFloatValue(),
                _ => new UnrealPropertyValueBase()
            };
        }
    }

    public class UnrealPropertyCoreValue : UnrealPropertyValueBase
    {
        public string StructName { get; }
        public CustomCoreJson Definition { get; private set; }
        public List<(string Name, UnrealPropertyValueBase Value)> Fields { get; } = [];
        public override PropertyTypes PropertyType => PropertyTypes.StructProperty;

        public UnrealPropertyCoreValue(CustomCoreJson core)
        {
            StructName = core.Name;
            Definition = core;

            foreach (var field in core.Fields)
            {
                var fieldValue = UnrealPropertyFactory.Create(field.Type);
                Fields.Add((field.Name, fieldValue));
            }
        }

        public override string PropertyString
        {
            get
            {
                if (string.IsNullOrEmpty(Definition?.Format))
                    return StructName;

                string result = Definition.Format;

                for (int i = 0; i < Definition.Fields.Count; i++)
                {
                    var fieldDef = Definition.Fields[i];
                    var fieldType = fieldDef.Type.ToLower();
                    var fieldValue = Fields[i].Value;

                    result = ReplaceWithObject(result, fieldDef.Name, fieldType, fieldValue);
                }

                return result;
            }
        }

        private static string ReplaceWithObject(string format, string fieldName, string fieldType, UnrealPropertyValueBase value)
        {
            var regex = new Regex(@"\{" + fieldName + @"(?::(F\d+|X|x))?\}");

            return regex.Replace(format, match =>
            {
                string formatSpecifier = match.Groups[1].Value;
                switch (fieldType)
                {
                    case "angle":
                        if (value is UnrealPropertyIntValue intVal)
                        {
                            float angle = (int)intVal.PropertyValue / 32768.0f * 180.0f;
                            return FormatFloat(angle, formatSpecifier);
                        }
                        break;

                    case "float":
                        if (value is UnrealPropertyFloatValue floatVal)
                            return FormatFloat((float)floatVal.PropertyValue, formatSpecifier);
                        break;

                    case "int":
                        if (value is UnrealPropertyIntValue intVal2)
                            return FormatInt((int)intVal2.PropertyValue, formatSpecifier);
                        break;

                    default:
                        return value.PropertyString;
                }

                return "null";
            });
        }

        private static string FormatFloat(float value, string format)
        {
            if (string.IsNullOrEmpty(format)) return value.ToString(CultureInfo.InvariantCulture);
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        private static string FormatInt(int value, string format)
        {
            if (string.IsNullOrEmpty(format)) return value.ToString();
            return value.ToString(format);
        }

        public override void BuildVirtualTree(VirtualNode valueTree)
        {
            if (Definition.Format is not null)
                base.BuildVirtualTree(valueTree);
            else
                for (int i = 0; i < Definition.Fields.Count; i++)
                {
                    var node = new VirtualNode($"{Fields[i].Name} ::{Definition.Fields[i].Type}");
                    Fields[i].Value.BuildVirtualTree(node);
                    valueTree.Children.Add(node);
                }
        }

        public override void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            foreach (var (_, value) in Fields)
                value.ReadPropertyValue(reader, size, header, property);
        }
    }

}
