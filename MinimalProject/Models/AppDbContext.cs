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
        
        // Define uma propriedade DbSet para a entidade AtribuicaoTarefaUsuario.
        public DbSet<AtribuicaoTarefaUsuario> Atribuicoes { get; set; }
        
        // Define uma propriedade DbSet para a entidade Projeto.
        public DbSet<Projeto> Projetos { get; set; }
        
        // Define uma propriedade DbSet para a entidade Notificacao.
        public DbSet<Notificacao> Notificacoes { get; set; }
        
        // Método para configurar o contexto do banco de dados.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configura o uso do banco de dados SQLite com o arquivo "app.db".
            optionsBuilder.UseSqlite("Data Source=app.db");
        }

        // Método para configurar a modelagem do banco de dados.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura a chave primária composta para a entidade AtribuicaoTarefaUsuario.
            modelBuilder.Entity<AtribuicaoTarefaUsuario>()
                .HasKey(atu => new { atu.TarefaId, atu.UsuarioId });

            // Configura o relacionamento entre AtribuicaoTarefaUsuario e Tarefa.
            modelBuilder.Entity<AtribuicaoTarefaUsuario>()
                .HasOne(atu => atu.Tarefa)
                .WithMany(t => t.Atribuicoes)
                .HasForeignKey(atu => atu.TarefaId);

            // Configura o relacionamento entre AtribuicaoTarefaUsuario e Usuario.
            modelBuilder.Entity<AtribuicaoTarefaUsuario>()
                .HasOne(atu => atu.Usuario)
                .WithMany(u => u.Atribuicoes)
                .HasForeignKey(atu => atu.UsuarioId);
        }
    }
}
