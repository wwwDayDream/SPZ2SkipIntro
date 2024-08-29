module Operators

    open System
    open System.Runtime.CompilerServices

    /// <summary>
    /// Pipes <paramref name="value"/> into a list of <paramref name="functions"/> and returns a list of the results.
    /// </summary>
    /// <param name="value">The value to pipe into the functions</param>
    /// <param name="functions">The functions to execute with the value provided</param>
    let inline (|>|) value functions =
        functions |> List.map (fun f -> f value)
    
    /// <summary>
    /// Pipes <paramref name="value"/> into a list of <paramref name="functions"/> and ignores the return.
    /// </summary>
    /// <param name="value">The value to pipe into the functions</param>
    /// <param name="functions">The functions to execute with the value provided</param>
    let inline (|>|!) value functions = value |>| functions |> ignore
    
    let inline (|?>) (value: 'a option) (func: 'a -> 'b, def: 'b) =
        if value.IsSome then func(value.Value) else def
    let inline (|?>!) (value: 'a option) (func: 'a -> 'b) =
        if value.IsSome then func(value.Value) |> ignore
        
    /// <summary>
    /// Pipes the option into the function if it is something and also turns the <paramref name="func"/>'s result into an option return.
    /// </summary>
    /// <param name="value">The value to pipe in.</param>
    /// <param name="func">The func to run if the value is not None.</param>
    let inline (|??>) (value: 'a option) func =
        if value.IsSome then
            func(value.Value) |> Some
        else
            None
    /// <summary>
    /// Pipes the option into the function if it is something and ignores the return.
    /// </summary>
    /// <param name="value">The value to pipe in.</param>
    /// <param name="func">The func to run if the value is not None.</param>
    let inline (|??>!) (value:'a option) func = value |??> func |> ignore
    
    /// <summary>
    /// Turns a potentially null object into an option with None if null.
    /// </summary>
    /// <param name="obj">A potentially null object</param>
    let inline (!?!) (obj: 'A): 'A option =
        if obj <> null then Some obj else None