# Add Invoice

-> id = 71f6e412-e7e4-40b6-aa8d-d3005a5f757f
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2017-05-30T10:14:44.2298760Z
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


~~~
