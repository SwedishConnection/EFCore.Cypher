using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public abstract class GraphTypeMapping
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeType"></param>
        /// <param name="clrType"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        protected GraphTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            [CanBeNull] DbType? dbType = null,
            int? size = null
        ) {
            StoreType = storeType;
            ClrType = clrType;
            DbType = dbType;
            Size = size;
        }

        public virtual string StoreType { get; }

        public virtual Type ClrType { get; }

        public virtual DbType? DbType { get; }

        public virtual int? Size { get; }

        protected virtual string GraphLiteralFormatString { get; } = "{0}";

        public virtual DbParameter CreateParameter(
            [NotNull] DbCommand command,
            [NotNull] string name,
            [CanBeNull] object value,
            bool? nullable = null
        ){
            var parameter = command.CreateParameter();
            // TODO: Direction?
            parameter.Value = value ?? DBNull.Value;

            if (nullable.HasValue) {
                parameter.IsNullable = nullable.Value;
            }

            if (DbType.HasValue) {
                parameter.DbType = DbType.Value;
            }

            if (Size.HasValue) {
                parameter.Size = Size.Value;
            }

            ConfigureParameter(parameter);

            return parameter;
        }

        protected virtual void ConfigureParameter([NotNull] DbParameter parameter)
        {
        }
    }
}