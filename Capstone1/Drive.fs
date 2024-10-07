module Capstone1.Drive

open Data.State

type Destination = Home | Office | Stadium | GasStation

let parseDestination (s: string) =
    match s.ToLower() with
        | "home" -> Ok Home
        | "office" -> Ok Office
        | "stadium" -> Ok Stadium
        | "gasstation" -> Ok GasStation
        | _ -> Error $"Unknown destination {s}"

let gasConsumption dest =
    match dest with
        | Home -> 25
        | Office -> 50
        | Stadium -> 25
        | GasStation -> 10

let drive gas dest =
    let remainingGas = gas - (gasConsumption dest)
    if remainingGas < 0
    then Error "You don't have enough gas"
    else
        if dest = GasStation then Ok <| remainingGas + 50 else Ok remainingGas

let processInput s =
    state {
        let! gas = get
        let result = parseDestination s |> Result.bind (drive gas)
        do! put (Result.defaultValue gas result)
        return result
    }