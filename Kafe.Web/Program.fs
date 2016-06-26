module Program

open Suave
open Suave.Web
open Suave.Successful
open Suave.RequestErrors
open Suave.Operators
open Suave.Filters
open CommandApi
open InMemory
open System.Text
open Chessie.ErrorHandling

let commandApiHandler eventStore (context : HttpContext) = async {
  let payload = Encoding.UTF8.GetString context.request.rawForm
  let! response = handleCommandRequest inMemoryQueries eventStore payload
  match response with
  | Ok ((state, events), _) -> return! OK (sprintf "%A" state) context
  | Bad (err) -> return! BAD_REQUEST err.Head.Message context
}

let commandApi eventStore =
  path "/command"
  >=> POST
  >=> commandApiHandler eventStore

[<EntryPoint>]
let main argv = 
  let app =
    let eventStore = inMemoryEventStore ()
    choose [
      commandApi eventStore
    ]
  let config = {defaultConfig with bindings = [HttpBinding.mkSimple HTTP "127.0.0.1" 8083]}
  startWebServer config app
  0 // return an integer exit code
