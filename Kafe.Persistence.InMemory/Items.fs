module Items

open System.Collections.Generic
open Domain
open Queries

let private foods =
  let dict = new Dictionary<int, Food>()
  dict.Add(1, Food {
    MenuNumber = 1
    Price = 1m
    Name = "Pecel"
  })
  dict.Add(2, Food {
    MenuNumber = 2
    Price = 2m
    Name = "Karedok"
  })
  dict.Add(3, Food {
    MenuNumber = 3
    Price = 3m
    Name = "Gado gado"
  })
  dict

let private drinks =
  let dict = new Dictionary<int, Drink>()
  dict.Add(1, Drink {
    MenuNumber = 1
    Price = 1m
    Name = "Teh Tawar"
  })
  dict.Add(2, Drink {
    MenuNumber = 2
    Price = 2m
    Name = "Teh Manis"
  })
  dict.Add(3, Drink {
    MenuNumber = 3
    Price = 3m
    Name = "Kopi hitam"
  })
  dict

let private getItems<'a> (dict : Dictionary<int, 'a>) keys =
  let invalidKeys = keys |> Array.except dict.Keys
  if Array.isEmpty invalidKeys then
    keys
    |> Array.map (fun n -> dict.[n])
    |> Array.toList
    |> Choice1Of2
  else
    invalidKeys |> Choice2Of2

let getItem<'a> (dict : Dictionary<int, 'a>) key =
  if dict.ContainsKey key then
    dict.Keys |> Some
  else
    None

let getFoodsByMenuNumbers keys =
  getItems foods keys |> async.Return

let getFoodByMenuNumber key =
  getItem foods key |> async.Return

let getDrinksByMenuNumbers keys =
  getItems drinks keys |> async.Return

let getDrinkByMenuNumber key =
  getItem drinks key |> async.Return

let getFoods () =
  foods.Values |> Seq.toList |> async.Return

let getDrinks () =
  drinks.Values |> Seq.toList |> async.Return

(*
let foodQUeries = {
  GetFoods = getFoods
  GetFoodByMenuNumber = getFoodByMenuNumber
  GetFoodsByMenuNumbers = getFoodsByMenuNumbers
}

let drinkQUeries = {
  GetFoods = getDrinks
  getDrinkByMenuNumber = getDrinkByMenuNumber
  GetDrinksByMenuNumbers = getDrinksByMenuNumbers
}
*)