using System;
using System.Collections.Generic;

using System.Reflection;
using UpkManager.Models.UpkFile.Classes;

namespace UpkManager.Models.UpkFile.Core
{
    public class ClassRegistry
    {
        private readonly Dictionary<string, Type> _classMap = new(StringComparer.OrdinalIgnoreCase);
        public static ClassRegistry Instance { get; } = new ClassRegistry();

        private ClassRegistry()
        {
            var unrealObjectType = typeof(UObject);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    if (unrealObjectType.IsAssignableFrom(type))
                    {
                        var attr = type.GetCustomAttribute<UnrealClassAttribute>();
                        if (attr != null && !_classMap.ContainsKey(attr.ClassName))
                        {
                            _classMap[attr.ClassName] = type;
                        }
                    }
        }

        public bool TryGetType(string className, out Type type) => _classMap.TryGetValue(className, out type);
    }
}
