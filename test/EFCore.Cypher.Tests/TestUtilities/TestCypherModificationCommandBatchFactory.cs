// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Update;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestCypherModificationCommandBatchFactory : IModificationCommandBatchFactory
    {

        public TestCypherModificationCommandBatchFactory(
        ) {
        }

        /// <summary>
        /// Number craeted
        /// </summary>
        /// <returns></returns>
        public int CreateCount { get; private set; }

        /// <summary>
        /// Command batch
        /// </summary>
        /// <returns></returns>
        public ModificationCommandBatch Create()
        {
            CreateCount++;
            
            throw new NotImplementedException();
        }
    }
}
