module ChickenScratch.CodeGeneration

open System

type RecordFieldDefinition = {
    Name : string
    Type : Type
    SampleData : obj seq
}

type RecordDefinition = {
    Name : string
    Fields : RecordFieldDefinition seq
}

let generateRecordType<'t> (getDef : 't -> RecordDefinition) (target : 't) =
    ""
