using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_RNB.Dto
{
    public class WebhookPayloadDtoInput
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("telefone")]
        public string Telefone { get; set; }

        [JsonPropertyName("dataDeNascimento")]
        public string DataDeNascimento { get; set; }

        [JsonPropertyName("valorTotal")]
        public decimal ValorTotal { get; set; }

        [JsonPropertyName("produtos")]
        public List<WebhookProdutoDtoInput> Produtos { get; set; }
    }

    public class WebhookProdutoDtoInput
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}