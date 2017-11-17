// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Reflection.Emit;
using System.Reflection;
using System;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.Utilities {

    /// <summary>
    /// Generates anonymous types just like the C# object initializers 
    /// (see https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/anonymous-types)
    /// </summary>
    public static class AnonymousTypeBuilder {

        private static AssemblyName assemblyName = new AssemblyName("EFCoreCypherAnonymous");

        private static HashSet<Type> anonymousTypes = new HashSet<Type>();

        private static ModuleBuilder moduleBuilder;

        static AnonymousTypeBuilder() {
            moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(
                    assemblyName, 
                    AssemblyBuilderAccess.Run
                )
                .DefineDynamicModule(assemblyName.Name);;
        }

        /// <summary>
        /// Create anonymous type
        /// </summary>
        /// <param name="Dictionary<string"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static Type Create(
            [NotNull] List<KeyValuePair<string, Type>> properties
        ) {
            Check.NotNull(properties, nameof(properties));

            // no empty lists
            if (!properties.Any()) {
                throw new InvalidOperationException(
                    CypherStrings.NoPropertiesWhenCreatingAnonymous
                );
            }

            // no null or empty property keys
            if (properties.Any(p => String.IsNullOrWhiteSpace(p.Key))) {
                throw new ArgumentException(
                    CypherStrings.PropertyNameMayNotBeNullOrWhitespace
                );
            }

            // no null property types
            if (properties.Any(p => p.Value is null)) {
                throw new ArgumentException(
                    CypherStrings.PropertyTypeMayNotBeNull
                );
            }

            // distinct property names
            var duplicateKeys = properties
                .GroupBy(p => p.Key)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (duplicateKeys.Any()) {
                throw new InvalidOperationException(
                    CypherStrings.DuplicatePropertyWithAnonymous(duplicateKeys.First())
                );
            }

            try {
                Monitor.Enter(anonymousTypes);

                var other = Find(properties);
                if (!(other is null)) {
                    return other;
                }

                TypeBuilder typeBuilder = moduleBuilder.DefineType(
                    $"Anonymous{anonymousTypes.Count()}",
                    TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable
                );

                typeBuilder
                    .WithPropertiesAndConstructor(properties);

                Type anonymous = typeBuilder
                    .CreateTypeInfo()
                    .AsType();

                anonymousTypes.Add(anonymous);

                return anonymous;            
            } catch (Exception) {
                throw new ApplicationException(
                    CypherStrings.BailCreatingAnonymous
                );
            } finally {
                Monitor.Exit(anonymousTypes);
            }
        }

        /// <summary>
        /// Find type by properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static Type Find(List<KeyValuePair<string, Type>> properties) {
            foreach (Type anonymous in anonymousTypes) {
                // wrong size
                if (anonymous.GetProperties().Count() != properties.Count) {
                    continue;
                }

                // property name and type match
                foreach (var prop in properties) {
                    if (!anonymous.GetProperties().Any(p => p.Name == prop.Key && p.PropertyType == prop.Value)) {
                        continue;
                    }
                }

                return anonymous;
            }

            return null;
        }

        /// <summary>
        /// With properties and constructor
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="Dictionary<string"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static TypeBuilder WithPropertiesAndConstructor(
            this TypeBuilder typeBuilder, 
            List<KeyValuePair<string, Type>> properties
        ) {
            // constructor definition
            Type[] parameterTypes = properties
                .Select(p => p.Value)
                .ToArray();

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterTypes    
            );

            ILGenerator ctor = constructorBuilder.GetILGenerator();
            ctor.Emit(OpCodes.Ldarg_0);
            ctor.Emit(
                OpCodes.Call,
                typeof(object).GetConstructor(Type.EmptyTypes)
            );

            // properties
            for (int index = 0; index < properties.Count(); index++) {
                var prop = properties.ElementAt(index);

                FieldBuilder fieldBuilder = typeBuilder.DefineField(
                    $"_{prop.Key}",
                    prop.Value,
                    FieldAttributes.Private
                );

                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                    prop.Key,
                    PropertyAttributes.None,
                    prop.Value,
                    Type.EmptyTypes
                );

                MethodBuilder getter = typeBuilder.DefineMethod(
                    $"get_{prop.Key}",
                    MethodAttributes.Public,
                    prop.Value,
                    Type.EmptyTypes
                );

                ILGenerator getterIL = getter.GetILGenerator();
                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getterIL.Emit(OpCodes.Ret);

                MethodBuilder setter = typeBuilder.DefineMethod(
                    $"set_{prop.Key}",
                    MethodAttributes.Public,
                    typeof(void),
                    new Type[] { prop.Value }
                );

                ILGenerator setterIL = setter.GetILGenerator();
                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, fieldBuilder);
                setterIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getter);
                propertyBuilder.SetSetMethod(setter);

                ctor.Emit(OpCodes.Ldarg_0);
                ctor.Emit(OpCodes.Ldarg, index + 1);
                ctor.Emit(OpCodes.Stfld, fieldBuilder);
            }

            ctor.Emit(OpCodes.Ret);

            return typeBuilder;
        }
    }
}