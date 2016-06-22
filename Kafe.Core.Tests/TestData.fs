module TestData

open Domain
open System

let tab = {Id = Guid.NewGuid(); TableNumber = 1}

let coke = Drink {
  MenuNumber = 1
  Name = "Coke"
  Price = 1.5m
}
let lemonade = Drink {
  MenuNumber = 8
  Name = "Lemonade"
  Price = 1.1m
}
let appleJuice = Drink {
  MenuNumber = 7
  Name = "Apple Juice"
  Price = 2.1m
}

let order = {Tab = tab; Foods = []; Drinks = []}

let salad = Food {
  MenuNumber = 1
  Name = "Salad"
  Price = 3.1m
}
let pizza = Food {
  MenuNumber = 1
  Name = "Pizza"
  Price = 4.1m
}

let foodPrice (Food food) = food.Price
let drinkPrice (Drink drink) = drink.Price