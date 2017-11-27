# EFCore.Cypher
This project is very much a work in process.  Currently, the aim is to get the basic building blocks in place and over the next couple of weeks some rudimentary unit testing.  The Entity Framework's core is a labyrinth of factories and interfaces with a deep love of dependencies objects.  EF for Cypher rides on the relational side of the framework.  The [open cypher project](http://www.opencypher.org/) aims to be the defacto query language standard for the graph world.  Cypher is very similar to SQL so many of the moving parts from the relational leg of EF can be revamped to work with metadata for graphs.

At a high level, the relational framework takes the fluent Linq call expressions and mangles them into a query language wrapped inside a database command.  Relinq is used to unravel the Linq calls into a query model.  The model despite being riddled with SQL jargoon can be extended (i.e. new node types) to describe the way reading clauses in Cypher are structured (i.e. a Join is just the vertex between an entity and a relationship).  The Relinq model lets us find the nodes that are involved in the stream (Queryable) and references to them (projections, predicates, etc.).  The model can be visted just like Linq expressions.  There is a visitor (CypherQueryModelVisitor) that walks the Relinq model yielding a collection of Cypher Read Only expressions.  A Read Only expression is similar to a select statement made up of one or more reading clauses and a return clause.  The Read Only expression, just like the corresponding select expression in the relational framework, has a method to grab the default query generator.  As the visitor walks the model the Read Only expression is filled and when compiled the query generator is baked into an expression.  The generator is what turns the Read Only expression into Cypher.  The whole process is massive collection of factories making expression visitors which turn Linq method calls into something that is pretty close to the Cypher grammer.

# Short-list of what is near done

## The model
Everything starts with the database model.  There are two additions to the relational extensions: labels and relationships.  Rather, than using the **table** attribute a **labels** attribute is used to categorize entity types (i.e. nodes).  The foreign key connecting entity types may be enriched with relationship metadata.  One of the navigation endpoints in a foreign key can start a relationship that is either a shadow entity (i.e. just a name) or defined from a Clr type (i.e. an entity that can have properties).

## Infrastructure to create a database context
There is just enough infrastructure (i.e. services) to be able to create a database context and start fleshing out the Relinq logic necessary to process Cyppher matches and implicit group statements.

## Join extensions
The **Join** call has two new derivations.  A relationship by name (label) becomes the default key selector for both the outer and inner streams.  A relationship as a stream turns the Join extension into two Join calls with the first between the outer and relationship then the resulting type and inner doing some expression replacing to fix the body of the result selector.

## Query Cypher generation with simple clauses
The Relinq visitor (_CypherQueryModelVisitor_) converts the relinq source, pipes (bodies) and sinks into _ReadOnlyExpression_ that the default cypher generator can turn into cypher.  The _AsyncSimpleQueryCypherTest_ has test cases that evaulate simple clauses to cypher.  Here are a couple examples:

 * ```ctx.<Entity>.AsCypher()``` => "MATCH (e:Entity) RETURN <properties>"
 * ```ctx.<Entity>.Select(e => e).AsCypher()``` => "MATCH (e:Entity) RETURN <properties>"
 * ```ctx.<Entity>.Where(e => e.<Property> == 100).AsCypher()``` => "MATCH (e:Entity) WHERE e.Property = 100 RETURN <properties>"
