using System;

namespace API_RNB.Models
{
    public class VendaHookHistoricoModel
    {
        public int IdVenda_Hook { get; set; }
        public DateTime Data_Hora_Envio { get; set; }
        public string Url_Destino { get; set; }
        public string Payload_Enviado { get; set; }
        public int? Http_Status_Code { get; set; }
        public string Resposta_Api { get; set; }
        public string Sucesso { get; set; } // 'S' ou 'N'
    }
}