using System.Globalization;

public static class ExtratoParser
{
    private static readonly CultureInfo CulturaBr = new("pt-BR");


    public static List<Lancamento> Parse(string conteudo)
    {
        var lancamentos = new List<Lancamento>();

         var linhas = conteudo.Split(
            '\n',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var linha in linhas)
        {
            var campos = linha.Split(';');
            if (campos.Length != 4)
                continue; // linha malformada — por ora ignora

            var data = DateTime.ParseExact(campos[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var descricao = campos[1];
            var valor = decimal.Parse(campos[2], NumberStyles.Number, CulturaBr);
            var tipo = campos[3].ToUpperInvariant() == "C"
                ? TipoLancamento.Credito
                : TipoLancamento.Debito;

            lancamentos.Add(new Lancamento(data, descricao, valor, tipo));
        }

        return lancamentos;
    }
}