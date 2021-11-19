using Dapper;
using Microsoft.EntityFrameworkCore;
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
    public class JogoRepositorio : Repositorio<Jogo>, IJogoRepositorio
    {
        protected readonly VirtualContext Db;
        private readonly IConfiguration _configuration;
        private readonly string _connectionStrings;
        public JogoRepositorio(VirtualContext db, IConfiguration configuration) : base(db)
        {
            Db = db;
            _configuration = configuration;
            _connectionStrings = _configuration.GetSection("ConnectionStrings:DefaultConnectionMySql").Value;
        }

        public async Task AdicionarJogadorJogo(List<JogadorJogo> jogadores)
        {
            await Db.JogadoresJogo.AddRangeAsync(jogadores);
            await SaveChanges();
        }

        public async Task AtualizarJogadorJogo(JogadorJogo jogador)
        {
            Db.JogadoresJogo.Update(jogador);
            await SaveChanges();
        }

        public async Task<IList<Cidade>> ObterCidades()
        {
           return await Db.Cidades.ToListAsync();
        }

        public async Task<JogoQuery> ObterDetalhesJogo(int idJogo)
        {
            try
            {
                return await Db.Jogos.Where(r => r.Id == idJogo)
                .Include(r => r.Adversario)
                .Include(r => r.Cidade)
                .Include(r => r.Jogadores)
                .ThenInclude(r => r.Jogador)
                .Select(r => 
                new JogoQuery 
                {
                    IdJogo = r.Id,
                    IdAdversario = r.IdAdversario,
                    IdCidade = r.IdCidade,
                    Adversario = r.Adversario.Nome,
                    BairroTime = r.Adversario.Bairro,
                    Cidade = r.Cidade.Nome,
                    Bairro = r.Bairro,
                    Campo = r.Campo,
                    Hora = r.Hora,
                    Data = r.DataJogo,
                    DataJogoFormatada = r.DataJogo.ToString("dd/MM/yyyy"),
                    Gols = r.Gols,
                    GolsAdversario = r.GolsAdversario,
                    Resultado = r.Resultado,
                    Jogadores = r.Jogadores
                    .Select(r => new JogadorDto
                    { 
                        IdJogador = r.IdJogador,
                        Nome = r.Jogador.Nome, 
                        Posicao = r.Jogador.Posicao,
                        Gols = r.Gols
                    }).ToList()
                }).FirstOrDefaultAsync();
               
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task<IEnumerable<JogadorJogo>> ObterJogadores(int idJogo)
        {
            return await Db.JogadoresJogo
                .Where(r => r.IdJogo == idJogo).ToListAsync();
        }

        public async Task<IEnumerable<JogoQuery>> ObterJogos()
        {
            var query = new StringBuilder();
            query.Append("SELECT ");
            query.Append(" IdJogo, jogo.Bairro, jogo.Campo, jogo.Hora, jogo.DataJogo as Data, DATE_FORMAT(jogo.DataJogo, '%d/%m/%Y') DataJogoFormatada, ");
            query.Append(" (case jogo.Gols < 0  when 1  then NULL else jogo.Gols end) Gols,");
            query.Append(" (case jogo.GolsAdversario < 0  when 1  then NULL else jogo.GolsAdversario end) GolsAdversario, jogo.Resultado, ");
            query.Append(" cidade.Id as IdCidade, cidade.Nome as Cidade,  ");
            query.Append(" time.IdTime as IdAdversario, time.Nome as Adversario, time.Bairro as BairroTime ");
            query.Append(" FROM tb_Jogo jogo ");
            query.Append(" INNER JOIN tb_Cidade cidade ON cidade.Id = jogo.IdCidade ");
            query.Append(" INNER JOIN tb_Time time ON jogo.IdAdversario = time.IdTime ");
            query.Append(" ORDER BY jogo.DataJogo DESC");

            await using MySqlConnection conexao = new MySqlConnection(_connectionStrings);
            await conexao.OpenAsync();
            var result = await conexao.QueryAsync<JogoQuery>(query.ToString());

            return result;
        }

        public async Task<AgendaQuery> ObterAgenda(int ano)
        {
            try
            {
                var results = await Db.Jogos
                .Include(r => r.Adversario)
                .Include(r => r.Cidade)
                .Include(r => r.Jogadores)
                .ThenInclude(r => r.Jogador)
                .Where(r => r.DataJogo.Year == ano)
                .ToListAsync();

                var agenda = new AgendaQuery();

                agenda.Ano = ano;
                agenda.Vitorias = results.Where(r => r.Gols >= 0).Count(r => r.Gols > r.GolsAdversario);
                agenda.Empates = results.Where(r => r.Gols >= 0).Count(r => r.Gols == r.GolsAdversario);
                agenda.Derrotas = results.Where(r => r.Gols >= 0).Count(r => r.Gols < r.GolsAdversario);
                agenda.Marcados = results.Where(r => r.Gols >= 0).Select(r => r.Gols).Sum().Value;
                agenda.Sofridos = results.Where(r => r.GolsAdversario >= 0).Select(r => r.GolsAdversario).Sum().Value;


                agenda.Meses = results.GroupBy(r => r.DataJogo.Month).OrderBy(r => r.Key)
                .Select(r =>
                new
                {
                    Ano = ano,
                    Mes = r.Key,
                    Vitorias = r.Count(t => t.Gols > t.GolsAdversario),
                    Derrotas = r.Count(t => t.Gols < t.GolsAdversario),
                    Empates = r.Count(t => t.Gols == t.GolsAdversario),
                    Jogos = r.Select(r => new
                    {
                        Adversario = r.Adversario.Nome,
                        BairroTime = r.Adversario.Bairro,
                        Cidade = r.Cidade.Nome,
                        Bairro = r.Bairro,
                        Campo = r.Campo,
                        Hora = r.Hora,
                        DataJogo = r.DataJogo.ToString("dd/MM/yyyy"),
                        Gols = r.Gols < 0? null : r.Gols,
                        GolsAdversario = r.GolsAdversario < 0 ? null : r.GolsAdversario,
                        Resultado = r.Resultado,
                        Jogadores = r.Jogadores
                        .Select(r => new
                        {
                            Nome = r.Jogador.Nome,
                            Posicao = r.Jogador.Posicao,
                            Gols = r.Gols
                        }).ToList()
                    }).ToList()

                }).ToList();

                return agenda;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task RemoverJogador(JogadorJogo jogador)
        {
            Db.Remove<JogadorJogo>(jogador);
            await SaveChanges();
        }

        public async Task<JogadorJogo> ObterJogadorJogo(int idJogo, int idJogador)
        {
            return await Db.JogadoresJogo
                .Where(r => r.IdJogo == idJogo && r.IdJogador == idJogador)
                .FirstOrDefaultAsync();
        }

        public async Task<HistoricoConfrontosQuery> HistoricoConfrontos(int idAdversario, int ano)
        {
            try
            {
                var historico = new HistoricoConfrontosQuery();
                await using MySqlConnection conexao = new MySqlConnection(_connectionStrings);
                await conexao.OpenAsync();
                int? anoEscolhido = ano == 0 ? null : ano; 
                var jogos = await conexao.QueryAsync<JogoDetalhesQuery>(QueryConfrontos(), new { IdAdversario = idAdversario, Ano = anoEscolhido });

                historico.Jogos = jogos.ToList();

                var idsJogos = jogos.Select(r => r.IdJogo);
                var jogadoresGols = await conexao.QueryAsync<JogadorQuery>(QueryJogadorGols(), new { IdsJogos = idsJogos });

                foreach (var jogo in jogos)
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

        public async Task<DashboardQuery> Dashboard()
        {
            try
            {              

                await using MySqlConnection conexao = new MySqlConnection(_connectionStrings);
                await conexao.OpenAsync();
                var jogos = await conexao.QueryAsync<DashboardJogosQuery>(QueryDashboardJogos());
                var artilheirosAno = await conexao.QueryAsync<DashboardArtilheiroQuery>(QueryDashboardArtilheirosAno());
                var artilheiros = await conexao.QueryAsync<DashboardArtilheiroQuery>(QueryDashboardArtilheiros());
                var maioresGoleadas = await conexao.QueryAsync<DashboardJogosQuery>(QueryDashboardMaioresGoledas());

                var historico = new DashboardQuery();
                historico.TotalVitorias = Db.Jogos.Count(r => r.Gols > r.GolsAdversario);
                historico.TotalJogos = Db.Jogos.Count();
                historico.TotalEmpates = Db.Jogos.Count(r => r.Gols == r.GolsAdversario);
                historico.TotalDerrotas = Db.Jogos.Count(r => r.Gols < r.GolsAdversario);
                historico.Artilheiros = artilheirosAno.ToList();
                historico.Agenda = jogos.ToList();
                historico.Marcados = Db.Jogos.Where(r => r.Gols >= 0).Select(r => r.Gols).Sum().Value;
                historico.Sofridos = Db.Jogos.Where(r => r.GolsAdversario >= 0).Select(r => r.GolsAdversario).Sum().Value;
                historico.MaioresArtilheiros = artilheiros.ToList();
                historico.MaioresGoleadas = maioresGoleadas.ToList();

                return historico;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private static string QueryConfrontos()
        {
            return @"
                SELECT 
	                j.IdJogo, j.IdAdversario, 
                    DATE_FORMAT(j.DataJogo, '%d/%m/%Y') AS Data, 
                    j.Campo, 
                    j.Hora,  
                    Resultado, 
                    j.Bairro, 
	                c.nome AS Cidade, 
                    t.nome AS Adversario,
                    (case j.Gols < 0  when 1  then NULL else j.Gols end) Gols,
                    (case j.GolsAdversario < 0  when 1  then NULL else j.GolsAdversario end) GolsAdversario
                       
                FROM db_virtual_clube.tb_Jogo j 
	                INNER JOIN db_virtual_clube.tb_Cidade c ON c.id = j.IdCidade
	                INNER JOIN db_virtual_clube.tb_Time t ON t.IdTime = j.IdAdversario
                WHERE j.IdAdversario = @IdAdversario AND (YEAR(j.DataJogo) = @Ano OR @ano IS NULL)
                ORDER BY j.DataJogo;";
        }

        private static string QueryJogadorGols()
        {
            return @"
                SELECT jj.IdJogador, jj.IdJogo, jo.Nome, jo.Posicao, jj.Gols FROM db_virtual_clube.tb_JogadorJogo jj
                INNER JOIN db_virtual_clube.tb_Jogador jo ON jo.IdJogador = jj.IdJogador
                where IdJogo IN @IdsJogos;
                ";
        }
                
        private static string QueryDashboardJogos()
        {
            return @"
                SELECT 
	                j.Idjogo, DATE_FORMAT(j.DataJogo, '%d/%m') AS Data, j.Campo, j.Hora, 
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

        private static string QueryDashboardArtilheirosAno()
        {
            return @" SELECT  jo.Nome, jo.Posicao,jo.Sobrenome, SUM(jj.Gols) AS Gols 
                    FROM db_virtual_clube.tb_JogadorJogo jj  
                    INNER JOIN db_virtual_clube.tb_Jogo jg ON jj.IdJogo = jg.IdJogo  
                    LEFT JOIN db_virtual_clube.tb_Jogador jo ON jj.IdJogador = jo.IdJogador  
                    WHERE YEAR(jg.DataJogo) = EXTRACT(YEAR FROM CURDATE())
                    GROUP BY jo.IdJogador ORDER BY Gols DESC;";
        }

        private static string QueryDashboardArtilheiros()
        {
            return @"SELECT  jo.Nome, jo.Posicao,jo.Sobrenome, SUM(jj.Gols) AS Gols  
                        FROM db_virtual_clube.tb_JogadorJogo jj  
                        INNER JOIN db_virtual_clube.tb_Jogo jg ON jj.IdJogo = jg.IdJogo  
                        LEFT JOIN db_virtual_clube.tb_Jogador jo ON jj.IdJogador = jo.IdJogador  
                        GROUP BY jo.IdJogador ORDER BY Gols DESC;";
        }

        private static string QueryDashboardMaioresGoledas()
        {
            return @"SELECT DATE_FORMAT(j.DataJogo, '%d/%m/%Y') AS Data, j.Campo,
		                (case j.Gols < 0  when 1  then NULL else j.Gols end) Gols,
		                (case j.GolsAdversario < 0  when 1  then NULL else j.GolsAdversario end) GolsAdversario, 
                        j.Bairro, c.nome AS Cidade, t.nome AS Adversario, t.cidade, t.bairro
	                FROM db_virtual_clube.tb_Jogo j 
		            INNER JOIN db_virtual_clube.tb_Cidade c ON c.id = j.IdCidade
		            INNER JOIN db_virtual_clube.tb_Time t ON t.IdTime = j.IdAdversario
                    WHERE j.Gols > j.GolsAdversario
                    ORDER BY   j.Gols DESC   
                    LIMIT 10;";
        }
    }
}
