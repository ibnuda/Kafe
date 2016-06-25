module InMemory

open System
open Table
open Chef
open Waiter
open Cashier
open Projections
open Queries
open Items
open EventStore
open NEventStore

type InMemoryEventStore () =
  static member Instance =
    Wireup.Init().UsingInMemoryPersistence().Build()

let inMemoryEventStore () =
  let eventStoreInstance = InMemoryEventStore.Instance
  {
    GetState = getState eventStoreInstance
    SaveEvent = saveEvents eventStoreInstance
  }

let toDoQueries = {
  GetCashierToDos = getCashierToDos
  GetChefToDos = getChefToDo
  GetWaiterToDos = getWaiterToDos
}

let inMemoryQueries = {
  Table = tableQueries
  ToDo = toDoQueries
}

let inMemoryActions = {
  Table = tableActions
  Chef = chefActions
  Waiter = waiterActions
  Cashier = cashierActions
}