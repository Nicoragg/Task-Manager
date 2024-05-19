using Microsoft.EntityFrameworkCore; // Importa o namespace para usar o Entity Framework Core.

namespace MinimalApiProject.Models // Define o namespace do projeto.
{
    // Define a classe de contexto do banco de dados que herda de DbContext.
    public class AppDbContext : DbContext
    {
        // Define uma propriedade DbSet para a entidade Tarefa.
        public DbSet<Tarefa> Tarefas { get; set; }
        
        // Define uma propriedade DbSet para a entidade Usuario.
        public DbSet<Usuario> Usuarios { get; set; }
          
        // Método para configurar o contexto do banco de dados.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configura o uso do banco de dados SQLite com o arquivo "app.db".
            optionsBuilder.UseSqlite("Data Source=app.db");
        }


    }
}
