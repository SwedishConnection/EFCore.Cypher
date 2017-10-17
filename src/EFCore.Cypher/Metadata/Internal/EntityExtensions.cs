using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class EntityExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool HasDefiningNavigation([NotNull] this IEntity entity)
            => entity.DefiningType != null;
    }
}