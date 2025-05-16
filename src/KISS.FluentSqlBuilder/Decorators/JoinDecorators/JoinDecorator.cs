namespace KISS.FluentSqlBuilder.Decorators.JoinDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record JoinDecorator : QueryDecorator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JoinDecorator"/> class.
    /// </summary>
    /// <param name="inner">innerComposite.</param>
    public JoinDecorator(IComposite inner)
        : base(inner)
    {
        OutDictEntityTypeExVariable = Expression.Variable(typeof(Dictionary<,>).MakeGenericType([typeof(object), Inner.OutEntityType]), "OutDictEntityTypeExVariable");
        OutDictKeyExVariable = Expression.Variable(TypeUtils.ObjType, "OutDictKeyExVariable");
        OutDictKeyAccessorExVariable = Expression.Variable(Inner.OutEntityType, "OutDictKeyAccessorExVariable");
    }
}