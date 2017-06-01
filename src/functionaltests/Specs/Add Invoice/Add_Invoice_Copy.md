# Add Invoice Copy

-> id = e9347e07-4d1a-4b73-af0b-60857005b2a6
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2017-06-01T12:03:26.7672760Z
-> tags = 

[Invoice]
|> CreateInvoice
    [BasicApi`2]
    |> InputIs
        [Model`1]
        |> Amount Amount=500
        |> Number Number=fasdasd
        |> Quantity Quantity=200
        |> Total Total=2000

    |> Status Status=OK

|> CheckJsonResponse
    [InvoiceCreatedJsonComparison]
    |> CheckJsonValues
        [table]
        |> CheckJsonValues-row path=$.items[0].amount, returnValue=500


|> CheckEventStore
    [EventStore]
    |> CheckJsonValues
        [table]
        |> CheckJsonValues-row path=$.results[0].amount, returnValue=500


~~~
