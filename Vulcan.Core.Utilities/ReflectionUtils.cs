using System;
using System.Reflection;

namespace Vulcan.Core.Utilities
{
    public static class ReflectionUtils
    {
        private static void ProcessProperty(dynamic property, dynamic obj, string oldValue, string newValue, string propertyTypeNamespace, string propertyHierarchy = "", short index = -1)
        {
            var isNull = true;
            dynamic propertyValue = null;
            try
            {
                propertyValue = index == -1 ? property.GetValue(obj) : property.GetValue(obj, new object[] {index});

                if (propertyValue != null)
                    isNull = false;
            }
            catch (Exception ex)
            {
                
            }
            if (isNull) return;

            //propertyValue = index == -1 ? property.GetValue(obj) : property.GetValue(obj, new object[] { index });
            //If we have a collection, then iterate through its elements.
            var isCollection = property.PropertyType.GetInterface("System.Collections.IEnumerable") != null;
            string currentPropertyName = property.Name;
            string currentPropertyType = property.PropertyType.Name;
            string currentPropertyTypeNamespace = property.PropertyType.Namespace;
            if (currentPropertyTypeNamespace != propertyTypeNamespace && currentPropertyType != "String" && !isCollection)
                return;
            if (isCollection && !(propertyValue is string))
                foreach (var item in propertyValue)
                    SubstituteSchemaName(item, oldValue, newValue, propertyTypeNamespace, propertyHierarchy + "." + currentPropertyName);
            //If field name and type matches, then set.  
            else
                if (currentPropertyName == "Value" && currentPropertyType == "String"
                    && (propertyHierarchy.EndsWith("MultiPartIdentifier.Item") ||
                        propertyHierarchy.EndsWith("SchemaIdentifier") ||
                        propertyHierarchy.EndsWith("Name"))
                    && propertyValue == oldValue)
                property.SetValue(obj, newValue);
            else
                    if (currentPropertyName == "Value" && currentPropertyType == "String"
                        && propertyHierarchy.EndsWith("Predicate.Expression.Parameters"))
                ProcessStringLiteral(propertyValue, property, obj, oldValue, newValue);
            else
                        if (currentPropertyTypeNamespace == propertyTypeNamespace)
                SubstituteSchemaName(propertyValue, oldValue, newValue, propertyTypeNamespace, propertyHierarchy + "." + currentPropertyName);
        }

        private static void ProcessStringLiteral(dynamic propertyValue, dynamic property, dynamic obj, string oldValue, string newValue)
        {
            string finalValue = propertyValue;
            //finalValue = finalValue.Replace(oldValue, newValue);
            var strArr = finalValue.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < strArr.Length; i++)
            {
                if (strArr[i] == oldValue)
                    strArr[i] = newValue;
            }
            finalValue = string.Join(".", strArr);
            property.SetValue(obj, finalValue);
        }


        public static void SubstituteSchemaName(dynamic obj, string oldValue, string newValue, string propertyTypeNamespace, string propertyHierarchy = "")
        {
            if (propertyHierarchy == "")
                propertyHierarchy += obj.GetType().Name;
            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetIndexParameters().Length == 1)
                {
                    int indexLength = obj.Count;
                    for (short i = 0; i < indexLength; i++)
                    {
                        ProcessProperty(property, obj, oldValue, newValue, propertyTypeNamespace, propertyHierarchy, i);
                    }
                }
                if (property.GetIndexParameters().Length == 0)
                    ProcessProperty(property, obj, oldValue, newValue, propertyTypeNamespace, propertyHierarchy);
            }
        }
        
    }
}
