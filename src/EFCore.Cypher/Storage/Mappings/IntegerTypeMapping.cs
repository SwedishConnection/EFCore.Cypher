
using System;
using System.Data;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage {

    public class IntegerTypeMapping : GraphTypeMapping
    {
        /// <summary>
        /// Signed 32 Number
        /// </summary>
        /// <param name="storeType"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected IntegerTypeMapping(
            [NotNull] string storeType, 
            [CanBeNull] DbType? dbType = null, 
            int? size = null
        ) : base(storeType, typeof(int), dbType, size)
        {
        }
    }
}