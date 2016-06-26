module CloseTab

open FSharp.Data
open Domain
open ReadModels
open Commands
open Queries
open CommandHandlers
open PrepareFood

[<Literal>]
let CloseTabJson = """{
  "closeTab" : {
    "tabId" : "b29fd982-d980-4b94-8c3f-36558e228e5f",
    "amount" : 12.1
  }
}"""

type CloseTabReq = JsonProvider<CloseTabJson>

let (|CloseTabRequest|_|) payload =
  try
    let request = CloseTabReq.Parse(payload).CloseTab
    (request.TabId, request.Amount) |> Some
  with
  | _ -> None

let validateCloseTabe getTableByTabId (tabId, amount) = async {
  let! table = getTableByTabId tabId
  match table with
  | Some t ->
    let tab = {Id = tabId; TableNumber = t.Number}
    return Choice1Of2 {Amount = amount; Tab = tab}
  | _ -> return Choice2Of2 "Invalid tab id."
}

let closeTabCommander getTableByTabId = {
  Validate = validateCloseTabe getTableByTabId
  ToCommand = CloseTab
}