using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API_RNB.Dto;
using API_RNB.Models;

namespace API_RNB.Services
{
    public class WebhookService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WebhookService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public WebhookPayloadDtoInput CreatePayload(VendaHookModel venda)
        {
            return new WebhookPayloadDtoInput
            {
                Cpf = venda.Cnpj_Cpf,
                Nome = venda.Nome_Pessoa,
                Email = venda.Email,
                Telefone = venda.Fones,
                DataDeNascimento = venda.Data_Aniversario?.ToString("yyyy-MM-dd"),
                ValorTotal = venda.Valor_Venda ?? 0,
                Produtos = venda.Produtos.Select(p => new WebhookProdutoDtoInput
                {
                    Name = p.Name,
                    Descricao = p.Descricao,
                    Valor = p.Valor
                }).ToList()
            };
        }

        public async Task<(bool Success, int StatusCode, string Response, string PayloadJson)> SendWebhookAsync(string url, WebhookPayloadDtoInput payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                return (response.IsSuccessStatusCode, (int)response.StatusCode, responseBody, json);
            }
            catch (Exception ex)
            {
                return (false, 0, $"Erro ao enviar webhook: {ex.Message}", json);
            }
        }
    }
}