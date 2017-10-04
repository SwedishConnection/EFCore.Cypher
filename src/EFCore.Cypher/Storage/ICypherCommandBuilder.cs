using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public interface ICypherCommandBuilder: IInfrastructure<IndentedStringBuilder> {
        ICypherParameterBuilder ParameterBuilder { get; }

        ICypherCommand Build();
    }
}