module PlaceOrder

open FSharp.Data
open Queries
open Commands
open Domain
open CommandHandlers

[<Literal>]
let PlaceOrderJson = """{
  "placeOrder" : {
    "tabId" : "b29fd982-d980-4b94-8c3f-36558e228e5f",
    "foodMenuNumbers" : [1, 2],
    "drinkMenuNumbers" : [2, 3]
  }
}"""

type PlaceOrderReq = JsonProvider<PlaceOrderJson>

let (|PlaceOrderRequest|_|) payload =
  try
    PlaceOrderReq.Parse(payload).PlaceOrder |> Some
  with
  | _ -> None 

let validatePlaceOrder queries (c : PlaceOrderReq.PlaceOrder) = async {
  let! table = queries.Table.GetTableByTabId c.TabId
  match table with
  | Some table ->
    let! foods = queries.Food.GetFoodsByMenuNumbers c.FoodMenuNumbers
    let! drinks = queries.Drink.GetDrinksByMenuNumbers c.DrinkMenuNumbers
    let isEmptyOrder foods drinks = List.isEmpty foods && List.isEmpty drinks
    match foods, drinks with
    | Choice1Of2 foods, Choice1Of2 drinks ->
      if isEmptyOrder foods drinks then
        let message = "Order should contain at least 1 food or drink."
        return Choice2Of2 message
      else
        let tab = {Id = c.TabId; TableNumber = table.Number}
        return Choice1Of2 (tab, drinks, foods)
    | Choice2Of2 foodKeys, Choice2Of2 drinkKeys ->
      let message = sprintf "Invalid food keys : %A, Invalid drink keys : %A." foodKeys drinkKeys
      return Choice2Of2 message
    | Choice2Of2 keys, _ ->
      let message = sprintf "Invalid food keys : %A." keys
      return Choice2Of2 message
    | _, Choice2Of2 keys ->
      let message = sprintf "Invalid drink keys : %A." keys
      return Choice2Of2 message
  | None -> return Choice2Of2 "Invalid tab id." 
}

let toPlaceOrderCommand (tab, drinks, foods) =
  {
    Tab = tab
    Drinks = drinks
    Foods= foods
  }
  |> PlaceOrder

let placeOrderCommander queries = {
  Validate = validatePlaceOrder queries
  ToCommand = toPlaceOrderCommand
}