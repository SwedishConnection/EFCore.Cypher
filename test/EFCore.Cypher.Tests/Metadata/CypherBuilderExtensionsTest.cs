
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
            new ModelBuilder(new ConventionSet());

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

        private class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public short SomeShort { get; set; }
            public MyEnum EnumValue { get; set; }

            public IEnumerable<Order> Orders { get; set; }
        }

        private enum MyEnum : ulong
        {
            Sun,
            Mon,
            Tue
        }

        private class Order
        {
            public int OrderId { get; set; }

            public int CustomerId { get; set; }
            public Customer Customer { get; set; }

            public OrderDetails Details { get; set; }
        }

        private class OrderDetails
        {
            public int Id { get; set; }

            public int OrderId { get; set; }
            public Order Order { get; set; }
        }
    }
}