using System;
using System.Collections.Generic;
using Event.Infrastructure;

namespace Domain.Events
{
    public class InvoiceCreated : IEvent
    {
        public InvoiceCreated(Guid id, List<LineItem> items) {
            Id = id;
            Items = items;
        }

        public Guid Id { get; private set; }
        public List<LineItem> Items { get; private set; }
    }

    public class LineItem {
        public LineItem(string number, decimal amount, int quantity, decimal total) {
            Number = number;
            Amount = amount;
            Quantity = quantity;
            Total = total;
        }

        public string Number { get; }
        public decimal Amount { get; }
        public int Quantity { get; }
        public decimal Total { get; }
    }
}
