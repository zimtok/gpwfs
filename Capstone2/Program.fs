module Capstone2.Main

open Data.ResultExtension
open Capstone2.Account
open Capstone2.ConsoleIO
open System


let printError (s: string) =
    Console.WriteLine $"ERROR: {s}"


let main =
    let account, transactions = cliReader
    let accumulate = processLine veryDumbParser cliAudit
    let collected =
        result {
            let! a = account
            let! t = transactions
            return! Seq.fold (fun acc curr -> Result.bind (fun a -> accumulate a curr) acc) (Ok a) t
        }
    match collected with
        | Ok a -> printf $"{a.Name}: {a.Balance}"
        | Error e -> printError e

main