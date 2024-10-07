module Capstone1.Main
open System
open Capstone1.Drive
open Data.State

let printResult res =
    match res with
        | Ok (v) ->  printf $"Remaining gas: {v}\n"
        | Error (e) -> printf $"ERROR: {e}\n"

[<TailCall>]
let rec main gas =
    printf "Enter a destination: "
    let input = Console.ReadLine()
    if String.IsNullOrEmpty(input) then
        exit 0
    else
        let result, remainingGas = run (processInput input) gas
        printResult result
        main remainingGas

main 100