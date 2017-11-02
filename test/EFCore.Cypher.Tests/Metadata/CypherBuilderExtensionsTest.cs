
// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    /// Uses relational builder test cases (<see ref="RelationalBuilderExtensionsTest" />) as 
    /// a template
    /// </summary>
    public class CypherBuilderExtensionsTest {
        protected virtual ModelBuilder CreateConventionModelBuilder() 
            => CypherTestHelpers.Instance.CreateConventionBuilder();

        protected virtual ModelBuilder CreateModelBuilder() =>
            new ModelBuilder(TestCypherConventionSetBuilder.Build());

        /// <summary>
        /// Set labels (with generics)
        /// </summary>
        [Fact]
        public void Can_set_labels()
        {
            // TODO: Replace with convention builder
            var modelBuilder = CreateModelBuilder();

            modelBuilder
                .Entity<Customer>()
                .HasLabels(new[] { "Customizer" });

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(Customer));

            Assert.Equal("Customer", entityType.DisplayName());
            Assert.Equal("Customizer", entityType.Cypher().Labels.Single());
        }

        /// <summary>
        /// Set labels without generics
        /// </summary>
        [Fact]
        public void Can_set_labels_non_generic()
        {
            // TODO: Replace with convention builder
            var modelBuilder = CreateModelBuilder();

            modelBuilder
                .Entity(typeof(Customer))
                .HasLabels(new[] { "Customizer" });

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(Customer));

            Assert.Equal("Customer", entityType.DisplayName());
            Assert.Equal("Customizer", entityType.Cypher().Labels.Single());
        }

        /// <summary>
        /// When labels are not defined, are null or not null 
        /// but empty
        /// </summary>
        [Fact]
        public void Can_get_labels_negative() {
            // TODO: Replace with convention builder
            var modelBuilder = CreateModelBuilder();

            // when labels have not be defined
            var entityBuilder = modelBuilder
                .Entity<Customer>();

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(Customer));

            Assert.Equal("Customer", entityType.DisplayName());
            Assert.Equal("Customer", entityType.Cypher().Labels.Single());

            // when labels are null
            modelBuilder
                .Entity<Customer>();

            entityBuilder.HasLabels(null);

            Assert.Equal("Customer", entityType.DisplayName());
            Assert.Equal("Customer", entityType.Cypher().Labels.Single());

            // when labels are not null but empty
            modelBuilder
                .Entity<Customer>();

            Assert.Throws<ArgumentException>(
                () => entityBuilder.HasLabels(new string[] {})
            );
        }

        [Fact]
        public void Can_set_labels_override() {
            // TODO: Replace with convention builder
            var modelBuilder = CreateModelBuilder();

            // labels from data annotation
            var entityBuilder = modelBuilder
                .Entity<Order>();

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(Order));

            Assert.Equal("Order", entityType.DisplayName());
            Assert.Equal("Custom Ordering", entityType.Cypher().Labels.Single());

            // explicit trumfs data annotation
            entityBuilder
                .HasLabels(new[] { "Overrider" });

            Assert.Equal("Order", entityType.DisplayName());
            Assert.Equal("Overrider", entityType.Cypher().Labels.Single());
        }

        [Fact]
        public void Can_inherit_labels() {
            // TODO: Replace with convention builder
            var modelBuilder = CreateModelBuilder();

            // labels from data annotation
            var entityBuilder = modelBuilder
                .Entity<International>();

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(International));

            Assert.Equal("International", entityType.DisplayName());
            string[] labels = entityType.Cypher().Labels;
            Assert.True(new [] { "Customer", "International" }.SequenceEqual(labels));
        }

        [Fact]
        public void Can_set_labels_from_defining_navigation() {
            // TODO: Replace with convention builder
            var modelBuilder = CreateModelBuilder();

            // labels from data annotation
            var entityBuilder = modelBuilder
                .Entity<Customer>();

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(OrderDetails));

            Assert.Equal("OrderDetails", entityType.DisplayName());
            string[] labels = entityType.Cypher().Labels;
            Assert.True(entityType.HasDefiningNavigation());
            Assert.NotEmpty(labels);
        }

        [Fact]
        public void ForeignKey_expectation_1() {
            // TODO: Replace with convention builder
            var modelBuilder = CreateModelBuilder();

            var entityBuilder = modelBuilder
                .Entity<Customer>();

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(Customer));

            var keys = entityType.GetKeys();
            var fks = entityType.GetForeignKeys();
            var props = entityType.GetProperties();
            var navs = entityType.GetNavigations();

            // the key is a shadow along with the forth property and fk of the navigation
            Assert.Equal(1, keys.Count());
            Assert.Empty(fks);
            Assert.Equal(4, props.Count());
            Assert.Equal(1, navs.Count());
        }

        private class Customer
        {
            public string Name { get; set; }
            public short SomeShort { get; set; }
            public MyEnum EnumValue { get; set; }

            public IEnumerable<Order> Orders { get; set; }
        }

        private class International: Customer {

        }

        private enum MyEnum : ulong
        {
            Sun,
            Mon,
            Tue
        }

        [Labels(new string[] { "Custom Ordering" })]
        private class Order
        {
            public Customer Customer { get; set; }

            public OrderDetails Details { get; set; }
        }

        private class OrderDetails
        {
            public Order Order { get; set; }
        }
    }
}