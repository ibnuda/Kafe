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