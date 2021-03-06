(*
From the article "Low risk ways to use F# at work"
http://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work/
*)


(* ======================================================
Get data from Freebase

Freebase Type Provider page: https://fsharp.github.io/FSharp.Data/library/Freebase.html

You'll probably need an API key if you're playing around a lot!
https://developers.google.com/console/help/?csw=1#activatingapis

See the F# Data Access page for more (http://fsharp.org/data-access/)

 === Setup ===
 1. Install Chocolately from http://chocolatey.org/
 2. Install NuGet command line
    cinst nuget.commandline
 3. Install FSharp.Data in same directory as script
    nuget install FSharp.Data -o Packages -ExcludeVersion 

====================================================== *)

// sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

// Requires FSharp.Data under script directory 
//    nuget install FSharp.Data -o Packages -ExcludeVersion  
#r @"Packages\FSharp.Data\lib\net40\FSharp.Data.dll"
open FSharp.Data


// without a key
let data = FreebaseData.GetDataContext()

// with a key
(*
[<Literal>]
let FreebaseApiKey = "<enter your freebase-enabled google API key here>"
type FreebaseDataWithKey = FreebaseDataProvider<Key=FreebaseApiKey>
let data = FreebaseDataWithKey.GetDataContext()
*)

// =================================
// List the US presidents
// =================================

data.Society.Government.``US Presidents``
|> Seq.map (fun p -> p.``President number`` |> Seq.head, p.Name)
|> Seq.sortBy fst
|> Seq.iter (fun (n,name) -> printfn "%s was number %i" name n )

// =================================
// What awards did Casablanca win?
// =================================

data.``Arts and Entertainment``.Film.Films.IndividualsAZ.C.Casablanca.``Awards Won``
|> Seq.map (fun award -> award.Year, award.``Award category``.Name)
|> Seq.sortBy fst
|> Seq.iter (fun (year,name) -> printfn "%s -- %s" year name)


// =================================
// Generate random data
//
// Based on a tweet from Kit Eason
// http://twitter.com/kitlovesfsharp/status/296240699735695360
// =================================


let randomElement =
    let random = new System.Random()
    fun (arr:string array) -> arr.[random.Next(arr.Length)]

let surnames = 
    FreebaseData.GetDataContext().Society.People.``Family names``
    |> Seq.truncate 1000
    |> Seq.map (fun name -> name.Name)
    |> Array.ofSeq
            
let firstnames = 
    FreebaseData.GetDataContext().Society.Celebrities.Celebrities
    |> Seq.truncate 1000
    |> Seq.map (fun celeb -> celeb.Name.Split([|' '|]).[0])
    |> Array.ofSeq

// generate random people and print
type Person = {Forename:string; Surname:string}
Seq.init 10 ( fun _ -> 
    {Forename = (randomElement firstnames); 
     Surname = (randomElement surnames) }
     )
|> Seq.iter (printfn "%A")

(*
// generate random people and insert into database
Seq.init count ( fun _ -> 
  dbSchema.ServiceTypes.People(
      Forename = (randomElement firstnames), 
      Surname = (randomElement surnames))
|> db.People.InsertAllOnSubmit         
*)