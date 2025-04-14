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
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<ITaskHistoryRepository> _taskHistoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;

        public ProjectServiceTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _taskHistoryRepositoryMock = new Mock<ITaskHistoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _projectService = new ProjectService(_projectRepositoryMock.Object, _taskRepositoryMock.Object, _mapperMock.Object);
            _taskService = new TaskService(_taskRepositoryMock.Object, _projectRepositoryMock.Object, _taskHistoryRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task AddTaskToProject_Should_ThrowException_When_ProjectHasMaxTasks()
        {
            // Arrange
            var projectId = ObjectId.GenerateNewId();
          
            _taskRepositoryMock
                .Setup(r => r.GetByProjectIdAsync(projectId.ToString()))
                .ReturnsAsync(Enumerable.Range(0, 20).Select(i => new TaskItem
                {
                    Id = ObjectId.GenerateNewId(),
                    Title = $"Tarefa {i}",
                    CreatedBy = "Mock"
                }).ToList());

            var newTask = new CreateTaskRequest
            {
                Title = "Nova tarefa",
                Description = "Descrição",
                Priority = "High",
                ProjectId = projectId.ToString(),
                AssignedUserName = "Leonardo",
                CreatedBy = "Mock"
            };

            // Mock do mapper (se necessário)
            _mapperMock
                .Setup(m => m.Map<TaskItem>(It.IsAny<CreateTaskRequest>()))
                .Returns(new TaskItem
                {
                    Id = ObjectId.GenerateNewId(),
                    Title = newTask.Title,
                    Description = newTask.Description,
                    Priority = Domain.Enums.ETaskPriority.High,
                    CreatedBy = newTask.CreatedBy,
                    Comments = new List<Comment>() 
                });

            // Act
            var act = async () => await _taskService.CreateAsync(newTask);

            // Assert
            await act.Should().ThrowAsync<CustomException>()
                .WithMessage("Erro: O limite de 20 tarefas para este projeto foi atingido.");
        }

        [Fact]
        public async Task DeleteProject_Should_ThrowException_When_HasPendingTasks()
        {
            // Arrange
            var projectId = ObjectId.GenerateNewId(); 

            _taskRepositoryMock
                 .Setup(r => r.GetByProjectIdAsync(projectId.ToString()))
                 .ReturnsAsync(Enumerable.Range(0, 1).Select(i => new TaskItem
                 {
                     Id = ObjectId.GenerateNewId(),
                     Title = $"Tarefa {i}",
                     Description = "Tarefa com status InProgress",
                     Status = ETaskStatus.InProgress,
                     CreatedBy = "Mock"
                 }).ToList());

            _projectRepositoryMock
            .Setup(r => r.GetByIdAsync(projectId.ToString()))
            .ReturnsAsync(new Project
            {
                Id = projectId,
                Name = "Projeto de Teste",
                Description = "Descrição",
                CreatedBy = "Mock",
                Tasks = new List<TaskCreated>() 
            });

            // Act
            Func<Task> act = async () => await _projectService.DeleteAsync(projectId.ToString());

            // Assert
            await act.Should().ThrowAsync<CustomException>()
                .WithMessage("O projeto possui tarefas pendentes. Conclua ou remova as tarefas antes de excluir o projeto.");
        }
    }
}
