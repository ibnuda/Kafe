module Queries

open ReadModels
open Domain
open States
open System

type TableQueries = {
  GetTables : unit -> Async<Table list>
  GetTableByTableNumber : int -> Async<Table option>
  GetTableByTabId : Guid -> Async<Table option>
}

type ToDoQueries = {
  GetChefToDos : unit -> Async<ChefToDo list>
  GetWaiterToDos : unit -> Async<WaiterToDo list>
  GetCashierToDos : unit -> Async<Payment list>
}

type FoodQueries = {
  GetFoodByMenuNumber : int -> Async<Food option>
  GetFoodsByMenuNumbers : int[] -> Async<Choice<Food list, int[]>>
}

type DrinkQueries = {
  GetDrinksByMenuNumbers : int[] -> Async<Choice<Drink list, int[]>>
  GetDrinkByMenuNumber : int -> Async<Drink option>
}

type Queries = {
  Table : TableQueries
  ToDo : ToDoQueries
  Food : FoodQueries
  Drink : DrinkQueries
}