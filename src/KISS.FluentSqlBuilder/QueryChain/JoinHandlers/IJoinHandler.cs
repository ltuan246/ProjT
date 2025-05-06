namespace KISS.FluentSqlBuilder.QueryChain.JoinHandlers;

/// <summary>
/// ISelectHandler.
/// </summary>
public interface IJoinHandler
{
    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    Type OutDictEntityType { get; }

    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    ParameterExpression OutDictEntityTypeExVariable { get; }

    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    ParameterExpression OutDictKeyExVariable { get; }

    /// <summary>
    /// JoinRowBlock.
    /// </summary>
    BlockExpression JoinRowBlock { get; }
}