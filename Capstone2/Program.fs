open Data.ResultExtension
open Capstone2.Account
open System

let printError (s: string) =
    Console.WriteLine $"ERROR: {s}"

let printTransaction t =
    match t with
        | Deposit d -> $"+{d}"
        | Withdrawal w -> $"-{w}"

let cliWriter account transaction =
    printf $"{account.Name}; {printTransaction transaction}; {account.Balance}\n"
    Ok ()

let veryDumbParser (line: string) =
    let parseInt (s: string) = Int32.TryParse s
    match line[0] with
        | '+' -> Result.map Deposit (tryToResult parseInt line[1..])
        | '-' -> Result.map Withdrawal (tryToResult parseInt line[1..])
        | _ -> Error $"Invalid input ${line}"

let rec mainLoop account =
    let input = Console.ReadLine()
    if String.IsNullOrEmpty input
        then exit 0
    let r = processLine veryDumbParser cliWriter account input
    (Result.mapError printError r) |> ignore
    mainLoop (Result.defaultValue account r)

let testAcc = {
    Name = "Oak Nuggins"
    Balance = 500
}
mainLoop testAcc