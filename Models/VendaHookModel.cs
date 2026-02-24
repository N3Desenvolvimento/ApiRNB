using System;
using System.Collections.Generic;

namespace API_RNB.Models
{
    public class VendaHookModel
    {
        public int IdVenda_Hook { get; set; }
        public int IdVenda { get; set; }
        public DateTime Data_Venda { get; set; }
        public TimeSpan Hora_Venda { get; set; }
        public decimal? Valor_Venda { get; set; }
        public int IdLoja { get; set; }
        public string Nome_Pessoa { get; set; }
        public string? Cnpj_Cpf { get; set; }
        public string? Fones { get; set; }
        public string? Email { get; set; }
        public DateTime? Data_Aniversario { get; set; }
        public string Enviado { get; set; }
        public DateTime? Data_Envio { get; set; }
        public string Status { get; set; } // pendente, enviando, enviado_com_sucesso, falha_envio
        public int Tentativas { get; set; }
        public string? Log_Erro { get; set; }

        public List<VendaItemModel> Produtos { get; set; } = new List<VendaItemModel>();
    }
}