﻿using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Contracts;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Requests;
using TaskManager.Application.Mapper;
using TaskManager.Application.Services;
using TaskManager.Application.Validators;
using TaskManager.Domain.Contracts;

namespace TaskManager.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {           
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IReportService, ReportService>();

            services.AddScoped<IValidator<ProjectDto>, ProjectValidator>();
            services.AddScoped<IValidator<CreateTaskRequest>, TaskValidator>();

            services.AddAutoMapper(typeof(MapConfig));

            return services;
        }
    }
}
