module OpenTab

open System
open FSharp.Data
open Domain
open Commands
open CommandHandlers
open Queries
open ReadModels

[<Literal>]
let OpenTabJson = """{
  "openTab" : {
    "tableNumber" : 1
  }
}"""

type OpenTabReq = JsonProvider<OpenTabJson>

let (|OpenTabRequest|_|) payload =
  try
    let req = OpenTabReq.Parse(payload).OpenTab
    {Id = Guid.NewGuid(); TableNumber = req.TableNumber}
    |> Some
  with
  | _ -> None

let validateOpenTab getTableByTableNumber tab = async {
  let! result = getTableByTableNumber tab.TableNumber
  match result with
  | Some table -> return Choice1Of2 tab
  | None -> return Choice2Of2 "Invalid table number."
}

let openTabCommander getTableByNumber = {
  Validate = validateOpenTab getTableByNumber
  ToCommand = OpenTab
}