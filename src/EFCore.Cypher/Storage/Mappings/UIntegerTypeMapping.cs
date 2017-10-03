
using System;
using System.Data;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage {

    public class UIntegerTypeMapping : GraphTypeMapping
    {
        /// <summary>
        /// Signed 32 Number
        /// </summary>
        /// <param name="storeType"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected UIntegerTypeMapping(
            [NotNull] string storeType, 
            [CanBeNull] DbType? dbType = null, 
            int? size = null
        ) : base(storeType, typeof(uint), dbType, size)
        {
        }
    }
}