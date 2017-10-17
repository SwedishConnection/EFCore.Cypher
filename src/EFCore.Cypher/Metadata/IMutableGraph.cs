using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableGraph: IGraph, IMutableAnnotatable {

        IMutableEntity AddEntity([NotNull] string[] labels);

        IMutableEntity AddEntity([NotNull] string[] labels, [NotNull] string definingNavigationName, [NotNull] IMutableEntity definingType);

        IMutableEntity AddEntity([NotNull] Type clrType);

        IMutableEntity AddEntity([NotNull] Type clrType, [NotNull] string definingNavigationName, [NotNull] IMutableEntity definingType);

        IMutableRelationship AddRelationship([NotNull] string[] labels);

        IMutableRelationship AddRelationship([NotNull] Type clrType);

        new IMutableEntity FindEntity([NotNull] string[] labels);

        new IMutableRelationship FindRelationship([NotNull] string[] labels);

        IMutableNode RemoveEntity([NotNull] string[] labels);

        IMutableNode RemoveRelationship([NotNull] string[] labels);

        new IEnumerable<IMutableEntity> GetEntities();

        new IEnumerable<IMutableRelationship> GetRelationships();
    }
}