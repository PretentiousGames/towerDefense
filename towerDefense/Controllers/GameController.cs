﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using towerDefense.Hubs;
using TowerDefense.Business;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace towerDefense.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index(string gameName)
        {
            var game = GameManager.GetGame(gameName);
            if (game != null)
            {
                try
                {
                    game.RawJson = JsonConvert.SerializeObject(game);
                }
                catch (Exception e)
                {
                    game.RawJson = JsonConvert.SerializeObject(new { error = e.Message });
                }
            }

            return game == null ? (ActionResult)RedirectToAction("Index", "Home") : View("Index", game);
        }

        [HttpPost]
        public ActionResult DeleteTank(string gamename, string playername, string tankname)
        {
            var game = GameManager.GetGame(gamename);

            if (game != null && (game.GameThread == null || !game.GameThread.IsAlive))
            {
                Player player = game.Players.FirstOrDefault(x => x.Name == playername);

                if (player != null)
                {
                    var tank = player.Tanks.Find(t => t.Name == tankname);

                    if (tank != null)
                    {
                        if (player.Tanks.Count == 1)
                        {
                            game.Players.Remove(player);
                        }
                        else
                        {
                           player.Tanks.Remove(tank); 
                        }
                    }
                }

                return Json(new { success = true });
            }

            return Json(new {error = "You cannot delete tanks while a game is in progress." });
        }

        [HttpPost]
        public ActionResult UploadFile(string playername, string gamename, HttpPostedFileBase file)
        {
            byte[] data = new byte[file.InputStream.Length];
            file.InputStream.Read(data, 0, data.Length);
            var assembly = Assembly.Load(data);

            var types = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ITank)));

            var game = GameManager.GetGame(gamename);
            if (game != null)
            {
                foreach (var type in types)
                {
                    try
                    {
                        var constructor = type.GetConstructor(new Type[] {});

                        var newTank = (Tank) constructor.Invoke(new object[] {});

                        try
                        {
                            JsonConvert.SerializeObject(newTank);
                        }
                        catch (Exception e)
                        {
                            return Json(new { error = "Failed to upload tank. " + e.Message });
                        }

                        Player player = game.Players.FirstOrDefault(x => x.Name == playername);

                        if (player != null)
                        {
                            var tank = player.Tanks.Find(t => t.Name == newTank.Name);

                            if (tank != null)
                            {
                                player.Tanks.Remove(tank);
                            }

                            player.Tanks.Add(newTank);
                        }
                        else
                        {
                            List<Tank> tanks = new List<Tank> {newTank};
                            game.Players.Add(new Player
                            {
                                Name = playername,
                                Tanks = tanks
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        return Json(new { error = "Failed to upload tank. " + e.Message });
                    }
                }
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult Start(string gameName)
        {
            var game = GameManager.GetGame(gameName);

            if (game == null)
            {
                return RedirectToAction("../Game/" + gameName);
            }

            IHubConnectionContext<dynamic> clients = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients;
            GameBroadcaster gameBroadcaster = new GameBroadcaster(clients);
            game.StartNewGame(gameBroadcaster);

            return Json("start");
        }

        [HttpPost]
        public ActionResult StopGame(string gameName)
        {
            var game = GameManager.GetGame(gameName);

            if (game == null)
            {
                return RedirectToAction("../Game/" + gameName);
            }

            IHubConnectionContext<dynamic> clients = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients;
            GameBroadcaster gameBroadcaster = new GameBroadcaster(clients);
            game.ClearGameOut(gameBroadcaster);

            return Json("start");
        }
    }
}