using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IGraph: IAnnotatable {

        IEnumerable<IEntity> GetEntities();

        IEntity FindEntity([NotNull] string[] labels);

        IEnumerable<IRelationship> GetRelationships();

        IRelationship FindRelationship([NotNull] string[] labels);
    }
}