﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace Silk.NET.BuildTools.Converters.Khronos
{
    public class ExtensionDefinition
    {
        public string Name { get; }
        public int Number { get; }
        public string Type { get; }
        public ExtensionConstant[] Constants { get; }
        public EnumExtensionValue[] EnumExtensions { get; }
        public string[] CommandNames { get; }
        public string[] TypeNames { get; }
        public string[] Supported { get; }

        public ExtensionDefinition(
            string name,
            int number,
            string type,
            ExtensionConstant[] constants,
            EnumExtensionValue[] enumExtensions,
            string[] commandNames,
            string[] typeNames,
            string[] supported)
        {
            Name = name;
            Number = number;
            Type = type;
            Constants = constants;
            EnumExtensions = enumExtensions;
            CommandNames = commandNames;
            TypeNames = typeNames;
            Supported = supported;
        }

        public static ExtensionDefinition CreateFromXml(XElement xe)
        {
            string name = xe.GetNameAttribute();
            string numberString = xe.Attribute("number").Value;
            int number = int.Parse(numberString);
            string type = xe.GetTypeAttributeOrNull();
            List<ExtensionConstant> extensionConstants = new List<ExtensionConstant>();
            List<EnumExtensionValue> enumExtensions = new List<EnumExtensionValue>();
            List<string> commandNames = new List<string>();
            List<string> typeNames = new List<string>();
            string[] supported = xe.Attribute("supported").Value.Split('|');

            foreach (var require in xe.Elements("require"))
            {
                foreach (var enumXE in require.Elements("enum"))
                {
                    string enumName = enumXE.GetNameAttribute();
                    string extends = enumXE.Attribute("extends")?.Value;
                    if (extends != null)
                    {
                        string valueString;
                        string offsetString = enumXE.Attribute("offset")?.Value;
                        if (offsetString != null)
                        {
                            int offset = int.Parse(offsetString);
                            int direction = 1;
                            if (enumXE.Attribute("dir")?.Value == "-")
                            {
                                direction = -1;
                            }

                            int value = direction * (1000000000 + (number - 1) * 1000 + offset);
                            valueString = value.ToString();
                        }
                        else
                        {
                            string bitPosString = enumXE.Attribute("bitpos")?.Value;
                            if (bitPosString != null)
                            {
                                int shift = int.Parse(bitPosString);
                                valueString = (1 << shift).ToString();
                            }
                            else
                            {
                                valueString = enumXE.Attribute("value").Value;
                            }
                        }
                        enumExtensions.Add(new EnumExtensionValue(extends, enumName, valueString));
                    }
                    else
                    {
                        var valueAttribute = enumXE.Attribute("value");
                        if (valueAttribute == null)
                            continue;

                        extensionConstants.Add(new ExtensionConstant(name, valueAttribute.Value));
                    }
                }
                foreach (var commandXE in require.Elements("command"))
                {
                    commandNames.Add(commandXE.GetNameAttribute());
                }

                foreach (var typeXE in require.Elements("type"))
                {
                    typeNames.Add(typeXE.GetNameAttribute());
                }
            }
            return new ExtensionDefinition(name, number, type, extensionConstants.ToArray(), enumExtensions.ToArray(), commandNames.ToArray(), typeNames.ToArray(), supported);
        }
    }

    public class ExtensionConstant
    {
        public string Name { get; }
        public string Value { get; }
        public ExtensionConstant(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    [DebuggerDisplay("{DebuggerDisplayString}")]
    public class EnumExtensionValue
    {
        public string ExtendedType { get; }
        public string Name { get; }
        public string Value { get; }

        public EnumExtensionValue(string extendedType, string name, string value)
        {
            ExtendedType = extendedType;
            Name = name;
            Value = value;
        }

        private string DebuggerDisplayString =>
            $"Ext: {ExtendedType}, {Name} = {Value}";
    }
}
