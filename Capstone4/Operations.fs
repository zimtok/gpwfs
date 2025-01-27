module Capstone4.Operations

open System
open Capstone4.Domain
open Capstone4.Domain.Accounts

let classifyAccount account =
    if account.Balance >= 0M then InCredit account else Overdrawn account

/// Withdraws an amount of an account (if there are sufficient funds)
let withdraw amount (InCredit account) =
    { account with Balance = account.Balance - amount }
    |> classifyAccount

/// Deposits an amount into an account
let deposit amount account =
    let account = get id account
    { account with Balance = account.Balance + amount }
    |> classifyAccount

/// Runs some account operation such as withdraw or deposit with auditing.
let auditAs operationName audit operation amount account =
    let updatedAccount = operation amount account
    
    let accountIsUnchanged = (updatedAccount = account)

    let transaction =
        let transaction = { Operation = operationName; Amount = amount; Timestamp = DateTime.UtcNow; Accepted = true }
        if accountIsUnchanged then { transaction with Accepted = false }
        else transaction
    
    let accountId = get (_.AccountId) account
    let ownerName = get (_.Owner.Name) account
    audit accountId ownerName transaction
    updatedAccount

let private tryParseOp op =
    match op with
    | "withdraw" -> Some Withdraw
    | "deposit" -> Some Deposit
    | _ -> None
    

/// Creates an account from a historical set of transactions
let loadAccount (owner, accountId, transactions) =
    let openingAccount = { AccountId = accountId; Balance = 0M; Owner = { Name = owner } } |> classifyAccount

    transactions
    |> Seq.sortBy(_.Timestamp)
    |> Seq.fold(fun account txn ->
        let op = tryParseOp txn.Operation
        match op, account with
        | Some Deposit, _ -> deposit txn.Amount account
        | Some Withdraw, InCredit _ -> withdraw txn.Amount account
        | Some Withdraw, Overdrawn _ -> account
        | None, _ -> account) openingAccount