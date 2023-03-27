module ChickenScratch.CodeGeneration

open System

let generateRecordType<'t, 'row> name (getData : 't -> obj[,]) (getFieldNames : 't -> string seq) (target : 't) =
    let data = target |> getData
    let fieldDefs =     
        target
        |> getFieldNames
        |> Seq.mapi (fun idx fname -> (idx, fname, (data[*, idx] |> Array.exists (fun v -> v = null)), (data[0, idx].GetType().FullName)))

    seq {
        yield sprintf "type %s = {" name
        for (_, fname, isOptional, fieldType) in fieldDefs do
            yield sprintf "    %s : %s%s" fname fieldType (if isOptional then " option" else "")
        yield "}"
        yield "    with"
        yield sprintf "        static member FromArray2D (source : obj[,]) ="
        yield "            ["
        yield "                for rowIdx in {0..(Array2D.length1 source) - 1} do"
        yield "                    yield {"
        for (idx, fname, isOptional, fieldType) in fieldDefs do
            if isOptional then
                yield sprintf "                        %s = if source[rowIdx, %d] = null then None else Some (source[rowIdx, %d] :?> %s)" fname idx idx fieldType
            else
                yield sprintf "                        %s = source[rowIdx, %d] :?> %s" fname idx fieldType            
        yield "                    }"
        yield "            ]"        
    }
    |> String.concat "\n"