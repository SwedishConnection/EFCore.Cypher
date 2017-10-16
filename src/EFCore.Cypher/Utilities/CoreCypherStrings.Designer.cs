using System.Reflection;
using System.Resources;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Internal
{
    public static class CoreCypherStrings {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("CoreCypherStrings", typeof(CoreCypherStrings).GetTypeInfo().Assembly);

        /// <summary>
        /// Node '{node}' cannot inherit from '{baseNode}' because '{baseNode}' is a shadow state node while '{node}' is not
        /// </summary>
        /// <param name="node"></param>
        /// <param name="baseNode"></param>
        /// <returns></returns>
        public static string NonClrBaseNode([CanBeNull] object node, [CanBeNull] object baseNode) =>
            string.Format(
                GetString("NonClrBaseNode", nameof(node), nameof(baseNode)),
                node,
                baseNode
            );

        /// <summary>
        ///     The node '{node}' cannot inherit from '{baseNode}' because '{node}' is a shadow state node while '{baseNode}' is not.
        /// </summary>
        public static string NonShadowBaseNode([CanBeNull] object node, [CanBeNull] object baseNode) => 
            string.Format(
                GetString("NonShadowBaseNode", nameof(node), nameof(baseNode)),
                node, 
                baseNode
            );

        /// <summary>
        ///     The node '{node}' cannot inherit from '{baseNode}' because '{clrType}' is not a descendent of '{baseClrType}'.
        /// </summary>
        public static string NotAssignableClrBaseNode([CanBeNull] object node, [CanBeNull] object baseNode, [CanBeNull] object clrType, [CanBeNull] object baseClrType) => 
            string.Format(
                GetString("NotAssignableClrBaseNode", nameof(node), nameof(baseNode), nameof(clrType), nameof(baseClrType)),
                node, 
                baseNode, 
                clrType, 
                baseClrType
            );

        /// <summary>
        ///     The node '{node}' cannot inherit from '{baseNode}' because '{baseNode}' is a descendent of '{node}'.
        /// </summary>
        public static string CircularInheritance([CanBeNull] object node, [CanBeNull] object baseNode) => 
            string.Format(
                GetString("CircularInheritance", nameof(node), nameof(baseNode)),
                node, 
                baseNode
            );

        /// <summary>
        ///     The specified field '{field}' of type '{fieldType}' cannot be used for the property '{node}.{property}' of type '{propertyType}'. Only backing fields of types that are assignable from the property type can be used.
        /// </summary>
        public static string BadBackingFieldType([CanBeNull] object field, [CanBeNull] object fieldType, [CanBeNull] object node, [CanBeNull] object property, [CanBeNull] object propertyType) => 
            string.Format(
                GetString("BadBackingFieldType", nameof(field), nameof(fieldType), nameof(node), nameof(property), nameof(propertyType)),
                field, 
                fieldType, 
                node, 
                property, 
                propertyType
            );

        /// <summary>
        ///     The specified field '{field}' could not be found for property '{property}' on node '{node}'.
        /// </summary>
        public static string MissingBackingField([CanBeNull] object field, [CanBeNull] object property, [CanBeNull] object node) => 
            string.Format(
                GetString("MissingBackingField", nameof(field), nameof(property), nameof(node)),
                field, 
                property, 
                node
            );

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }
            return value;
        }
    }
}