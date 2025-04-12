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

namespace TaskManager.Application.Services
{
    public sealed class TaskService(ITaskRepository repository, 
                                    IProjectRepository projectRepository,
                                    ITaskHistoryRepository taskHistoryRepository) : ITaskService
    {
        public async Task<string> CreateAsync(CreateTaskRequest request)
        {
            var createdBy = request.CreatedBy ?? "Desconhecido";
            var assignedUser = request.AssignedUserName ?? "Anônimo";
            var now = DateTime.UtcNow;

            var tasks = await repository.GetByProjectIdAsync(request.ProjectId);

            if (tasks.Count() >= 20)
                return "Erro: O limite de 20 tarefas para este projeto foi atingido.";

            var initialComment = new Comment
            {
                Id = ObjectId.GenerateNewId(),
                TaskId = "",
                AuthorName = createdBy,
                Content = $"Tarefa criada por {createdBy} em {now:dd/MM/yyyy HH:mm}",
                CreatedAt = now
            };

            var task = new TaskItem
            {
                Id = ObjectId.GenerateNewId(),
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                ProjectId = request.ProjectId,
                AssignedUserName = assignedUser,
                CreatedBy = createdBy,
                Comments = new List<Comment> { initialComment },
                Status = Domain.Enums.ETaskStatus.ToDo,
                CreatedAt = DateTime.UtcNow
            };

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
                t.Status,
                t.Priority,
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
                task.Status,
                task.Priority,
                task.ProjectId,
                task.AssignedUserName ?? "Anônimo",
                task.CreatedBy,
                task.Comments.Select(c => new CommentDto(c.Content, c.AuthorName, c.CreatedAt)).ToList()
            );
        }

        public async Task UpdateAsync(UpdateTaskRequest request)
        {
            var task = await repository.GetByIdAsync(request.Id);

            if (task is null) return;
          
            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = request.Status;
            task.AssignedUserName = request.AssignedUserName ?? task.AssignedUserName;
            task.UpdatedAt = DateTime.UtcNow;

            if (request.Comments is not null && request.Comments.Any())
            {
                foreach (var comment in request.Comments)
                {
                    task.Comments.Add(new Comment
                    {
                        Id = ObjectId.GenerateNewId(),
                        TaskId = task.Id.ToString(),
                        AuthorName = comment.AuthorName,
                        Content = comment.Content,                       
                        CreatedAt = comment.CreatedAt
                    });
                }
            }

            await repository.UpdateAsync(task);
            
            var histories = GetTaskHistories(task, request);           
            foreach (var history in histories)
            {
                await taskHistoryRepository.AddHistoryAsync(history);
            }
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
                histories.Add(CreateTaskHistory("Title", task.Title, request.Title, request.UpdatedBy));
                task.Title = request.Title;
            }

            if (task.Description != request.Description)
            {
                histories.Add(CreateTaskHistory("Description", task.Description, request.Description, request.UpdatedBy));
                task.Description = request.Description;
            }

            if (task.Status != request.Status)
            {
                histories.Add(CreateTaskHistory("Status", task.Status.ToString(), request.Status.ToString(), request.UpdatedBy));
                task.Status = request.Status;
            }

            if (task.AssignedUserName != request.AssignedUserName)
            {
                histories.Add(CreateTaskHistory("AssignedUserName", task.AssignedUserName, request.AssignedUserName, request.UpdatedBy));
                task.AssignedUserName = request.AssignedUserName ?? task.AssignedUserName;
            }

            if (task.Comments.Count != request.Comments.Count ||
                   task.Comments.Any(c => !request.Comments.Any(r => r.Content == c.Content && r.AuthorName == c.AuthorName)))
            {
                histories.Add(CreateTaskHistory("Comments",
                    string.Join(", ", task.Comments.Select(c => c.Content)),
                    string.Join(", ", request.Comments.Select(c => c.Content)),
                    request.UpdatedBy));

                // Atualizar os comentários na tarefa
                task.Comments = ConvertToComments(request.Comments);
            }

            return histories;
        }

        private List<Comment> ConvertToComments(List<CommentDto> commentDtos)
        {
            return commentDtos?.Select(dto => new Comment
            {
                Id = ObjectId.GenerateNewId(), 
                TaskId = dto.TaskId,
                AuthorName = dto.AuthorName,
                Content = dto.Content,
                CreatedAt = dto.CreatedAt
            }).ToList();
        }

        private TaskHistory CreateTaskHistory(string fieldName, string oldValue, string newValue, string modifiedBy)
        {
            return new TaskHistory
            {
                Id = ObjectId.GenerateNewId(),
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
