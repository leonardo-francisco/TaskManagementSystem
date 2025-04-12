using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Requests;

namespace TaskManager.Application.Validators
{
    public sealed class TaskValidator : AbstractValidator<CreateTaskRequest>
    {
        public TaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("O título da tarefa é obrigatório.")
                .MaximumLength(150).WithMessage("O título deve ter no máximo 150 caracteres.");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("O projeto é obrigatório.");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Prioridade inválida.");
        }
    }
}
