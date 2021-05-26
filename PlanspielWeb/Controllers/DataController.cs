﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planspiel.Models;
using DAL.Models;

namespace PlanspielWeb.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase {
        [HttpPost]
        public void Post([FromBody] SaveDataModel data) {
            if (data == null)
                return;

            new SaveDataRepository(Database.Instance).AddOrIgnore(new SaveData {
                SteamID = data.SteamID,
                Date = new Date(data.UnixDays),
                Profit = data.Profit,
                CompanyValue = data.CompanyValue,
                //DemandSatisfaction = data.DemandSatisfaction,
                MachineUptime = double.IsNaN(data.MachineUptime) ? 1 : data.MachineUptime,
                AbleToPayLoansBack = data.AbleToPayLoansBack,
                AveragePollution = double.IsNaN(data.AveragePollution) ? 0 : data.AveragePollution,
                BuildingCount = data.BuildingCount,
                UnlockedResearchCount = data.UnlockedResearchCount,
                RegionCount = data.RegionCount
            });
        }
    }
}