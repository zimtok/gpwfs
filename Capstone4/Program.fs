module Capstone4.Program

open System
open Capstone4.Domain
open Capstone4.Domain.Accounts
open Capstone4.Operations

let withdrawWithAudit = auditAs "withdraw" Auditing.composedLogger withdraw
let depositWithAudit = auditAs "deposit" Auditing.composedLogger deposit
let loadAccountFromDisk = FileRepository.tryFindTransactionsOnDisk >> (Option.map Operations.loadAccount)

type Command = Exit | AccountCommand of BankOperation

[<AutoOpen>]
module CommandParsing =
    let tryParseCommand cmd =
        match cmd with
        | 'x' -> Some Exit
        | 'd' -> AccountCommand Deposit |> Some
        | 'w' -> AccountCommand Withdraw |> Some
        | _ -> None
    let tryGetAccountCommand c =
        match c with
        | Exit -> None
        | AccountCommand cmd -> Some cmd
        

[<AutoOpen>]
module UserInput =
    let commands = seq {
        while true do
            Console.Write "(d)eposit, (w)ithdraw or e(x)it: "
            yield Console.ReadKey().KeyChar
            Console.WriteLine() }
    
    let tryGetAmount command =
        Console.WriteLine()
        Console.Write "Enter Amount: "
        let result, amount = Console.ReadLine() |> Decimal.TryParse
        match result with
        | true -> Some (command, amount)
        | false -> None

[<EntryPoint>]
let main _ =
    let openingAccount =
        Console.Write "Please enter your name: "
        let name = Console.ReadLine()
        loadAccountFromDisk name
        |> Option.defaultValue (InCredit {Balance = 0M; AccountId = Guid.NewGuid(); Owner = {Name = name}})
    
    printfn $"Current balance is £%M{get (_.Balance) openingAccount}"

    let processCommand account (command, amount) =
        printfn ""
        let account =
            match command with
            | Deposit -> depositWithAudit amount account
            | Withdraw ->
                match account with
                | InCredit _ -> withdrawWithAudit amount account
                | Overdrawn _ ->
                    printfn "Your account is overdrawn"
                    account
        printfn $"Current balance is £%M{get (_.Balance) account}"
        account

    let closingAccount =
        commands
        |> Seq.choose tryParseCommand
        |> Seq.takeWhile ((<>) Exit)
        |> Seq.choose tryGetAccountCommand
        |> Seq.choose tryGetAmount
        |> Seq.fold processCommand openingAccount
    
    printfn ""
    printfn $"Closing Balance:\r\n %A{closingAccount}"
    Console.ReadKey() |> ignore

    0