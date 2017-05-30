# Add Invoice

-> id = 71f6e412-e7e4-40b6-aa8d-d3005a5f757f
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2017-05-30T09:20:31.1196360Z
-> tags = 

[Invoice]
|> CreateInvoice
    [BasicApi`1]
    |> InputIs
        [Model`1]
        |> Amount Amount=500
        |> Number Number=abc123
        |> Quantity Quantity=5
        |> Total Total=2500

    |> Status Status=OK

~~~
