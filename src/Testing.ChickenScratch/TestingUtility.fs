module Testing.ChickenScratch.TestingUtility

open System

type ThrownExceptionProperty = 
| ExnType of Type
| InnerExnType of Type
| MessageContains of string seq

module ThrownExceptionProperty =
    let describe prop =    
        match prop with
        | ExnType t -> sprintf "Exception of type '%A'" t
        | InnerExnType t -> sprintf "InnerException type '%A'" t
        | MessageContains strs -> sprintf "Message contains %A" strs


    let describeAll props = 
        props |> Seq.map describe |> String.concat "\n\t"


    let matches (exn : Exception) prop = 
        match prop with
        | ExnType t -> t.IsAssignableFrom(exn.GetType())
        | InnerExnType t -> t.IsAssignableFrom(exn.InnerException.GetType())
        | MessageContains strs -> strs |> Seq.forall exn.Message.Contains


    let matchesAll props (exn : Exception) = 
        props |> Seq.forall (matches exn)   
    


let throwWith props = 
    let matches (f : obj) = 
        let wrap tf = 
            try
                tf()
                false
            with ex ->
                ex |> ThrownExceptionProperty.matchesAll props                

        match f with
        | :? (unit -> unit) as tf -> wrap tf
        | :? (unit -> obj) as tf -> wrap (tf >> ignore)
        | _ -> false

    NHamcrest.Core.CustomMatcher<obj>(ThrownExceptionProperty.describeAll props, Func<_,_> matches)

