using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public interface ICypherCommand {

        string CommandText { get; }

        IReadOnlyList<ICypherParameter> Parameters { get; }
    }
}