using System;
using System.Collections.Generic;
using Domain.Events;
using Event.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ratelimiter.Controllers
{
    [Route("/api/[controller]")]
    public class InvoiceController : Controller
    {
        private ILogger<InvoiceController> _logger;
        private IEventStore _eventStore;

        public InvoiceController(ILogger<InvoiceController> logger,
                                 IEventStore eventStore)
        {
            _logger = logger;
            _eventStore = eventStore;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello";
        }

        [HttpPost]
        public InvoiceCreated Post([FromBody]CreateInvoiceModel model)
        {
            return new InvoiceCreated(Guid.NewGuid(),
                                      new List<LineItem> {
                                          new LineItem(model.Number, model.Amount, model.Quantity, model.Total)
                                      });
        }
    }

    public class CreateInvoiceModel {
        public decimal Amount { get; set; }
        public string Number { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
