
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

        /// <summary>
        /// Set labels (with generics)
        /// </summary>
        [Fact]
        public void Can_set_labels()
        {
            var modelBuilder = CreateConventionModelBuilder();

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
            var modelBuilder = CreateConventionModelBuilder();

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
        /// but empty then the entity's short name is used
        /// </summary>
        [Fact]
        public void Can_get_labels_negative() {
            var modelBuilder = CreateConventionModelBuilder();

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

        /// <summary>
        /// Set labels first via a data annotation
        /// then override with explicit
        /// </summary>
        [Fact]
        public void Can_set_labels_override() {
            var modelBuilder = CreateConventionModelBuilder();

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

        /// <summary>
        /// Inherit default label (the Customer entity 
        /// is registered thanks to foreign keys)
        /// </summary>
        [Fact]
        public void Can_inherit_default_labels() {
            var modelBuilder = CreateConventionModelBuilder();

            // labels from data annotation
            modelBuilder.Entity<International>();

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(International));

            var registeredThanksToForeignKeys = modelBuilder
                .Model
                .FindEntityType(typeof(Customer));

            Assert.NotNull(registeredThanksToForeignKeys);
            Assert.Equal("International", entityType.DisplayName());
            string[] labels = entityType.Cypher().Labels;
            Assert.True(new [] { "Customer", "International" }.SequenceEqual(labels));
        }

        /// <summary>
        /// If an entity type is ignored then labels are not inherited
        /// </summary>
        [Fact]
        public void Inheritance_labels_when_ignored() { 
            var modelBuilder = CreateConventionModelBuilder();

            modelBuilder.Entity<A>();
            modelBuilder.Ignore<B>();
            modelBuilder.Entity<C>();

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(C));

            Assert.Equal("C", entityType.DisplayName());
            string[] labels = entityType.Cypher().Labels;
            Assert.True(new [] { "Public", "C" }.SequenceEqual(labels));

            modelBuilder.Entity<B>();
            Assert.True(
                new [] { "Public", "Undercover", "C" }
                    .SequenceEqual(entityType.Cypher().Labels)
            );
        }

        /// <summary>
        /// If an entity type is not registered then labels are not inherited
        /// </summary>
        [Fact]
        public void Inheritance_labels_when_not_registered() { 
            var modelBuilder = CreateConventionModelBuilder();

            modelBuilder.Entity<A>();
            // base types are only available if the entiy is registered
            modelBuilder.Entity<C>();

            var entityType = modelBuilder
                .Model
                .FindEntityType(typeof(C));
            
            var notRegistered = modelBuilder
                .Model
                .FindEntityType(typeof(B));

            Assert.Null(notRegistered);
            Assert.Equal("C", entityType.DisplayName());
            string[] labels = entityType.Cypher().Labels;
            Assert.True(new [] { "Public", "C" }.SequenceEqual(labels));
        }

        /// <summary>
        /// The entity starting relationship builder is the 
        /// starting endpoint in the underlying relationship
        /// </summary>
        [Fact]
        public void Can_set_relationship_overriding_annotations() {
            var modelBuilder = CreateConventionModelBuilder();

            modelBuilder.Entity<Owner>()
                .HasMany(e => e.Things)
                .HasRelationship("Mine")
                .WithOne(e => e.TheMan);

            var owner = modelBuilder
                .Model
                .FindEntityType(typeof(Owner));

            // Stuff will always be the declaring entity (note: 
            // the direction of the foreign key can never be 
            // inverted explicitly with the relation builders)
            var stuff = modelBuilder
                .Model
                .FindEntityType(typeof(Stuff));

            IForeignKey fk =  stuff
                .GetForeignKeys()
                .First();

            Assert.Equal(stuff, fk.DeclaringEntityType);
            Assert.Equal(owner, fk.PrincipalEntityType);

            ICypherRelationship rel = fk
                .Cypher()
                .Relationship;

            Assert.Equal("Mine", rel.Relation.Name);
            Assert.Equal(owner, rel.Starting);
            Assert.Equal(stuff, rel.Ending);

            // Find the same relationships through the model
            rel = modelBuilder
                .Model
                .Starting(owner.ClrType)
                .Single();

            Assert.Equal("Mine", rel.Relation.Name);
            Assert.Null(rel.Relation.ClrType);
            Assert.Equal(owner, rel.Starting);
            Assert.Equal(stuff, rel.Ending);
        }

        /// <summary>
        /// Data annotations 
        /// </summary>
        [Fact]
        public void Can_set_relationship_with_annotations() {
            var modelBuilder = CreateConventionModelBuilder();

            modelBuilder.Entity<Owner>();

            var stuff = modelBuilder
                .Model
                .FindEntityType(typeof(Stuff));

            var owner = modelBuilder
                .Model
                .FindEntityType(typeof(Owner));

            var beforehand = stuff
                .GetForeignKeys()
                .First()
                .Cypher()
                .Relationship;

            Assert.Equal("Not Mine", beforehand.Relation.Name);
            Assert.Equal(owner, beforehand.Starting);

            // change the direction and name of the relationship
            modelBuilder.Entity<Stuff>()
                .HasOne(e => e.TheMan)
                .HasRelationship("Mine")
                .WithMany(e => e.Things);

            var afterwards = stuff
                .GetForeignKeys()
                .First()
                .Cypher()
                .Relationship;

            Assert.Equal("Mine", afterwards.Relation.Name);
            Assert.Equal(stuff, afterwards.Starting);
        }

        /// <summary>
        /// Fail if the starting entity is not a member of the 
        /// foreign key
        /// </summary>
        [Fact]
        public void Fail_if_starting_is_not_a_member() {
            Assert.Throws<InvalidOperationException>(
                () => {
                var modelBuilder = CreateConventionModelBuilder();

                modelBuilder.Entity<A>();

                modelBuilder.Entity<Owner>()
                    .HasMany(e => e.Things)
                    .HasRelationship("What", typeof(A))
                    .WithOne();
            });
        }

        /// <summary>
        /// Fail if the starting entity is not registered
        /// </summary>
        [Fact]
        public void Fail_if_starting_is_not_registered() {
            Assert.Throws<ArgumentNullException>(
                () => {
                var modelBuilder = CreateConventionModelBuilder();

                modelBuilder.Entity<Owner>()
                    .HasMany(e => e.Things)
                    .HasRelationship("What", typeof(A))
                    .WithOne();
            });
        }

        /// <summary>
        /// Fail if both navigations endpoints on a foreign 
        /// key have the a relationship attribute
        /// </summary>
        [Fact]
        public void Fail_if_data_annotations_on_both_endpoints() {
            Assert.Throws<InvalidOperationException>(
                () => {
                var modelBuilder = CreateConventionModelBuilder();

                modelBuilder.Entity<Gary>();
            });
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

            [Relationship("Not Mine")]
            public IEnumerable<Stuff> Things { get; set; }
        }

        private class Stuff {
            public int Id { get; set; }

            public Owner TheMan { get; set; }
        }

        [Labels(new[] {"Public"})]
        private class A {
            public int TopClass { get; set; }
        }

        [Labels(new[] {"Undercover"})]
        private class B: A {
            public string Secret { get; set; }
        }

        private class C: B {
            public string TheRealSecret { get; set; }
        }

        private class Gary {
            [Relationship("Wedding")]
            public Mitch BestMan { get; set; }
        }

        private class Mitch {
            [Relationship("Wedding")]
            public Gary TheOtherGuy { get; set; }
        }
    }
}