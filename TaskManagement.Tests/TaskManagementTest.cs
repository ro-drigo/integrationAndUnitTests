using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Database;
using TaskManagement.Models;


namespace TaskManagement.Tests
{
    public class TaskManagementTest
    {
        [Theory(DisplayName = "Pass Data Tasks Returns if Task Is Late")]
        [InlineData(true, "2002-12-22T10:00:00", false)]
        [InlineData(false, "2999-12-22T10:00:00", true)]
        public void PassData_Tasks_ReturnsIfTaskIsLate(bool result, DateTime dueDate, bool isCompleted)
        {
            Tasks task = new Tasks();

            task.DueDate = dueDate;
            task.IsCompleted = isCompleted;


            Assert.Equal(result, TaskManagement.BusinessRules.Validations.IsTaskLate(task));
        }
    }

    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>  // cria uma instância da API para os testes
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();  // cria um cliente HTTP para fazer as requisições
        }

        // teste de integração para obter todas as tarefas
        [Fact]
        public async Task GetTasks_ReturnsOkResponse()
        {
            // Act
            var response = await _client.GetAsync("/api/tasks");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        // Teste de integração para criar uma nova tarefa
        [Fact]
        public async Task PostTask_CreatesNewTask()
        {
            // Arrange
            var newTask = new Tasks
            {
                Title = "Test Task",
                Description = "Task created for testing purposes"
            };

            // Cria o conteúdo da requisição (JSON)
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(newTask),
                System.Text.Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/tasks", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        // Teste de integração para atualizar uma tarefa
        [Fact]
        public async Task PutTask_UpdatesExistingTask()
        {
            // Arrange
            var existingTask = new Tasks
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description"
            };
            var postResponse = await _client.PostAsync("/api/tasks",
                new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(existingTask),
                    System.Text.Encoding.UTF8,
                    "application/json"));

            var taskId = 1;

            // atualiza a tarefa
            var updatedTask = new Tasks
            {
                Id = 1,
                Title = "Updated Title",
                Description = "Updated Description"
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(updatedTask),
                System.Text.Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PutAsync($"/api/tasks/{taskId}", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        // teste de integração para excluir uma tarefa
        [Fact]
        public async Task DeleteTask_RemovesTask()
        {
            // Arrange
            var task = new Tasks
            {
                Id = 1,
                Title = "Task to be deleted",
                Description = "This task will be deleted"
            };

            var postResponse = await _client.PostAsync("/api/tasks",
                new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(task),
                    System.Text.Encoding.UTF8,
                    "application/json"));

            var taskId = 1;

            // Act
            var response = await _client.DeleteAsync($"/api/tasks/{taskId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}