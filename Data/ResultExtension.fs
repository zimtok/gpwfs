module Data.ResultExtension

type ResultExpression() = 
    member this.Return(v) = Ok v
    member this.Bind(v, f) = Result.bind f v

    member _.ReturnFrom(m) = m

let result = ResultExpression()

let tryToResult tryX string =
    match tryX string with
        | true, v -> Ok v
        | _, _ -> Error $"Failed to parse {string}"

let apply resultFn resultValue =
    match resultFn, resultValue with
        | Ok f, Ok v -> f v |> Ok
        | Error e, _
        | _, Error e -> Error e
