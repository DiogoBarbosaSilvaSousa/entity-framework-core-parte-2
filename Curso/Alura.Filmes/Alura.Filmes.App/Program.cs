using Alura.Filmes.App.Dados;
using Alura.Filmes.App.Extensions;
using Alura.Filmes.App.Negocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Alura.Filmes.App
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.LogSQLToConsole();

                var sql = "INSERT INTO language (name) VALUES ('Teste 1'), ('Teste 2'), ('Teste 3')";
                var registros = contexto.Database.ExecuteSqlCommand(sql);
                System.Console.WriteLine($"O total de registros afetados é {registros}.");

                var deleteSql = "DELETE FROM language WHERE name LIKE 'Teste%'";
                registros = contexto.Database.ExecuteSqlCommand(deleteSql);
                System.Console.WriteLine($"O total de registros afetados é {registros}");
            }
        }

        private static void StoredProcedure(AluraFilmesContexto contexto)
        {
            var categ = "Action"; //36
            var paramCateg = new SqlParameter("category_name", categ);

            var paramTotal = new SqlParameter
            {
                ParameterName = "@total_actors",
                Size = 4,
                Direction = System.Data.ParameterDirection.Output
            };

            contexto.Database.ExecuteSqlCommand("total_actors_from_given_category @category_name, @total_actors OUT", paramCateg, paramTotal);

            System.Console.WriteLine($"O total de atores na categoria {categ} é de {paramTotal.Value}");
        }

        private static void InserindoSQLnaConsulta(AluraFilmesContexto contexto)
        {
            //var sql = @"select a.* from actor a inner join
            //             (select top 5 a.actor_id, count(*) as total 
            //                    from actor a 
            //                  inner join film_actor fa 
            //                      on fa.actor_id = a.actor_id 
            //                group by a.actor_id
            //                order by total desc) filmes on filmes.actor_id = a.actor_id";

            var sql = @"select a.* from actor a inner join
                              top5_most_starred_actors filmes on filmes.actor_id = a.actor_id";

            var atoresMaisAtuantes = contexto.Atores.FromSql(sql).Include(a => a.Filmografia);

            //var atoresMaisAtuantes = contexto.Atores
            //      .Include(a => a.Filmografia)
            //      .OrderByDescending(a => a.Filmografia.Count)
            //      .Take(5);

            foreach (var ator in atoresMaisAtuantes)
            {
                System.Console.WriteLine($"O ator {ator.PrimeiroNome} {ator.UltimoNome} atuou em {ator.Filmografia.Count} filmes.");
            }
        }

        private static void FuncionariosClientes(AluraFilmesContexto contexto)
        {
            Console.WriteLine("Clientes: ");
            foreach (var cliente in contexto.Clientes)
            {
                Console.WriteLine(cliente);
            }

            Console.WriteLine("Funcionários:");
            foreach (var func in contexto.Funcionarios)
            {
                Console.WriteLine(func);
            }
        }

        private static void InserirFilme(AluraFilmesContexto contexto)
        {
            var filme = new Filme();
            filme.Titulo = "Cassino Royale";
            filme.Duracao = 120;
            filme.AnoLancamento = "2000";
            filme.Classificacao = ClassificacaoIndicativa.MaioresQue14;
            filme.IdiomaFalado = contexto.Idiomas.First();
            contexto.Entry(filme).Property("last_update").CurrentValue = DateTime.Now;

            contexto.Filmes.Add(filme);
            contexto.SaveChanges();

            var filmeInserido = contexto.Filmes.First(f => f.Titulo == "Cassino Royale");
            Console.WriteLine(filmeInserido.Classificacao);
        }

        private static void InserirAtorDuplicado(AluraFilmesContexto contexto)
        {
            var ator1 = new Ator { PrimeiroNome = "Emma", UltimoNome = "Watson" };
            var ator2 = new Ator { PrimeiroNome = "Emma", UltimoNome = "Watson" };
            contexto.Atores.AddRange(ator1, ator2);
            contexto.SaveChanges();

            var emmaWatson = contexto.Atores.Where(a => a.PrimeiroNome == "Emma" && a.UltimoNome == "Watson");
            Console.WriteLine($"Total de atores encontrados: {emmaWatson.Count()}.");
        }
    }
}
