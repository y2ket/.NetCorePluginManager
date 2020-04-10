﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  .Net Core Plugin Manager is distributed under the GNU General Public License version 3 and  
 *  is also available under alternative licenses negotiated directly with Simon Carter.  
 *  If you obtained Service Manager under the GPL, then the GPL applies to all loadable 
 *  Service Manager modules used on your system as well. The GPL (version 3) is 
 *  available at https://opensource.org/licenses/GPL-3.0
 *
 *  This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 *  See the GNU General Public License for more details.
 *
 *  The Original Code was created by Simon Carter (s1cart3r@gmail.com)
 *
 *  Copyright (c) 2018 - 2020 Simon Carter.  All Rights Reserved.
 *
 *  Product:  AspNetCore.PluginManager.DemoWebsite
 *  
 *  File: AllTimings.cs
 *
 *  Purpose:  Displays all timings in a single grid for easy overview
 *
 *  Date        Name                Reason
 *  10/04/2020  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;

using SharedPluginFeatures;

using PluginManager.Abstractions;
using System.Linq;
using System.Text;

#pragma warning disable CS1591

namespace SystemAdmin.Plugin.Classes.MenuItems
{
    public class AllTimings : SystemAdminSubMenu
    {
        #region Private Members

        private const int MinimumResetMilliseconds = 1500;

        private static List<SystemAdminSubMenu> _timingItems;
        private static DateTime _lastRun = DateTime.MinValue;
        private static string _lastData;

        private readonly IPluginClassesService _pluginClassesService;
        private readonly ISystemAdminHelperService _systemAdminHelperService;

        #endregion Private Members

        #region Constructors

        public AllTimings(IPluginClassesService pluginClassesService, ISystemAdminHelperService systemAdminHelperService)
        {
            _pluginClassesService = pluginClassesService ?? throw new ArgumentNullException(nameof(pluginClassesService));
            _systemAdminHelperService = systemAdminHelperService ?? throw new ArgumentNullException(nameof(systemAdminHelperService));
        }

        #endregion Constructors

        #region SystemAdminSubMenu Methods

        public override String Action()
        {
            return String.Empty;
        }

        public override String Area()
        {
            return String.Empty;
        }

        public override String Controller()
        {
            return String.Empty;
        }

        public override String Data()
        {
            TimeSpan span = DateTime.Now - _lastRun;

            if (span.TotalMilliseconds > MinimumResetMilliseconds)
            {
                _lastRun = DateTime.Now;

                if (_timingItems == null)
                {
                    _timingItems = _systemAdminHelperService.GetSubMenuItems(nameof(Languages.LanguageStrings.Timings))
                        .Where(t => t.UniqueId != UniqueId).ToList();
                }

                StringBuilder Result = new StringBuilder("Name|Total Requests|Fastest|Slowest|Average|Trimmed Avg ms|Total ms", 2048);

                foreach (SystemAdminSubMenu item in _timingItems)
                {
                    Result.Append('\r');
                    Result.Append(item.Name());
                    string[] data = item.Data().Split('\r');

                    for (int i = 1; i < data.Length; i++)
                    {
                        Result.Append('|');
                        Result.Append(data[i].Split('|')[1]);
                    }
                }

                _lastData = Result.ToString();
            }

            return _lastData;
        }

        public override string Image()
        {
            return "stopwatch";
        }

        public override Enums.SystemAdminMenuType MenuType()
        {
            return Enums.SystemAdminMenuType.Grid;
        }

        public override string Name()
        {
            return nameof(Languages.LanguageStrings.AllTimings);
        }

        public override string ParentMenuName()
        {
            return nameof(Languages.LanguageStrings.Timings);
        }

        public override int SortOrder()
        {
            return int.MinValue;
        }

        #endregion SystemAdminSubMenu Methods
    }
}

#pragma warning restore CS1591