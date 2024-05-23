namespace KISS.QueryBuilder.Component;

public abstract record FieldDefinition<TComponent, TField>
{
    public static implicit operator FieldDefinition<TComponent>(FieldDefinition<TComponent, TField> field)
    {
        return new UntypedFieldDefinitionAdapter<TComponent, TField>(field);
    }
}

public abstract record FieldDefinition<TComponent>
{
}

public record UntypedFieldDefinitionAdapter<TComponent, TField>(FieldDefinition<TComponent, TField> Field) : FieldDefinition<TComponent>
{

}