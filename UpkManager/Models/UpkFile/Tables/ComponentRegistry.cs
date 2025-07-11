using System;
using System.Collections.Generic;
using System.IO;

namespace UpkManager.Models.UpkFile.Tables
{
    public static class ComponentRegistry
    {
        private static HashSet<string> _componentClassNames = new(StringComparer.OrdinalIgnoreCase);

        public static void LoadFromFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            _componentClassNames = new HashSet<string>(lines, StringComparer.OrdinalIgnoreCase);
        }

        public static bool HasComponent(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
                return false;

            return _componentClassNames.Contains(className);
        }
    }
}
