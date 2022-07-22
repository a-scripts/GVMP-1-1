using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMP_CNR.Handler;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Doors;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Jailescape;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.JumpPoints;
using VMP_CNR.Module.Players.PlayerAnimations;
using VMP_CNR.Module.Spawners;
using VMP_CNR.Module.Teams;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> JailTunnel(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle || iPlayer.IsTied || iPlayer.IsCuffed || iPlayer.IsACop()) return false;

            if (JailescapeModule.Instance.IsInTunnelDiggingRange(iPlayer))
            {
                if(!JailescapeModule.Instance.CanTunnelDigged())
                {
                    iPlayer.SendNewNotification("Es kann zurzeit kein Tunnel gegraben werden, das wäre zu auffällig");
                    return false;
                }
                Chats.sendProgressBar(iPlayer, 45000);

                iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_bum_wash@male@high@idle_a", "idle_a");
                iPlayer.Player.TriggerEvent("freezePlayer", true);
                iPlayer.SetData("userCannotInterrupt", true);

                await Task.Delay(45000);

                iPlayer.ResetData("userCannotInterrupt");
                if (JailescapeModule.jailTunnelEntrance != null)
                {
                    JumpPointModule.Instance.jumpPoints.Remove(JailescapeModule.jailTunnelEntrance.Id);
                    if(JailescapeModule.jailTunnelEntrance.ColShape != null)
                    {
                        JailescapeModule.jailTunnelEntrance.ColShape?.Delete();
                    }
                }

                if (JailescapeModule.jailTunnelEscape != null)
                {
                    JumpPointModule.Instance.jumpPoints.Remove(JailescapeModule.jailTunnelEscape.Id);
                    if (JailescapeModule.jailTunnelEscape.ColShape != null)
                    {
                        JailescapeModule.jailTunnelEscape.ColShape?.Delete();
                    }
                }

                Random rand = new Random();
                Jailtunnel tunnel = JailescapeModule.Instance.GetAll().ToList().ElementAt(rand.Next(0, JailescapeModule.Instance.GetAll().ToList().Count)).Value;

                NAPI.Task.Run(() =>
                {
                    JailescapeModule.jailTunnelEntrance = new JumpPoint
                    {
                        Id = JumpPointModule.Instance.jumpPoints.Last().Key + 1,
                        Name = "Tunnel",
                        Position = iPlayer.Player.Position,
                        AdminUnbreakable = true,
                        DestinationId = JumpPointModule.Instance.jumpPoints.Last().Key + 2,
                        Dimension = 0,
                        Heading = iPlayer.Player.Heading,
                        InsideVehicle = false,
                        LastBreak = DateTime.Now.Add(new TimeSpan(0, -5, 0)),
                        Locked = false,
                        Range = 2,
                        Teams = new HashSet<Team>(),
                        Unbreakable = true
                    };

                    JailescapeModule.jailTunnelEscape = new JumpPoint
                    {
                        Id = JumpPointModule.Instance.jumpPoints.Last().Key + 2,
                        Name = "Tunnel",
                        Position = tunnel.Position,
                        AdminUnbreakable = true,
                        DestinationId = JumpPointModule.Instance.jumpPoints.Last().Key + 1,
                        Dimension = 0,
                        Heading = tunnel.Heading,
                        InsideVehicle = false,
                        LastBreak = DateTime.Now.Add(new TimeSpan(0, -5, 0)),
                        Locked = false,
                        Range = 2,
                        Teams = new HashSet<Team>(),
                        Unbreakable = true
                    };

                    JailescapeModule.jailTunnelEntrance.Destination = JailescapeModule.jailTunnelEscape;
                    JailescapeModule.jailTunnelEscape.Destination = JailescapeModule.jailTunnelEntrance;

                    JailescapeModule.jailTunnelEntrance.ColShape = ColShapes.Create(JailescapeModule.jailTunnelEntrance.Position, 2, 0);
                    JailescapeModule.jailTunnelEscape.ColShape = ColShapes.Create(JailescapeModule.jailTunnelEscape.Position, 2, 0);
                    JailescapeModule.jailTunnelEntrance.ColShape.SetData("jumpPointId", JailescapeModule.jailTunnelEntrance.Id);
                    JailescapeModule.jailTunnelEscape.ColShape.SetData("jumpPointId", JailescapeModule.jailTunnelEscape.Id);

                    JumpPointModule.Instance.jumpPoints.Add(JailescapeModule.jailTunnelEntrance.Id, JailescapeModule.jailTunnelEntrance);
                    JumpPointModule.Instance.jumpPoints.Add(JailescapeModule.jailTunnelEscape.Id, JailescapeModule.jailTunnelEscape);
                });
                
                iPlayer.Player.TriggerEvent("freezePlayer", false);
                iPlayer.SendNewNotification("Du hast den Tunnel gegraben, beeil dich er ist max 2 Minuten geöffnet!");
                //iPlayer.SendNewNotification("Gehen zu viele Menschen durch, kann der Tunnel einbrechen!");

                TeamModule.Instance.Get((int)teams.TEAM_ARMY).SendNotification("Achtung: Es wurde ein Tunnelbau im Staatsgefängnis bemerkt!");

                iPlayer.StopAnimation();

                JailescapeModule.Instance.SetLastDigged();
                return true;
            }
            else iPlayer.SendNewNotification("Hier kann kein Tunnel gegraben werden!");
            return false;
            
        }
    }
}