﻿using System.Xml.Linq;

namespace Silk.NET.BuildTools.Converters.Khronos
{
    public static class XElementExtensions
    {
        public static string GetNameAttribute(this XElement xe)
        {
            return xe.Attribute("name").Value;
        }

        public static string GetNameElement(this XElement xe)
        {
            return xe.Element("name").Value;
        }

        public static string GetTypeElement(this XElement xe)
        {
            return xe.Element("type").Value;
        }

        public static string GetTypeAttributeOrNull(this XElement xe)
        {
            return xe.Attribute("type")?.Value;
        }

        public static bool GetOptionalAttributeOrFalse(this XElement xe)
        {
            var attr = xe.Attribute("optional");
            return attr != null && attr.Value == "true";
        }

        public static bool HasCategoryAttribute(this XElement xe, string value)
        {
            var attr = xe.Attribute("category");
            return attr != null && attr.Value == value;
        }
    }
}
