namespace API_RNB.Dto
{
    public class ProdutoDtoOutput
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Referencia { get; set; }
        public decimal Preco_Venda { get; set; }
        public decimal Preco_Promocao { get; set; }
        public decimal Estoque { get; set; }
    }
}
