module Capstone2.Main

open Data.ResultExtension
open Capstone2.Account
open System

let printError (s: string) =
    Console.WriteLine $"ERROR: {s}"

let cliWriter account transaction =
    printf $"{account.Name}; {toString transaction}; {account.Balance}\n"
    Ok ()


[<TailCall>]
let rec mainLoop account =
    let input = Console.ReadLine()
    if String.IsNullOrEmpty input
        then exit 0
    let r = processLine veryDumbParser cliWriter account input
    (Result.mapError printError r) |> ignore
    mainLoop (Result.defaultValue account r)
    ()

let readName =
    Console.WriteLine("Enter your name:")
    let name = Console.ReadLine()
    if String.IsNullOrEmpty name then Error "invalid name" else Ok name

let readStartingBalance =
    Console.WriteLine("Enter your starting balance:")
    let balance = Console.ReadLine()
    tryToResult parseBalance balance

result {
    let! name = readName
    let! balance = readStartingBalance
    let account = {
        Name = name
        Balance = balance
    }
    return (mainLoop account)
} |> ignore
