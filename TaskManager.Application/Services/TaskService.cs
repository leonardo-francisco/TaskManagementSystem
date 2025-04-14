using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Contracts;
using TaskManager.Application.DTOs.Requests;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Contracts;
using TaskManager.Domain.Entities;
using MongoDB.Bson;
using AutoMapper;
using TaskManager.Domain.Enums;
using TaskManager.Application.Exception;

namespace TaskManager.Application.Services
{
    public sealed class TaskService(ITaskRepository repository, 
                                    IProjectRepository projectRepository,
                                    ITaskHistoryRepository taskHistoryRepository,
                                    IMapper mapper) : ITaskService
    {
        public async Task<string> CreateAsync(CreateTaskRequest request)
        {
            var now = DateTime.UtcNow;
            var initialComment = new Comment();

            var tasks = await repository.GetByProjectIdAsync(request.ProjectId);

            if (tasks.Count() >= 20)
                throw new CustomException("Erro: O limite de 20 tarefas para este projeto foi atingido.");

            var task = mapper.Map<TaskItem>(request);

            if (task.Comments == null || task.Comments.Count() == 0)
            {
                initialComment = new Comment
                {
                    AuthorName = "Sistema",
                    Content = $"Tarefa criada por {task.CreatedBy} em {now:dd/MM/yyyy HH:mm}",
                    CreatedAt = now
                };
                task.Comments = new List<Comment> { initialComment };
            }            
            
            task.Status = Domain.Enums.ETaskStatus.ToDo;

            await repository.AddAsync(task);
            await AddTaskAndCollaboratorToProjectAsync(task);

            return task.Id.ToString()!;
        }

        public async Task<IEnumerable<TaskDto>> GetByProjectIdAsync(string projectId)
        {
            var tasks = await repository.GetByProjectIdAsync(projectId);
            return tasks.Select(t => new TaskDto(
                t.Id.ToString()!,
                t.Title,
                t.Description ?? "",
                t.Status.ToString(),
                t.Priority.ToString(),
                t.ProjectId,
                t.AssignedUserName ?? "Anônimo",
                t.CreatedBy,
                t.Comments.Select(c => new CommentDto(c.Content, c.AuthorName, c.CreatedAt)).ToList()
            ));
        }

        public async Task<TaskDto?> GetByIdAsync(string id)
        {
            var task = await repository.GetByIdAsync(id);
            if (task is null) return null;

            return new TaskDto(
                task.Id.ToString()!,
                task.Title,
                task.Description ?? "",
                task.Status.ToString(),
                task.Priority.ToString(),
                task.ProjectId,
                task.AssignedUserName ?? "Anônimo",
                task.CreatedBy,
                task.Comments.Select(c => new CommentDto(c.Content, c.AuthorName, c.CreatedAt)).ToList()
            );
        }

        public async Task UpdateAsync(UpdateTaskRequest request)
        {
            var task = await repository.GetByIdAsync(request.Id);
            var histories = GetTaskHistories(task, request);

            if (task is null) return;

            if (Enum.TryParse<ETaskPriority>(request.Priority, out var requestedPriority) &&
                   requestedPriority != task.Priority)
                throw new CustomException("A prioridade da tarefa não pode ser alterada.");

            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = Enum.Parse<ETaskStatus>(request.Status);
            task.AssignedUserName = request.AssignedUserName ?? task.AssignedUserName;
            task.DueDate = request.DueDate;
            task.UpdatedBy = request.UpdatedBy;
            task.UpdatedAt = DateTime.UtcNow;

            if (request.Comments is not null && request.Comments.Any())
            {
                foreach (var comment in request.Comments)
                {
                    task.Comments.Add(new Comment
                    {                      
                        AuthorName = comment.AuthorName,
                        Content = comment.Content,                       
                        CreatedAt = comment.CreatedAt
                    });
                }
            }

            await repository.UpdateAsync(task);
            
                        
            foreach (var history in histories)
            {
                await taskHistoryRepository.AddHistoryAsync(history);
            }
        }

        public async Task DeleteAsync(string projectId, string taskId)
        {
            await repository.DeleteAsync(taskId);
            await projectRepository.DeleteTaskAsync(projectId, taskId);
        }

        #region Private Methods
        private async Task AddTaskAndCollaboratorToProjectAsync(TaskItem task)
        {
            var taskCreated = new TaskCreated
            {
                TaskId = task.Id.ToString(),
                CreatedBy = task.CreatedBy
            };

            await projectRepository.AddTaskToProjectAsync(task.ProjectId, taskCreated);
            await projectRepository.AddCollaboratorToProjectAsync(task.ProjectId, task.AssignedUserName);
        }

        private List<TaskHistory> GetTaskHistories(TaskItem task, UpdateTaskRequest request)
        {
            var histories = new List<TaskHistory>();

            // Verificar cada campo e registrar alterações no histórico
            if (task.Title != request.Title)
            {
                histories.Add(CreateTaskHistory("Title", task.Title, request.Title, request.UpdatedBy, task.Id.ToString()));
                task.Title = request.Title;
            }

            if (task.Description != request.Description)
            {
                histories.Add(CreateTaskHistory("Description", task.Description, request.Description, request.UpdatedBy, task.Id.ToString()));
                task.Description = request.Description;
            }

            if (task.Status != Enum.Parse<ETaskStatus>(request.Status))
            {
                histories.Add(CreateTaskHistory("Status", task.Status.ToString(), request.Status.ToString(), request.UpdatedBy, task.Id.ToString()));
                task.Status = Enum.Parse<ETaskStatus>(request.Status);
            }

            if (task.AssignedUserName != request.AssignedUserName)
            {
                histories.Add(CreateTaskHistory("AssignedUserName", task.AssignedUserName, request.AssignedUserName, request.UpdatedBy, task.Id.ToString()));
                task.AssignedUserName = request.AssignedUserName ?? task.AssignedUserName;
            }

            if (task.Comments.Count != request.Comments.Count ||
                   task.Comments.Any(c => !request.Comments.Any(r => r.Content == c.Content && r.AuthorName == c.AuthorName)))
            {
                histories.Add(CreateTaskHistory("Comments",
                    string.Join(", ", task.Comments.Select(c => c.Content)),
                    string.Join(", ", request.Comments.Select(c => c.Content)),
                    request.UpdatedBy,
                    task.Id.ToString()));

                // Atualizar os comentários na tarefa
                task.Comments = ConvertToComments(request.Comments);
            }

            return histories;
        }

        private List<Comment> ConvertToComments(List<CommentDto> commentDtos)
        {
            return commentDtos?.Select(dto => new Comment
            {               
                AuthorName = dto.AuthorName,
                Content = dto.Content,
                CreatedAt = dto.CreatedAt
            }).ToList();
        }

        private TaskHistory CreateTaskHistory(string fieldName, string oldValue, string newValue, string modifiedBy, string taskId)
        {
            return new TaskHistory
            {
                Id = ObjectId.GenerateNewId(),
                TaskId = taskId,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                ModifiedBy = modifiedBy,
                ModifiedAt = DateTime.UtcNow
            };
        }

       
        #endregion
    }
}
