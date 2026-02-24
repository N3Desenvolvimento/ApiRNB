using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using API_RNB.Dto;
using API_RNB.Models;
using API_RNB.Repository;

namespace API_RNB.Services
{
    public class VendasHookWorker : BackgroundService
    {
        private readonly ILogger<VendasHookWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Channel<VendaHookModel> _channel;
        private const int MaxConcurrency = 5;

        public VendasHookWorker(
            ILogger<VendasHookWorker> logger,
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory
        )
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;

            var options = new BoundedChannelOptions(100) { FullMode = BoundedChannelFullMode.Wait };
            _channel = Channel.CreateBounded<VendaHookModel>(options);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Vendas Hook Service iniciado com arquitetura de alta performance."
            );

            // Inicia consumidores concorrentes
            var consumers = new List<Task>();
            for (int i = 0; i < MaxConcurrency; i++)
            {
                consumers.Add(ProcessarFilaAsync(stoppingToken));
            }

            // Inicia produtor
            var producer = ProduzirVendasAsync(stoppingToken);

            await Task.WhenAll(producer, Task.WhenAll(consumers));
        }

        private async Task ProduzirVendasAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation(
                        $"[{DateTime.Now:HH:mm:ss}] Verificando novas vendas pendentes no banco..."
                    );

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var repository =
                            scope.ServiceProvider.GetRequiredService<VendasHookRepository>();
                        var vendasPendentes = await repository.GetPendingVendasHooksAsync();

                        int count = vendasPendentes?.Count() ?? 0;

                        if (count > 0)
                        {
                            _logger.LogInformation(
                                $"[{DateTime.Now:HH:mm:ss}] Encontradas {count} vendas para processar."
                            );

                            foreach (var venda in vendasPendentes)
                            {
                                await _channel.Writer.WriteAsync(venda, stoppingToken);
                            }
                        }
                        else
                        {
                            _logger.LogInformation(
                                $"[{DateTime.Now:HH:mm:ss}] Nenhuma venda pendente encontrada."
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no Produtor de Vendas.");
                }

                // Aguarda 30 segundos com feedback visual
                _logger.LogInformation($"[{DateTime.Now:HH:mm:ss}] Aguardando próximo ciclo...");
                for (int i = 0; i < 30; i++)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;
                    Console.Write($"\rAguardando: {i + 1}s / 30s");
                    await Task.Delay(1000, stoppingToken);
                }
                Console.WriteLine(); // Quebra linha após contagem
            }
        }

        private async Task ProcessarFilaAsync(CancellationToken stoppingToken)
        {
            await foreach (var venda in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var repository =
                            scope.ServiceProvider.GetRequiredService<VendasHookRepository>();
                        var webhookService =
                            scope.ServiceProvider.GetRequiredService<WebhookService>();
                        await ProcessarVenda(venda, repository, webhookService);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        $"Erro fatal ao processar item da fila: {venda.IdVenda_Hook}"
                    );
                }
            }
        }

        private async Task ProcessarVenda(
            VendaHookModel venda,
            VendasHookRepository repository,
            WebhookService webhookService
        )
        {
            var urlDestino = "https://hooknitro.phdvirtual.com.br/webhook/6997c2a66d4b235fdcb9cb93";

            // Monta o payload usando o WebhookService
            var payload = webhookService.CreatePayload(venda);

            // Prepara o DTO de histórico
            var historicoInput = new VendaHookHistoricoDtoInput
            {
                IdVenda_Hook = venda.IdVenda_Hook,
                Data_Hora_Envio = DateTime.Now,
                Url_Destino = urlDestino,
                Sucesso = "N",
            };

            try
            {
                // Envia o webhook usando o WebhookService
                var (success, statusCode, responseBody, jsonEnviado) =
                    await webhookService.SendWebhookAsync(urlDestino, payload);

                // Atualiza o histórico com os dados do envio
                historicoInput.Payload_Enviado = jsonEnviado;
                historicoInput.Http_Status_Code = statusCode;
                historicoInput.Resposta_Api = responseBody;

                if (success)
                {
                    historicoInput.Sucesso = "S";
                    var successInput = new VendaHookStatusDtoInput
                    {
                        Id = venda.IdVenda_Hook,
                        Status = "enviado_com_sucesso",
                    };
                    await repository.UpdateVendaHookStatusAsync(successInput);
                    _logger.LogInformation($"Venda id: {venda.IdVenda_Hook} enviada com sucesso.");
                }
                else
                {
                    historicoInput.Sucesso = "N";
                    await TratarFalha(venda, repository, $"Status: {statusCode} - {responseBody}");
                }
            }
            catch (Exception ex)
            {
                historicoInput.Sucesso = "N";
                historicoInput.Resposta_Api = $"Erro interno: {ex.Message}";
                // Em caso de erro interno, o payload pode não ter sido gerado se falhou antes,
                // mas aqui já temos o payload objeto, vamos serializar só pra garantir o log se jsonEnviado estiver vazio
                if (string.IsNullOrEmpty(historicoInput.Payload_Enviado))
                {
                    historicoInput.Payload_Enviado = JsonSerializer.Serialize(payload);
                }

                await TratarFalha(venda, repository, ex.Message);
            }
            finally
            {
                try
                {
                    await repository.InsertVendaHookHistoricoAsync(historicoInput);
                }
                catch (Exception exHist)
                {
                    _logger.LogError(exHist, "Erro ao gravar histórico de envio.");
                }
            }
        }

        private async Task TratarFalha(
            VendaHookModel venda,
            VendasHookRepository repository,
            string erro
        )
        {
            venda.Tentativas++;

            if (venda.Tentativas >= 3)
            {
                // Se atingiu 3 tentativas, marca como falha definitiva
                // Importante: Passamos o status 'falha_envio' E atualizamos o contador de tentativas internamente no update se necessário
                // Mas o método UpdateVendaHookStatusAsync atual não atualiza TENTATIVAS, apenas STATUS e LOG_ERRO.
                // Precisamos garantir que TENTATIVAS seja atualizado para 3 no banco também.

                var tentativaInput = new VendaHookTentativaDtoInput
                {
                    Id = venda.IdVenda_Hook,
                    Tentativas = venda.Tentativas,
                    Erro = erro,
                };
                await repository.IncrementTentativasAsync(tentativaInput); // Atualiza para 3

                var failInput = new VendaHookStatusDtoInput
                {
                    Id = venda.IdVenda_Hook,
                    Status = "falha_envio",
                    Erro = erro,
                };
                await repository.UpdateVendaHookStatusAsync(failInput); // Marca como falha

                _logger.LogError(
                    $"Venda {venda.IdVenda_Hook} falhou definitivamente após {venda.Tentativas} tentativas."
                );
            }
            else
            {
                // Apenas incrementa o contador de tentativas e mantém como pendente
                // O worker pegará novamente no próximo ciclo (daqui a 30s)
                var tentativaInput = new VendaHookTentativaDtoInput
                {
                    Id = venda.IdVenda_Hook,
                    Tentativas = venda.Tentativas,
                    Erro = erro,
                };
                await repository.IncrementTentativasAsync(tentativaInput);
                _logger.LogWarning(
                    $"Venda {venda.IdVenda_Hook} falhou. Tentativa {venda.Tentativas} de 3."
                );
            }
        }
    }
}
