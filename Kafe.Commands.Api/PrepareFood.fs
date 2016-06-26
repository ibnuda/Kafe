module PrepareFood

open FSharp.Data
open Commands
open Queries
open CommandHandlers

[<Literal>]
let PrepareFoodJson = """{
  "prepareFood" : {
    "tabId" : "b29fd982-d980-4b94-8c3f-36558e228e5f",
    "menuNumber" : 1
  }
}"""

type PrepareFoodReq = JsonProvider<PrepareFoodJson>

let (|PrepareFoodRequest|_|) payload =
  try
    let request = PrepareFoodReq.Parse(payload).PrepareFood
    (request.TabId, request.MenuNumber) |> Some
  with
  | _ -> None

let validateFood getTableByTabId getFoodByMenuNumber (tabId, foodNumber) = async {
  let! table = getTableByTabId tabId
  match table with
  | Some table ->
    let! food = getFoodByMenuNumber foodNumber
    match food with
    | Some food -> return Choice1Of2 (food, tabId)
    | _ -> return Choice2Of2 "Invalid food number."
  | None -> return Choice2Of2 "Invalid tab id."
}

let prepareFoodCommander getTableByTabId getFoodByMenuNumber = {
  Validate = validateFood getTableByTabId getFoodByMenuNumber
  ToCommand = PrepareFood
}