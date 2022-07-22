﻿using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using GTANetworkAPI;

namespace VMP_CNR.Module.NSA
{
    public class NSALogger : Module<NSALogger>
    {
        string BankTable = "nsa_bankhistory";
        string BankTransferTable = "nsa_banktransferhistory";

        public void LogBank(DbPlayer dbPlayer, int amount, string notice)
        {
            NAPI.Task.Run(() =>
            {
                MySQLHandler.ExecuteAsync($"INSERT INTO `{BankTable}` (`player`, `amount`, `notice`) VALUES ('{dbPlayer.Id}', '{amount}', '{notice}')");
            });

        }

        public void LogBankTransfer(DbPlayer dbPlayer, DbPlayer destPlayer, int amount, string notice)
            {
                NAPI.Task.Run(() =>
                {
                    MySQLHandler.ExecuteAsync($"INSERT INTO `{BankTransferTable}` (`player1`, `player2`, `amount`, `notice`) VALUES ('{dbPlayer.Id}', '{destPlayer.Id}', '{amount}', '{notice}')");
                });
                }
    }
}
