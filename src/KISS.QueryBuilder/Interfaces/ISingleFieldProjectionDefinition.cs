namespace KISS.QueryBuilder.Interfaces;

public interface ISingleFieldProjectionDefinition : IQuerying
{
    (RenderedFieldDefinition field, bool isIncluding) FieldDefinition { get; }
}