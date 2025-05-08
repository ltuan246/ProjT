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
    /// JoinRowBlock.
    /// </summary>
    BlockExpression JoinRowBlock { get; }
}