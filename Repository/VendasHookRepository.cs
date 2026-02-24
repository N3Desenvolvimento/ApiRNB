using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API_RNB.Conexao;
using API_RNB.Dto;
using API_RNB.Models;
using Dapper;

namespace API_RNB.Repository
{
    public class VendasHookRepository
    {
        private readonly FirebirdDatabase _db;

        public VendasHookRepository(FirebirdDatabase db)
        {
            _db = db;
        }

        public async Task<IEnumerable<VendaHookModel>> GetPendingVendasHooksAsync()
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();

                // 1. Busca as vendas pendentes
                var sqlVendas =
                    @"
                    SELECT * 
                    FROM VENDAS_HOOK 
                    WHERE (ENVIADO IS NULL OR ENVIADO = 'N')
                      AND (STATUS IS NULL OR STATUS = 'pendente' OR STATUS = 'falha_envio')
                      AND (TENTATIVAS IS NULL OR TENTATIVAS < 3)";

                var vendas = await connection.QueryAsync<VendaHookModel>(sqlVendas);
                var vendasLista = vendas.ToList();

                if (!vendasLista.Any())
                    return vendasLista;

                // 2. Busca os itens dessas vendas
                var idsVendas = vendasLista.Select(v => v.IdVenda).Distinct().ToList();

                // Firebird não suporta arrays no IN facilmente com Dapper sem expandir
                // Mas Dapper expande IEnumerable automaticamente no IN
                var sqlItens =
                    @"
                    SELECT 
                        VI.IDVENDA,
                        P.descricao_produto as Name,
                        VI.DESCRICAO_ITEM as Descricao,
                        VI.VALOR as Valor
                    FROM VENDAS_ITENS VI
                    LEFT JOIN produtos P ON VI.IDPRODUTO = P.IDPRODUTO
                    WHERE VI.IDVENDA IN @Ids";

                var itens = await connection.QueryAsync<dynamic>(sqlItens, new { Ids = idsVendas });

                // 3. Associa os itens às vendas
                foreach (var venda in vendasLista)
                {
                    var itensDaVenda = itens
                        .Where(i => i.IDVENDA == venda.IdVenda)
                        .Select(i => new VendaItemModel
                        {
                            Name = i.NAME,
                            Descricao = i.DESCRICAO,
                            Valor = (decimal)i.VALOR,
                        })
                        .ToList();

                    venda.Produtos = itensDaVenda;
                }

                return vendasLista;
            }
        }

        public async Task UpdateVendaHookStatusAsync(VendaHookStatusDtoInput input)
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                var sql =
                    @"
                    UPDATE VENDAS_HOOK 
                    SET STATUS = @Status,
                        DATA_ENVIO = CASE WHEN @Status = 'enviado_com_sucesso' THEN CURRENT_DATE ELSE DATA_ENVIO END,
                        ENVIADO = CASE WHEN @Status = 'enviado_com_sucesso' THEN 'S' ELSE ENVIADO END,
                        LOG_ERRO = @Erro,
                        TENTATIVAS = CASE WHEN @Tentativas IS NOT NULL THEN @Tentativas ELSE TENTATIVAS END
                    WHERE IDVENDA_HOOK = @Id";

                await connection.ExecuteAsync(sql, input);
            }
        }

        public async Task IncrementTentativasAsync(VendaHookTentativaDtoInput input)
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var sql =
                    @"
                        UPDATE VENDAS_HOOK 
                        SET TENTATIVAS = @Tentativas,
                            LOG_ERRO = @Erro,
                            STATUS = 'pendente'
                        WHERE IDVENDA_HOOK = @Id";

                await connection.ExecuteAsync(sql, input, transaction: transaction);
                transaction.Commit();
            }
        }

        public async Task<IEnumerable<VendaHookModel>> GetSentVendasHooksAsync()
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                var sql = @"SELECT * FROM VENDAS_HOOK WHERE ENVIADO = 'S' ORDER BY DATA_ENVIO DESC";
                var result = await connection.QueryAsync<VendaHookModel>(sql);
                return result;
            }
        }

        public async Task InsertVendaHookHistoricoAsync(VendaHookHistoricoDtoInput input)
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var sql =
                    @"
                        INSERT INTO VENDAS_HOOK_HISTORICO 
                        (IDHISTORICO, IDVENDA_HOOK, DATA_HORA_ENVIO, URL_DESTINO, PAYLOAD_ENVIADO, HTTP_STATUS_CODE, RESPOSTA_API, SUCESSO)
                        VALUES 
                        (GEN_ID(GEN_VENDAS_HOOK_HISTORICO_ID, 1), @IdVenda_Hook, @Data_Hora_Envio, @Url_Destino, @Payload_Enviado, @Http_Status_Code, @Resposta_Api, @Sucesso)";

                await connection.ExecuteAsync(sql, input, transaction: transaction);
                transaction.Commit();
            }
        }
    }
}
