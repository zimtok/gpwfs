module Capstone2.ConsoleIO

open System
open Capstone2.Account
open Data.ResultExtension

let cliAudit account transaction =
    printf $"{account.Name}; {toString transaction}; {account.Balance}\n"
    Ok()

let readName =
    Console.WriteLine("Enter your name:")
    let name = Console.ReadLine()
    if String.IsNullOrEmpty name then Error "invalid name" else Ok name

let readStartingBalance =
    Console.WriteLine("Enter your starting balance:")
    let balance = Console.ReadLine()
    tryToResult parseBalance balance

let rec readLines () =
    seq {
        let line = Console.ReadLine()
        if not (String.IsNullOrEmpty line) then
            yield line
            yield! readLines ()
    }
let cliReader: Result<Account,string> * Result<string seq,string> =
    let acc =
        result {
            let! name = readName
            let! balance = readStartingBalance
            return {
                Name = name
                Balance = balance
            }
        }
    let lines = readLines ()
    (acc, Ok lines)
