module Errors

open Domain

type Error =
| TabAlreadyOpened
| CanNotPlaceEmptyOrder
| CanNotOrderWithClosedTab
| OrderAlreadyPlaced
| CanNotServeNonOrderedDrink of Drink
| CanNotServeAlreadyServedDrink of Drink
| OrderAlreadyServed
| CanNotServeForNonPlacedOrder
| CanNotServeWithClosedTab
| CanNotPrepareNonOrderedFood of Food
| CanNotPrepareAlreadyPreparedFood of Food
| CanNotPrepareForNonPlacedOrder
| CanNotPrepareWithClosedTab
| CanNotServeNonPreparedFood of Food
| CanNotServeNonOrderedFood of Food
| CanNotServeAlreadyServedFood of Food
| InvalidPayment of decimal * decimal
| CanNotPayForNonServerOrder

let errorToString = function
| TabAlreadyOpened -> "Tab already opened."
| CanNotPlaceEmptyOrder -> "Can not place empty order."
| CanNotOrderWithClosedTab -> "Can not order with closed tab."
| OrderAlreadyPlaced -> "Order already placed."
| CanNotServeNonOrderedDrink (Drink drink) -> sprintf "%s (%d) is not an ordered drink." drink.Name drink.MenuNumber
| CanNotServeAlreadyServedDrink (Drink drink) -> sprintf "%s (%d) already served." drink.Name drink.MenuNumber
| OrderAlreadyServed -> "Order already served."
| CanNotServeForNonPlacedOrder -> "Can not serve for non placed order."
| CanNotServeWithClosedTab -> "Can not serve with closed tab."
| CanNotPrepareNonOrderedFood (Food food) -> sprintf "%s (%d) in not an ordered food." food.Name food.MenuNumber
| CanNotPrepareAlreadyPreparedFood (Food food) -> sprintf "%s (%d) already prepared." food.Name food.MenuNumber
| CanNotPrepareForNonPlacedOrder -> "Can not prepare for non placed order."
| CanNotPrepareWithClosedTab -> "Can not prepare with closed tab."
| CanNotServeNonPreparedFood (Food food) -> sprintf "Can not serve non prepared food. (%s (%d))" food.Name food.MenuNumber
| CanNotServeNonOrderedFood (Food food) -> sprintf "Can not serve non ordered food. (%s (%d))" food.Name food.MenuNumber
| CanNotServeAlreadyServedFood (Food food) -> sprintf "Can not serve already prepared food. (%s (%d))" food.Name food.MenuNumber
| InvalidPayment (expected, actual) -> sprintf "The actual amount (%f) is not equal to the expected (%f)." expected actual
| CanNotPayForNonServerOrder -> "Can not pay for non served order."