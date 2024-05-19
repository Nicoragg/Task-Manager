using System; // Importa o namespace para funcionalidades básicas do .NET.
using System.Linq; // Importa o namespace para consultas LINQ.
using System.Threading; // Importa o namespace para funcionalidades de threading.
using System.Threading.Tasks; // Importa o namespace para funcionalidades de tarefas assíncronas.
using Microsoft.EntityFrameworkCore; // Importa o namespace para Entity Framework Core.
using Microsoft.Extensions.DependencyInjection; // Importa o namespace para injeção de dependência.
using Microsoft.Extensions.Hosting; // Importa o namespace para o serviço de hospedagem.
using MinimalApiProject.Models; // Importa o namespace para modelos do projeto.

public class NotificationBackgroundService : BackgroundService // Define um serviço em segundo plano que herda de BackgroundService.
{
    private readonly IServiceProvider _serviceProvider; // Define um campo para armazenar o provedor de serviços.

    public NotificationBackgroundService(IServiceProvider serviceProvider) // Construtor que recebe um provedor de serviços.
    {
        _serviceProvider = serviceProvider; // Inicializa o campo _serviceProvider.
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) // Método principal que será executado quando o serviço iniciar.
    {
        while (!stoppingToken.IsCancellationRequested) // Loop contínuo que roda até que uma solicitação de cancelamento seja recebida.
        {
            using (var scope = _serviceProvider.CreateScope()) // Cria um escopo de serviço.
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Obtém uma instância do AppDbContext.
                var now = DateTime.UtcNow; // Obtém a data e hora atual em UTC.

                // Verifica tarefas próximas do prazo
                var tasksToNotify = await dbContext.Tarefas
                    .Where(task => task.Prazo <= now.AddDays(1) && task.Prazo > now) // Filtra as tarefas cujo prazo é no próximo dia.
                    .Include(task => task.Atribuicoes!) // Inclui as atribuições de tarefas.
                        .ThenInclude(at => at.Usuario) // Inclui os usuários das atribuições.
                    .ToListAsync(); // Converte a consulta em uma lista.

                foreach (var task in tasksToNotify) // Itera sobre cada tarefa próxima do prazo.
                {
                    foreach (var atribuicao in task.Atribuicoes!) // Itera sobre cada atribuição da tarefa.
                    {
                        var notification = new Notificacao // Cria uma nova notificação.
                        {
                            UsuarioId = atribuicao.UsuarioId, // Define o ID do usuário.
                            Mensagem = $"A tarefa '{task.Titulo}' está próxima do prazo de conclusão.", // Define a mensagem da notificação.
                            DataCriacao = DateTime.UtcNow // Define a data de criação da notificação.
                        };

                        dbContext.Notificacoes.Add(notification); // Adiciona a notificação ao contexto.
                    }
                }

                // Verifica tarefas expiradas
                var expiredTasks = await dbContext.Tarefas
                    .Where(task => task.Prazo <= now) // Filtra as tarefas cujo prazo já expirou.
                    .Include(task => task.Atribuicoes!) // Inclui as atribuições de tarefas.
                        .ThenInclude(at => at.Usuario) // Inclui os usuários das atribuições.
                    .ToListAsync(); // Converte a consulta em uma lista.

                foreach (var task in expiredTasks) // Itera sobre cada tarefa expirada.
                {
                    foreach (var atribuicao in task.Atribuicoes!) // Itera sobre cada atribuição da tarefa.
                    {
                        var notification = new Notificacao // Cria uma nova notificação.
                        {
                            UsuarioId = atribuicao.UsuarioId, // Define o ID do usuário.
                            Mensagem = $"A tarefa '{task.Titulo}' expirou.", // Define a mensagem da notificação.
                            DataCriacao = DateTime.UtcNow // Define a data de criação da notificação.
                        };

                        dbContext.Notificacoes.Add(notification); // Adiciona a notificação ao contexto.
                    }
                }

                await dbContext.SaveChangesAsync(); // Salva as alterações no banco de dados.
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Espera uma hora antes de repetir o loop.
        }
    }
}


//Este serviço em segundo plano verifica periodicamente (a cada hora) se há tarefas
//próximas do prazo ou expiradas e cria notificações apropriadas para os usuários atribuídos.