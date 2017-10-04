namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableRelationship: IRelationship, IMutableNode {

        new IMutableEntity Start { get; set; }

        new IMutableEntity End { get; set; }
    }
}