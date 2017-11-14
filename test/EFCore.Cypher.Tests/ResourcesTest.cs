// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using System.Resources;
using Xunit;

namespace Microsoft.EntityFrameworkCore {

    public class ResourcesTest {

        [Fact]
        public void Load_resources() {
            var name = typeof(ResourcesTest).GetTypeInfo().Assembly.GetName().Name;

            ResourceManager rm = new ResourceManager(
                name + ".ResourceStrings", 
                typeof(ResourcesTest).GetTypeInfo().Assembly
            );

            var text = GetString(
                rm,
                "NotAForeignKeyMember",
                null,
                null,
                null
            );

            Assert.NotNull(text);
        }

        private static string GetString(
            ResourceManager rm, 
            string name, 
            params string[] formatterNames
        )
        {
            var value = rm.GetString(name);
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }
            return value;
        }
    }
}