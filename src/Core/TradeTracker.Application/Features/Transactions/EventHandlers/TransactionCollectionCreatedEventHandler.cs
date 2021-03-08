using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TradeTracker.Application.Interfaces.Infrastructure;
using TradeTracker.Application.Models;
using TradeTracker.Domain.Events;

namespace TradeTracker.Application.Features.Transactions.EventHandlers
{
    public class TransactionCollectionCreatedEventHandler : INotificationHandler<DomainEventNotification<TransactionCollectionCreatedEvent>>
    {
        private readonly ILogger<TransactionCollectionCreatedEventHandler> _logger;
        private readonly IPositionTrackingService _positionTrackingService;

        public TransactionCollectionCreatedEventHandler(
            ILogger<TransactionCollectionCreatedEventHandler> logger,
            IPositionTrackingService positionTrackingService)
        {
            _logger = logger;
            _positionTrackingService = positionTrackingService;
        }

        public Task Handle(DomainEventNotification<TransactionCollectionCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"TradeTracker Domain Event: {domainEvent.GetType().Name} received.");

            _positionTrackingService.RefreshAfterCollectionCreation(
                accessKey: domainEvent.AccessKey, 
                transactionMap: domainEvent.TransactionMap);
        
            return Task.CompletedTask;
        }
    }
}