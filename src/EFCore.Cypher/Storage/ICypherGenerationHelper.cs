using System.Text;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage
{

    public interface ICypherGenerationHelper {

        string StatementTerminator { get; }

        string BatchTerminator { get; }

        string GenerateParameterName([NotNull] string name);

        void GenerateParameterName([NotNull] StringBuilder builder, [NotNull] string name);

        string EscapeLiteral([NotNull] string literal);

        void EscapeLiteral([NotNull] StringBuilder builder, [NotNull] string literal);

        string EscapeIdentifier([NotNull] string identifier);

        void EscapeIdentifier([NotNull] StringBuilder builder, [NotNull] string identifier);

        string DelimitIdentifier([NotNull] string identifier);

        void DelimitIdentifier([NotNull] StringBuilder builder, [NotNull] string identifier);

        string DelimitIdentifier([NotNull] string name, [CanBeNull] string schema);

        void DelimitIdentifier([NotNull] StringBuilder builder, [NotNull] string name, [CanBeNull] string schema);
    }
}