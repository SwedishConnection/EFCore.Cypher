using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableGraph: IGraph, IMutableAnnotatable {

        IMutableEntity AddEntity([NotNull] string[] labels);

        IMutableRelationship AddRelationship([NotNull] string[] labels);

        new IMutableEntity FindEntity([NotNull] string[] labels);

        new IMutableRelationship FindRelationship([NotNull] string[] labels);

        IMutableNode RemoveEntity([NotNull] string[] labels);

        IMutableNode RemoveRelationship([NotNull] string[] labels);

        new IEnumerable<IMutableEntity> GetEntities();

        new IEnumerable<IMutableRelationship> GetRelationships();
    }
}