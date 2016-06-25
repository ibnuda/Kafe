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
      Drinks = [drinks]
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