using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Virtual.Data.Context;
using Virtual.Domain.Interfaces.Repositories;
using Virtual.Domain.Queries;

namespace Virtual.Data.Repository
{
    public class MobileRepositorio: IMobileRepositorio
    {
        protected readonly VirtualContext Db;
        private readonly IConfiguration _configuration;
        private readonly string _connectionStrings;
        public MobileRepositorio(VirtualContext db, IConfiguration configuration)
        {
            Db = db;
            _configuration = configuration;
            _connectionStrings = _configuration.GetSection("ConnectionStrings:DefaultConnectionMySql").Value;
        }

        public async Task<IEnumerable<AgendaMobile>> ObterAgenda(int ano)
        {
            
            try
            {
                await using MySqlConnection conexao = new MySqlConnection(_connectionStrings);
                await conexao.OpenAsync();
                var jogos = await conexao.QueryAsync<AgendaMobile>(QueryDashboardJogos());


                return jogos.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private static string QueryDashboardJogos()
        {
            return @"
                SELECT 
	                j.IdJogo, DATE_FORMAT(j.DataJogo, '%d/%m') AS Data, j.Campo, j.Hora, 
                     (case j.Gols < 0  when 1  then NULL else j.Gols end) Gols,
                    (case j.GolsAdversario < 0  when 1  then NULL else j.GolsAdversario end) GolsAdversario, 
                    Resultado, j.Bairro, 
	                c.nome AS Cidade, t.nome AS Adversario, t.cidade
                FROM db_virtual_clube.tb_Jogo j 
	                INNER JOIN db_virtual_clube.tb_Cidade c ON c.id = j.IdCidade
	                INNER JOIN db_virtual_clube.tb_Time t ON t.IdTime = j.IdAdversario
                WHERE YEAR(j.DataJogo) = EXTRACT(YEAR FROM CURDATE())
                ORDER BY j.DataJogo DESC;";
        }

    }
}
