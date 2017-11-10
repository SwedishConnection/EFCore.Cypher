
// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
            modelBuilder.Entity<International>();

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
            Assert.NotEmpty(labels);
        }

        [Fact]
        public void Can_set_relationship_without_annotations() {
            // TODO: Replace with convention builder
            var modelBuilder = CreateModelBuilder();

            modelBuilder.Entity<Owner>()
                .HasMany(e => e.Things)
                .WithOne(e => e.TheMan)
                .HasRelationship("Mine");

            var owner = modelBuilder
                .Model
                .FindEntityType(typeof(Owner));

            var stuff = modelBuilder
                .Model
                .FindEntityType(typeof(Stuff));

            ICypherRelationship rel = stuff
                .GetForeignKeys()
                .First()
                .Cypher()
                .Relationship;

            Assert.Equal("Mine", rel.Relation.Name);
            Assert.True(rel.Starting == owner);
            Assert.True(rel.Ending == stuff);

            rel = modelBuilder
                .Model
                .Starting(owner.ClrType)
                .Single();

            Assert.Equal("Mine", rel.Relation.Name);
            Assert.True(rel.Starting == owner);
            Assert.True(rel.Ending == stuff);
        }

        private class Customer
        {
            public string Name { get; set; }
            public short SomeShort { get; set; }
            public MyEnum EnumValue { get; set; }

            [Relationship("USING")]
            public IEnumerable<Order> Orders { get; set; }

            [Relationship("HAS")]
            public Weird AWeird { get; set; }

            [Relationship("HAVE")]
            public Weird BWeird { get; set; }
        }

        private class International: Customer {
            [Relationship("HAVING")]
            public new Weird BWeird { get; set; }
        }

        private enum MyEnum : ulong
        {
            Sun,
            Mon,
            Tue
        }

        private class BaseType {

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

        private class Weird {
            public int Interval { get; set; }
        }

        private class Owner {
            public string Name { get; set; }

            [Relationship("Mine")]
            public IEnumerable<Stuff> Things { get; set; }
        }

        private class Stuff {
            public int Id { get; set; }

            public Owner TheMan { get; set; }
        }
    }
}