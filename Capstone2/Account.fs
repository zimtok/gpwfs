module Capstone2.Account

open Data.ResultExtension

type Transaction = Deposit of int | Withdrawal of int
type Account = {
    Name : string
    Balance : int
}

let applyTransaction account transaction =
    match transaction with
        | Deposit d -> Ok ({account with Balance = account.Balance + d})
        | Withdrawal w ->
        if account.Balance >= w
        then Ok ({account with Balance = account.Balance - w})
        else Error "Overdraft"

let processLine parser writer account line =
 result {
    let! transaction = parser line
    let! updated = applyTransaction account transaction
    do! writer updated transaction
    return updated
 }