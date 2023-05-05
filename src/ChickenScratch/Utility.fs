module ChickenScratch.Utility

open System

type NamingConvention = 
    | PascalCase
    | CamelCase
    | SnakeCase
    | BackTicks



module String = 
    type private processState = 
    | LookingForFirstWord
    | ProcessingLower
    | ProcessingUpper
    | ProcessingNum
    | BetweenWords
    
    let private processStr word1Char1Mod wordxChar1Mod otherMod sep (str : string) =
        let rec next state (chars : char seq) =
            seq {
                match state, Seq.tryHead chars with
                | _, None -> ()
                | LookingForFirstWord, Some c when Char.IsLetter c -> 
                    yield c |> word1Char1Mod
                    let nextMode = if Char.IsUpper c then ProcessingUpper else ProcessingLower
                    yield! next nextMode (Seq.tail chars)
                | LookingForFirstWord, _ -> yield! next LookingForFirstWord (Seq.tail chars)
                | ProcessingLower, Some c when Char.IsUpper c ->
                    if Option.isSome sep then yield sep.Value
                    yield c |> wordxChar1Mod
                    yield! next ProcessingUpper (Seq.tail chars)
                | ProcessingLower, Some c when Char.IsLower c -> 
                    yield c |> otherMod
                    yield! next ProcessingLower (Seq.tail chars)
                | ProcessingLower, Some c when Char.IsDigit c -> 
                    if Option.isSome sep then yield sep.Value
                    yield c
                    yield! next BetweenWords (Seq.tail chars)
                | ProcessingLower, _ -> yield! next BetweenWords (Seq.tail chars)
                
                | ProcessingUpper, Some c when Char.IsUpper c -> 
                    yield c |> otherMod
                    yield! next ProcessingUpper (Seq.tail chars)
                | ProcessingUpper, Some c when Char.IsLower c ->
                    yield c |> otherMod
                    yield! next ProcessingLower (Seq.tail chars)
                | ProcessingUpper, Some c when Char.IsDigit c -> 
                    if Option.isSome sep then yield sep.Value
                    yield c
                    yield! next BetweenWords (Seq.tail chars)
                | ProcessingUpper, _ -> yield! next BetweenWords (Seq.tail chars)
                
                | ProcessingNum, Some c when Char.IsDigit c -> 
                    yield c
                    yield! next ProcessingNum (Seq.tail chars)
                | ProcessingNum, Some c when Char.IsLetter c ->
                    if Option.isSome sep then yield sep.Value
                    yield c |> wordxChar1Mod
                    let nextMode = if Char.IsUpper c then ProcessingUpper else ProcessingLower
                    yield! next nextMode (Seq.tail chars)
                | ProcessingNum, _ -> yield! next BetweenWords (Seq.tail chars)

                | BetweenWords, Some c when Char.IsLetter c -> 
                    if Option.isSome sep then yield sep.Value
                    yield c |> wordxChar1Mod
                    let nextMode = if Char.IsUpper c then ProcessingUpper else ProcessingLower
                    yield! next nextMode (Seq.tail chars)
                | BetweenWords, Some c when Char.IsDigit c -> 
                    if Option.isSome sep then yield sep.Value
                    yield c
                    yield! next ProcessingNum (Seq.tail chars)
                | BetweenWords, _ -> yield! next BetweenWords (Seq.tail chars)
            }

        next LookingForFirstWord str |> Seq.toArray |> String


    let toNamingConvention (naming : NamingConvention) (str : string) = 
        match naming with
        | PascalCase -> str |> processStr Char.ToUpper Char.ToUpper Char.ToLower None           
        | CamelCase -> str |> processStr Char.ToLower Char.ToUpper Char.ToLower None
        | SnakeCase -> str |> processStr Char.ToLower Char.ToLower Char.ToLower (Some '_')
        | BackTicks -> 
            match (Seq.tryHead str), (Seq.tail str) with
            | Some c, t when Char.IsWhiteSpace c && t |> Seq.forall Char.IsWhiteSpace -> ""                
            | Some c, _ when not (Char.IsLetter c || c = '_') -> 
                sprintf "``%s``" str
            | Some c, t when t |> Seq.exists (fun c -> not (Char.IsLetterOrDigit c || c = '_')) ->
                sprintf "``%s``" str
            | _ -> str
            

        



