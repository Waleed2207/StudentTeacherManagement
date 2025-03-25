using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentTeacherManagement.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StudentTeacherManagement.Models.Entities;

namespace StudentTeacherManagement.Services
{
    public class DeadlineNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeadlineNotificationService> _logger;

        public DeadlineNotificationService(IServiceScopeFactory scopeFactory, ILogger<DeadlineNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var allAssignments = await unitOfWork.Assignments.GetAllAsync() ?? Enumerable.Empty<Assignment>();

                    var upcomingAssignments = allAssignments
                        .Where(a =>
                            a.Deadline.HasValue &&
                            a.Deadline.Value > DateTime.UtcNow &&
                            a.Deadline.Value <= DateTime.UtcNow.AddDays(1))
                        .ToList();

                    _logger.LogInformation("Found {Count} upcoming assignments due soon.", upcomingAssignments.Count);

                    // Optional: Hook in actual notification logic here
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    _logger.LogError(ex, "An error occurred while checking for upcoming assignment deadlines.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected during shutdown — do nothing
                }
            }
        }
    }
}
