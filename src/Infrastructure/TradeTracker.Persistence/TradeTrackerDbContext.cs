using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeTracker.Application.Common.Interfaces.Infrastructure;
using TradeTracker.Application.Common.Interfaces.Persistence;
using TradeTracker.Application.Interfaces;
using TradeTracker.Domain.Common;
using TradeTracker.Domain.Entities;
using TradeTracker.Domain.Interfaces;
using TradeTracker.Persistence.Seed.Transactions;

namespace TradeTracker.Persistence
{
    public class TradeTrackerDbContext : DbContext, ITradeTrackerDbContext
    {
        private readonly IDomainEventService _domainEventService;
        private readonly ILoggedInUserService _loggedInUserService;

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Position> Positions { get; set; }

        public TradeTrackerDbContext(DbContextOptions<TradeTrackerDbContext> options)
           : base(options)
        {
        }

        public TradeTrackerDbContext(
            DbContextOptions<TradeTrackerDbContext> options,
            IDomainEventService domainEventService,
            ILoggedInUserService loggedInUserService)
            : base(options)
        {
            _domainEventService = domainEventService;
            _loggedInUserService = loggedInUserService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TradeTrackerDbContext).Assembly);

            var equityTransactions = TransactionSeeder.GenerateEquityTransactions(
                accessKey: Guid.Parse("322f44e6-bacd-42a4-9f8d-d0a8d36eb1cb"),
                transactionCount: 100);

            modelBuilder.Entity<Transaction>().HasData(equityTransactions);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _loggedInUserService.UserId;
                        break;
                        
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = _loggedInUserService.UserId;
                        break;
                }
            }

            var events = ExtractEvents();

            var result = await base.SaveChangesAsync(cancellationToken);

            await DispatchEvents(events);

            return result;
        }

        private IEnumerable<DomainEvent> ExtractEvents()
        {
            return ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .ToList();
        }

        private async Task DispatchEvents(IEnumerable<DomainEvent> events)
        {
            while (true)
            {
                var domainEvent = events
                    .Where(e => !e.IsPublished)
                    .FirstOrDefault();

                if (domainEvent == null)
                {
                    break;
                }

                domainEvent.IsPublished = true;
                await _domainEventService.Publish(domainEvent);
            }
        }
    }
}
