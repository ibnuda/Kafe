module Chef

open System
open System.Collections.Generic
open Domain
open ReadModels
open Projections
open Queries
open Table

let private chefToDos = new Dictionary<Guid, ChefToDo>()

let private addFoodsToPrepare tabId foods =
  match getTableById tabId with
  | Some table ->
    let tab = {Id = tabId; TableNumber = table.Number}
    let toDo : ChefToDo = {Tab = tab; Foods = foods}
    chefToDos.Add(tabId, toDo)
  | None -> ()
  async.Return ()

let private removeFood tabId foods =
  let toDo = chefToDos.[tabId]
  let chefTodo = {toDo with Foods = List.filter (fun removed -> removed <> foods) toDo.Foods}
  chefToDos.[tabId] <- chefTodo
  async.Return ()

let private remove tabId =
  chefToDos.Remove(tabId) |> ignore
  async.Return ()

let chefActions = {
  AddFoodsToPrepare = addFoodsToPrepare
  RemoveFood = removeFood
  Remove = remove
}

let getChefToDo () =
  chefToDos.Values
  |> Seq.toList
  |> async.Return