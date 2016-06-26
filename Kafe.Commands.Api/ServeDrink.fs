module ServeDrink

open FSharp.Data
open Commands
open Queries
open CommandHandlers

[<Literal>]
let ServeDrinkJson = """{
  "serveDrink": {
    "tabId" : "b29fd982-d980-4b94-8c3f-36558e228e5f",
    "menuNumber" : 1
  }
}"""

type ServeDrinkReq = JsonProvider<ServeDrinkJson>

let (|ServeDrinkRequest|_|) payload =
  try
    let request = ServeDrinkReq.Parse(payload).ServeDrink
    (request.TabId, request.MenuNumber) |> Some
  with
  | _ -> None

let validateServeDrink getTableByTabId getDrinksByMenuNumbers (tabId, drinksMenuNumber) = async {
  let! table = getTableByTabId tabId
  match table with
  | Some _ ->
    let! drinks = getDrinksByMenuNumbers drinksMenuNumber
    match drinks with
    | Some drink -> return Choice1Of2 (drink, tabId)
    | _ -> return Choice2Of2 "Invalid drink menu number."
  | None -> return Choice2Of2 "Invalid tab id."
}

let serveDrinkCommander getTableByTabId getDrinksByMenuNumber =
  let validate = getDrinksByMenuNumber |> validateServeDrink getTableByTabId
  {
    Validate = validate
    ToCommand = ServeDrink
  }