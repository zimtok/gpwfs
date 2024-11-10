module Capstone2.Account

open Data.ResultExtension

type Transaction = Deposit of int | Withdrawal of int
type Account = {
    Name : string
    Balance : int
}

let toString t =
    match t with
    | Deposit d -> $"D{d}"
    | Withdrawal w -> $"R{w}"

let parseBalance (s: string) = System.Int32.TryParse s
let veryDumbParser (line: string) =
    let action =
        match line[0] with
        | 'D' -> Ok Deposit
        | 'W' -> Ok Withdrawal
        | _ -> Error $"Invalid input ${line}"
    let amount =
        tryToResult parseBalance line[1..]
        |> Result.bind (fun v -> if v >= 0 then Ok v else Error "Invalid amount")
    apply action amount

let applyTransaction account transaction =
    match transaction with
        | Deposit d -> Ok {account with Balance = account.Balance + d}
        | Withdrawal w ->
        if account.Balance >= w
        then Ok {account with Balance = account.Balance - w}
        else Error "Overdraft"

let processLine parser writer account line =
 result {
    let! transaction = parser line
    let! updated = applyTransaction account transaction
    do! writer updated transaction
    return updated
 }