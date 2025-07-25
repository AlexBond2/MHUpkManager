using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UpkManager.Models.UpkFile.Core
{
    public class CoreRegistry
    {
        private readonly Dictionary<string, Type> _types;
        public static CoreRegistry Instance { get; } = new CoreRegistry();

        private CoreRegistry()
        {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => t.GetCustomAttribute<AtomicStructAttribute>() != null && !t.IsInterface && !t.IsAbstract)
                .ToDictionary(
                    t => t.GetCustomAttribute<AtomicStructAttribute>()!.Name, 
                    t => t, 
                    StringComparer.OrdinalIgnoreCase
                );
        }

        public bool TryGetProperty(string name, out Type definition)
        {
            return _types.TryGetValue(name, out definition);
        }
    }
}
