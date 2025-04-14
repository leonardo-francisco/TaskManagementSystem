using AutoMapper;
using FluentAssertions;
using MongoDB.Bson;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Requests;
using TaskManager.Application.Exception;
using TaskManager.Application.Services;
using TaskManager.Domain.Contracts;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Tests
{
    public class TaskServiceTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<ITaskHistoryRepository> _taskHistoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _taskHistoryRepositoryMock = new Mock<ITaskHistoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _projectService = new ProjectService(_projectRepositoryMock.Object, _taskRepositoryMock.Object, _mapperMock.Object);
            _taskService = new TaskService(_taskRepositoryMock.Object, _projectRepositoryMock.Object, _taskHistoryRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task UpdateTask_Should_ThrowException_When_TryingToChangePriority()
        {
            // Arrange
            var taskId = ObjectId.GenerateNewId();
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Tarefa Teste",
                Priority = ETaskPriority.High,
                Status = ETaskStatus.InProgress,
                CreatedBy = "Mock"
            };

            var request = new UpdateTaskRequest
            {
                Id = taskId.ToString(),
                Title = "Tarefa Alterada",
                Priority = "Low", 
                Status = "InProgress",
                UpdatedBy = "Leonardo"
            };

            _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId.ToString())).ReturnsAsync(task);

            // Act
            Func<Task> act = async () => await _taskService.UpdateAsync(request);

            // Assert
            await act.Should().ThrowAsync<CustomException>()
                .WithMessage("A prioridade da tarefa não pode ser alterada.");
        }

        [Fact]
        public async Task UpdateTask_Should_AddHistory_When_FieldsAreChanged()
        {
            // Arrange
            var taskId = ObjectId.GenerateNewId();
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Antigo título",
                Description = "Descrição antiga",
                Priority = ETaskPriority.Medium,
                Status = ETaskStatus.ToDo,
                AssignedUserName = "João",
                CreatedBy = "Mock",
                Comments = new List<Comment>() // Inicialmente sem comentários
            };

            var request = new UpdateTaskRequest
            {
                Id = taskId.ToString(),
                Title = "Novo título",
                Description = "Nova descrição",
                Status = "Done",
                AssignedUserName = "Maria",
                Comments = new List<CommentDto>
                {
                    new CommentDto("Novo comentário", "Leonardo", DateTime.UtcNow)
                },
                UpdatedBy = "Leonardo"
            };

            // Mock da task existente
            _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId.ToString())).ReturnsAsync(task);

            // Mock do update e do histórico
            _taskRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
            _taskHistoryRepositoryMock.Setup(r => r.AddHistoryAsync(It.IsAny<TaskHistory>())).Returns(Task.CompletedTask);

            // Act
            await _taskService.UpdateAsync(request);

            // Assert
            _taskHistoryRepositoryMock.Verify(r => r.AddHistoryAsync(It.IsAny<TaskHistory>()), Times.AtLeastOnce);
        }
    }
}
