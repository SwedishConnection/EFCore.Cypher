// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    /// Borrowed test coverage from Entity Framework Core
    /// </summary>
    public class EntityTest {

        private static CypherGraph MakeGraph() {
            return new CypherGraph();
        }

        /// <summary>
        /// No circular inheritance
        /// </summary>
        [Fact]
        public void Circular_inheritance_should_throw() {
            var graph = MakeGraph();

            var a = graph.AddEntity(typeof(A).Name);
            var b = graph.AddEntity(typeof(B).Name);
            var c = graph.AddEntity(typeof(C).Name);
            var d = graph.AddEntity(typeof(D).Name);

            b.HasBaseType(a);
            c.HasBaseType(a);
            d.HasBaseType(c);

            Assert.Equal(
                CoreStrings.CircularInheritance(a.DisplayName(), a.DisplayName()),
                Assert.Throws<InvalidOperationException>(() => { a.HasBaseType(a); }).Message);

            Assert.Equal(
                CoreStrings.CircularInheritance(a.DisplayName(), b.DisplayName()),
                Assert.Throws<InvalidOperationException>(() => { a.HasBaseType(b); }).Message);

            Assert.Equal(
                CoreStrings.CircularInheritance(a.DisplayName(), d.DisplayName()),
                Assert.Throws<InvalidOperationException>(() => { a.HasBaseType(d); }).Message);
        }

        /// <summary>
        /// B is added without a Clr type while A has a Clr type which
        /// fails when setting A as the base type of B
        /// </summary>
        [Fact]
        public void Setting_CLR_base_for_shadow_entity_type_should_throw()
        {
            var graph = MakeGraph();

            var a = graph.AddEntity(typeof(A));
            var b = graph.AddEntity(typeof(B).Name);

            Assert.Equal(
                CoreStrings.NonShadowBaseType(typeof(B).Name, typeof(A).Name),
                Assert.Throws<InvalidOperationException>(() => { b.HasBaseType(a); }).Message);
        }

        /// <summary>
        /// A is added without a Clr type while B has a Clr type which
        /// fails when setting A as the base type of B
        /// </summary>
        [Fact]
        public void Setting_shadow_base_for_CLR_entity_type_should_throw()
        {
            var graph = MakeGraph();

            var a = graph.AddEntity(typeof(A).Name);
            var b = graph.AddEntity(typeof(B));

            Assert.Equal(
                CoreStrings.NonClrBaseType(typeof(B).Name, typeof(A).Name),
                Assert.Throws<InvalidOperationException>(() => { b.HasBaseType(a); }).Message);
        }

        /// <summary>
        /// B dervives from A which fails setting B as the base type of A
        /// </summary>
        [Fact]
        public void Setting_not_assignable_base_should_throw()
        {
            var graph = MakeGraph();

            var a = graph.AddEntity(typeof(A));
            var b = graph.AddEntity(typeof(B));

            Assert.Equal(
                CoreStrings.NotAssignableClrBaseType(typeof(A).Name, typeof(B).Name, typeof(A).Name, typeof(B).Name),
                Assert.Throws<InvalidOperationException>(() => { a.HasBaseType(b); }).Message);
        }

        [Fact]
        public void Properties_on_base_type_should_be_inherited()
        {
            var graph = MakeGraph();
            
            var a = graph.AddEntity(typeof(A));
            a.AddProperty(A.GProperty);
            a.AddProperty(A.EProperty);

            var b = graph.AddEntity(typeof(B));
            b.AddProperty(B.HProperty);
            b.AddProperty(B.FProperty);

            var c = graph.AddEntity(typeof(C));
            c.AddProperty(C.HProperty);
            c.AddProperty("I", typeof(string));

            Assert.Equal(new[] { "E", "G" }, a.GetProperties().Select(p => p.Name).ToArray());
            Assert.Equal(new[] { "F", "H" }, b.GetProperties().Select(p => p.Name).ToArray());
            Assert.Equal(new[] { "H", "I" }, c.GetProperties().Select(p => p.Name).ToArray());

            b.HasBaseType(a);
            c.HasBaseType(a);

            
            Assert.Equal(new[] { "E", "G" }, a.GetProperties().Select(p => p.Name).ToArray());
            Assert.Equal(new[] { "E", "G", "F", "H" }, b.GetProperties().Select(p => p.Name).ToArray());
            Assert.Equal(new[] { "E", "G", "H", "I" }, c.GetProperties().Select(p => p.Name).ToArray());
            Assert.Equal(new[] { 0, 1, 2, 3 }, b.GetProperties().Select(p => p.GetIndex()));
            Assert.Equal(new[] { 0, 1, 2, 3 }, c.GetProperties().Select(p => p.GetIndex()));
            Assert.Same(b.FindProperty("E"), a.FindProperty("E"));
        }

        private class A
        {
            public static readonly PropertyInfo EProperty = typeof(A).GetProperty("E");
            public static readonly PropertyInfo GProperty = typeof(A).GetProperty("G");

            public string E { get; set; }
            public string G { get; set; }
        }

        private class B : A
        {
            public static readonly PropertyInfo FProperty = typeof(B).GetProperty("F");
            public static readonly PropertyInfo HProperty = typeof(B).GetProperty("H");

            public string F { get; set; }
            public string H { get; set; }
        }

        private class C : A
        {
            public static readonly PropertyInfo FProperty = typeof(C).GetProperty("F");
            public static readonly PropertyInfo HProperty = typeof(C).GetProperty("H");

            public string F { get; set; }
            public string H { get; set; }
        }

        private class D : C
        {
        }
    }
    
}
