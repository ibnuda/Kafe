module Cashier

open System
open Domain
open Projections
open System.Collections.Generic
open Table

let private cashierToDos = new Dictionary<Guid, Payment>()

let private addTabAmount tabId amount =
  match getTableByTabId tabId with
  | Some table ->
    let payment = {Tab = { Id = tabId; TableNumber = table.Number}; Amount = amount}
    cashierToDos.Add(tabId, payment)
  | None -> ()
  async.Return ()

let private remove tabId =
  cashierToDos.Remove(tabId) |> ignore
  async.Return ()

let cashierActions = {
  AddTabAmount = addTabAmount
  Remove = remove
}

let getCashierToDos () =
  cashierToDos.Values
  |> Seq.toList
  |> async.Return