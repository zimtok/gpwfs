namespace Capstone4.Domain

open System

type Customer = { Name : string }
type Account = { AccountId : Guid; Owner : Customer; Balance : decimal }
type RatedAccount = InCredit of Account | Overdrawn of Account
type Transaction = { Timestamp : DateTime; Operation : string; Amount : decimal; Accepted : bool }
type BankOperation = Deposit | Withdraw

module Accounts =
    let get f account =
        match account with
        | InCredit c -> f c
        | Overdrawn o -> f o

module Transactions =
    /// Serializes a transaction
    let serialize transaction =
        $"{transaction.Timestamp}***%s{transaction.Operation}***%M{transaction.Amount}***%b{transaction.Accepted}"
    
    /// Deserializes a transaction
    let deserialize (fileContents:string) =
        let parts = fileContents.Split([|"***"|], StringSplitOptions.None)
        { Timestamp = DateTime.Parse parts.[0]
          Operation = parts.[1]
          Amount = Decimal.Parse parts.[2]
          Accepted = Boolean.Parse parts.[3] }