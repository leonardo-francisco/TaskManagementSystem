using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Requests;

namespace TaskManager.Application.Validators
{
    public sealed class ProjectValidator : AbstractValidator<CreateProjectRequest>
    {
        public ProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do projeto é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do projeto deve ter no máximo 100 caracteres.");
        }
    }
}
