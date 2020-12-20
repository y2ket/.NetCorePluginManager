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
 *  Product:  DynamicContent.Plugin
 *  
 *  File: DynamicContentModel.cs
 *
 *  Purpose:  Dynamic content result model
 *
 *  Date        Name                Reason
 *  19/12/2020  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

namespace DynamicContent.Plugin.Model
{
    public sealed class DynamicContentModel
    {
        public DynamicContentModel()
        {
            Success = false;
            Data = String.Empty;
        }

        public DynamicContentModel(bool success)
        {
            Success = success;
            Data = String.Empty;
        }

        public DynamicContentModel(string data)
        {
            if (String.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            Success = true;
            Data = data;
        }

        public bool Success { get; set; }

        public string Data { get; set; }
    }
}
