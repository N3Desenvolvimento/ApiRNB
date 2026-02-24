namespace API_RNB.Models
{
    public class VendaItemModel
    {
        public string Name { get; set; } // Nome do produto (PRODUTO.DESCRICAO)
        public string Descricao { get; set; } // Descrição do item (VENDAS_ITENS.DESCRICAO_ITEM)
        public decimal Valor { get; set; } // Valor do item (VENDAS_ITENS.VALOR)
    }
}
