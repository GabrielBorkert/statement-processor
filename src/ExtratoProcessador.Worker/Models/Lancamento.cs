public record Lancamento(
    DateTime Data,
    string Descricao,
    decimal Valor,
    TipoLancamento Tipo
);