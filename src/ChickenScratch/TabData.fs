namespace ChickenScratch

open System
open Microsoft.FSharp.Reflection
open ChickenScratch.Utility


type TabDataCell = System.Collections.Generic.KeyValuePair<string,obj>
type TabDataRow = TabDataCell seq


module TabDataCell =   
    type private IsEnum<'t when 't : (new : unit -> 't)
                    and 't : struct 
                    and 't :> ValueType> = 't
        

    let inline Cast<'t> (kvp : TabDataCell) = kvp.Value :?> 't


    let inline CastOptional<'t> (kvp : TabDataCell) = 
        match kvp.Value with
        | null -> None
        | _ -> Some (Cast<'t> kvp)


    let inline ParseEnumWithConvention<'t when IsEnum<'t>> convention (kvp : TabDataCell) = 
        kvp.Value :?> string |> String.toNamingConvention convention |> Enum.Parse<'t>


    let inline ParseEnum<'t when IsEnum<'t>> kvp = ParseEnumWithConvention<'t> PascalCase kvp


    let inline ParseOptionalEnumWithConvention<'t when IsEnum<'t>> convention (kvp : TabDataCell) = 
        match kvp.Value with
        | null -> None
        | _ -> Some (ParseEnumWithConvention<'t> convention kvp)


    let inline ParseOptionalEnum<'t when IsEnum<'t>> kvp = 
        ParseOptionalEnumWithConvention<'t> PascalCase kvp


    let inline ParseUnion<'t> (kvp : TabDataCell) = 
        let case = kvp.Value :?> string |> String.toNamingConvention PascalCase
        
        FSharpType.GetUnionCases typeof<'t> 
        |> Array.tryFind (fun uc -> uc.Name = case)
        |> function
            | Some uc -> FSharpValue.MakeUnion(uc, [||]) :?> 't
            | _ -> invalidArg "unionCase" (sprintf "Could not find union case %s" case)


    let inline ParseOptionalUnion<'t> (kvp : TabDataCell) = 
        match kvp.Value with
        | null -> None
        | _ -> Some (ParseUnion<'t> kvp)



module TabDataRow =
    let inline Value name (row : TabDataRow) = 
        row |> Seq.find (fun kvp -> kvp.Key = name) |> TabDataCell.Cast


    let inline OptionalValue name (row : TabDataRow) = 
        row 
        |> Seq.tryFind (fun kvp -> kvp.Key = name) 
        |> function
            | Some kvp -> TabDataCell.CastOptional kvp
            | _ -> None
        

    let inline EnumValueWithConvention convention name (row : TabDataRow) = 
        row |> Seq.find (fun kvp -> kvp.Key = name) |> TabDataCell.ParseEnumWithConvention convention


    let inline EnumValue name row = EnumValueWithConvention PascalCase name row


    let inline OptionalEnumValueWithConvention convention name (row : TabDataRow) =
        row 
        |> Seq.tryFind (fun kvp -> kvp.Key = name)
        |> function
            | Some kvp -> TabDataCell.ParseOptionalEnumWithConvention convention kvp
            | _ -> None


    let inline OptionalEnumValue name row = OptionalEnumValueWithConvention PascalCase name row


    //let inline UnionValueWithConvention convention name (row : TabDataRow) = 
    //    row |> Seq.find (fun kvp -> kvp.Key = name) |> TabDataCell.ParseUnionWithConvention convention


    //let inline UnionValue name row = UnionValueWithConvention PascalCase name row


    //let inline OptionalUnionValueWithConvention convention name (row : TabDataRow) =
    //    row 
    //    |> Seq.tryFind (fun kvp -> kvp.Key = name) 
    //    |> function
    //        | Some kvp -> TabDataCell.ParseOptionalUnionWithConvention convention kvp
    //        | _ -> None        


    //let inline OptionalUnionValue name row = OptionalUnionValueWithConvention PascalCase name row


