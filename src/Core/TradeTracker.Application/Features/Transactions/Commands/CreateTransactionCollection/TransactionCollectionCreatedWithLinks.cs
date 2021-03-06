using System.Collections.Generic;
using TradeTracker.Application.Features.Transactions.Queries.GetTransactions;
using TradeTracker.Application.Models.Common.Resources.Responses;

namespace TradeTracker.Application.Features.Transactions.Commands.CreateTransactionCollection
{
    public class TransactionCollectionCreatedWithLinks
    {
        public IEnumerable<TransactionForReturnWithLinks> Items { get; set; }

        public IEnumerable<Link> Links { get; set; }
    }
}