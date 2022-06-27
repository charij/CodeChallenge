namespace PlanetWars.Bot.FSharp

module AgentFSharp =
    open PlanetWars.Shared
    open System

    type FleetMove = {
        SourcePlanetId : int
        TargetPlanetId : int
        NumberOfShips : int
    }

    let logPLanets title planets =
        printf "%s %s: " Environment.NewLine title
        planets |> List.iter (fun (p:Planet) -> printf " %i " p.Id )

    let processGameState myId (gameState:StatusResult) : FleetMove list =
        let enemyPlanets = 
            gameState.Planets 
            |> Seq.toList 
            |> List.filter (fun (p:Planet) -> p.OwnerId <> myId) 
        
        let ownedPlanets = 
            gameState.Planets 
            |> Seq.toList 
            |> List.filter(fun (p:Planet) -> p.OwnerId = myId)     
        
        ownedPlanets |> logPLanets "Owned Planets"

        let createFleetMove (planet:Planet) =
            let closestPlanets planets =
                planets
                |> List.sortBy (fun (p:Planet) -> planet.Position.Distance(p.Position))
       
            let firstAttackablePlanet targetPlanets =
                targetPlanets
                |> List.tryFind (fun (p:Planet) -> planet.NumberOfShips > p.NumberOfShips )

            let createMove targetPlanet =
                match targetPlanet with
                | Some (p:Planet) -> Some {SourcePlanetId = planet.Id; TargetPlanetId = p.Id; NumberOfShips = p.NumberOfShips + 1}
                | None -> None
        
            enemyPlanets
            |> closestPlanets
            |> firstAttackablePlanet 
            |> createMove

        let rec generateMoves fleetMoves owned =
            match owned with
            | currentPlanet::remainingOwnedPlanets -> 
                match currentPlanet |> createFleetMove with
                | Some newMove -> remainingOwnedPlanets |> generateMoves (newMove::fleetMoves)
                | None -> remainingOwnedPlanets |> generateMoves fleetMoves
            | [] -> fleetMoves

        ownedPlanets
        |> generateMoves List.empty





