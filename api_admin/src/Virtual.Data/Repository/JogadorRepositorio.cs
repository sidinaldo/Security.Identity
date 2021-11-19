using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virtual.Data.Context;
using Virtual.Domain.DTO.Jogadores;
using Virtual.Domain.Interfaces.Repositories;
using Virtual.Domain.Models;
using Virtual.Domain.Queries;

namespace Virtual.Data.Repository
{
    public class JogadorRepositorio : Repositorio<Jogador>, IJogadorRepositorio
    {
        protected readonly VirtualContext Db;
        private readonly IConfiguration _configuration;
        private readonly string _connectionStrings;
        public JogadorRepositorio(VirtualContext db, IConfiguration configuration) : base(db)
        {
            Db = db;
            _configuration = configuration;
            _connectionStrings = _configuration.GetSection("ConnectionStrings:DefaultConnectionMySql").Value;
        }

        public async Task<HistoricoJogadorQuery> HistoricoJogador(int idJogador, int? ano)
        {
            try
            {
                var historico = new HistoricoJogadorQuery();
                await using MySqlConnection conexao = new MySqlConnection(_connectionStrings);
                await conexao.OpenAsync();
                var jogos = await conexao.QueryAsync<HistoricoJogadorJogoQuery>(QueryJogador(), new { IdJogador = idJogador });

                historico.HistoricoJogadorJogos = ano > 0 ? jogos.Where(j => j.Ano == ano).ToList() : jogos.ToList();
                historico.TotalGols = jogos.Any() ? jogos.First().TotalGols : 0;

                var idsJogos = jogos.Select(r => r.IdJogo);
                var jogadoresGols = await conexao.QueryAsync<JogadorQuery>(QueryJogadorGols(), new { IdsJogos = idsJogos });

                foreach (var jogo in historico.HistoricoJogadorJogos)
                {
                    jogo.Jogadores = jogadoresGols.Where(r => r.IdJogo == jogo.IdJogo).ToList();
                }

                return historico;
            }
            catch (Exception ex)
            {
                return null;
            }

           
        }

        private static string QueryJogador()
        {
            return @"
                SELECT 
                    jj.Gols AS GolsAno, DATE_FORMAT(j.DataJogo, '%d/%m/%Y') AS Data, YEAR(j.DataJogo) AS ANO, j.Campo, j.Hora, j.Gols, j.GolsAdversario, Resultado, j.Bairro, 
                    c.nome AS Cidade, jo.Nome, jo.Posicao, t.nome AS Adversario, jj.IdJogo, jj.IdJogador,
                    (SELECT SUM(Gols) FROM db_virtual_clube.tb_JogadorJogo WHERE IdJogador = @IdJogador) AS TotalGols
               FROM db_virtual_clube.tb_JogadorJogo jj 
                    INNER JOIN db_virtual_clube.tb_Jogo j ON jj.IdJogo = j.Idjogo
                    INNER JOIN db_virtual_clube.tb_Cidade c ON c.id = j.IdCidade
                    INNER JOIN db_virtual_clube.tb_Jogador jo ON jo.IdJogador = jj.IdJogador
                    INNER JOIN db_virtual_clube.tb_Time t ON t.IdTime = j.IdAdversario
                WHERE jj.IdJogador = @IdJogador
                ORDER BY j.DataJogo DESC;
                ";
        }

        private static string QueryJogadorGols()
        {
            return @"
                SELECT jj.IdJogador, jj.IdJogo, jo.Nome, jo.Posicao, jj.Gols 
                FROM db_virtual_clube.tb_JogadorJogo jj
                INNER JOIN db_virtual_clube.tb_Jogador jo ON jo.IdJogador = jj.IdJogador
                WHERE IdJogo IN @IdsJogos;
                ";
        }

        public async Task<IEnumerable<ArtilheiroQuery>> ObterArtilheiroAno(int ano)
        {
            try
            {

                await using MySqlConnection conexao = new MySqlConnection(_connectionStrings);
                await conexao.OpenAsync();
                var artilheiros = await conexao.QueryAsync<ArtilheiroQuery>(QueryArtilheiros(), new { Ano = ano });

                return artilheiros;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static string QueryArtilheiros()
        {
            return @" SELECT  jo.Nome, jo.Posicao,jo.Sobrenome, SUM(jj.Gols) AS Gols  
                    FROM db_virtual_clube.tb_JogadorJogo jj  
                    INNER JOIN db_virtual_clube.tb_Jogo jg ON jj.IdJogo = jg.IdJogo  
                    LEFT JOIN db_virtual_clube.tb_Jogador jo ON jj.IdJogador = jo.IdJogador  
                    WHERE YEAR(jg.DataJogo) = @Ano
                    GROUP BY jo.IdJogador ORDER BY Gols DESC;";
        }

    }
}
