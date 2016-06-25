module Waiter

open System
open Projections
open ReadModels
open System.Collections.Generic
open Table

let private waiterToDos = new Dictionary<Guid, WaiterToDo>()

let private addDrinkToServe tabId drinks =
  match getTableById tabId with
  | Some table ->
    let toDo = {
      Tab = {Id = tabId; TableNumber = table.Number}
      Foods = []
      Drinks = drinks
    }
    waiterToDos.Add(tabId, toDo)
  | None -> ()
  async.Return ()

let private addFoodToServe tabId foods =
  if waiterToDos.ContainsKey tabId then
    let toDo = waiterToDos.[tabId]
    let waiterToDo =
      {toDo with Foods = foods :: toDo.Foods}
    waiterToDos.[tabId] <- waiterToDo
  else
    match getTableById tabId with
    | Some table ->
      let toDo = {
        Tab = {Id = tabId; TableNumber = table.Number}
        Foods = [foods]
        Drinks = []
      }
      waiterToDos.Add(tabId, toDo)
    | None -> ()
  async.Return ()

let private markDrinkServed tabId drink =
  let toDo = waiterToDos.[tabId]
  let waiterToDo = {
    toDo with
      Drinks = List.filter (fun d -> d <> drink) toDo.Drinks
  }
  waiterToDos.[tabId] <- waiterToDo
  async.Return ()

let private markFoodServed tabId foods =
  let toDo = waiterToDos.[tabId]
  let waiterToDo = {
    toDo with
      Foods = List.filter (fun f -> f <> foods) toDo.Foods
  }
  waiterToDos.[tabId] <- waiterToDo
  async.Return ()

let private remove tabId =
  waiterToDos.Remove(tabId) |> ignore
  async.Return ()

let waiterActions : WaiterActions = {
  AddDrinksToServe = addDrinkToServe
  AddFoodToServe = addFoodToServe
  MarkDrinkServed = markDrinkServed
  MarkFoodServed = markFoodServed
  Remove = remove
}

let getWaiterToDos () =
  waiterToDos.Values
  |> Seq.toList
  |> async.Return