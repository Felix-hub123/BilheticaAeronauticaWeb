namespace BilheticaAeronauticaWeb.Helper
{
    /// <summary>
    /// Classe que representa a resposta padrão das operações,
    /// indicando sucesso ou falha, mensagem descritiva e resultados opcionais.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Indica se a operação foi bem sucedida.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Mensagem descrevendo o resultado ou erro da operação.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Objeto que contém os resultados retornados pela operação,
        /// pode ser qualquer tipo de dado ou coleção.
        /// </summary>
        public object Results { get; set; }
    }
}