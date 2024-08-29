namespace SkipIntro

open System.Reflection
open BepInEx
open BepInEx.Configuration
open HarmonyLib
open BepInEx.Logging
open Operators

module SkipIntro =
    let mutable internal Plugin: BaseUnityPlugin option = None
    let mutable internal Logger: ManualLogSource option = None
    let mutable internal Patcher: Harmony option = None
    
    let mutable internal LoadEmptyScene: ConfigEntry<bool> option = None

open SkipIntro

[<BepInPlugin("wwwDayDream.SkipIntro", "SkipIntro", "0.0.2")>]
type SkipIntroPlugin() =
    inherit BaseUnityPlugin()
    
    member this.Awake() =
        Plugin <- Some this
        Logger <- Some this.Logger
        Patcher <- Harmony(this.Info.Metadata.GUID) |> Some
        
        LoadEmptyScene <- this.Config.Bind<bool>("General", "LoadEmptyScene", true, "Should we load an empty scene to help save time and avoid preloading for longer?") |> Some
        
        Patcher |??>! (_.PatchAll())
        
[<HarmonyPatch(typeof<Preloader>)>]
module PreloaderPatcher =
    [<HarmonyPatch("MoveToNextState")>] [<HarmonyPrefix>]
    let PrepareStartOptionsOverride (__instance: Preloader) =
        typeof<Preloader>.GetMethod("DoMoveToMainMenu",  BindingFlags.Instance ||| BindingFlags.NonPublic).Invoke(__instance, [||]) |> ignore
        false

// Reduces startup time
[<HarmonyPatch(typeof<MenuBackgroundSavegame>)>]
module MenuBackgroundSaveGamePatcher =
    [<HarmonyPatch("PrepareStartOptions")>] [<HarmonyPrefix>]
    let PrepareStartOptionsOverride (__instance: MenuBackgroundSavegame) (gameData: IGameData) (__result : IGameStartOptions byref) =
        if not LoadEmptyScene.IsSome || not LoadEmptyScene.Value.Value then
            true
        else
            __result <- typeof<MenuBackgroundSavegame>.GetMethod("PrepareCleanMenuSavegame", BindingFlags.Instance ||| BindingFlags.NonPublic).Invoke(__instance, [|gameData|]) :?> IGameStartOptions
            false
        