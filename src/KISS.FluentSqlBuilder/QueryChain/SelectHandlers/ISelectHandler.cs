namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
/// ISelectHandler.
/// </summary>
public interface ISelectHandler
{
    /// <summary>
    /// CurrentEntityExVariable.
    /// </summary>
    Type InEntityType { get; }

    /// <summary>
    /// CurrentEntityExVariable.
    /// </summary>
    Type OutEntityType { get; }

    /// <summary>
    /// CurrentEntityExVariable.
    /// </summary>
    ParameterExpression CurrentEntityExVariable { get; }

    /// <summary>
    /// CurrentEntityExVariable.
    /// </summary>
    ParameterExpression OutEntitiesExVariable { get; }

    /// <summary>
    /// InitializeEntityIfKeyMissing.
    /// </summary>
    ConditionalExpression InitializeEntityIfKeyMissing { get; }
}