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
 *  Product:  AspNetCore.PluginManager.Tests
 *  
 *  File: MockLoadData.cs
 *
 *  Purpose:  Mock ILoadData class
 *
 *  Date        Name                Reason
 *  29/11/2020  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using DynamicContent.Plugin.Templates;
using Middleware;
using Middleware.DynamicContent;
using PluginManager.Abstractions;
using SharedPluginFeatures.DynamicContent;

namespace AspNetCore.PluginManager.DemoWebsite.Classes.Mocks
{
    public class MockDynamicContentProvider : IDynamicContentProvider
    {
        #region Private Members

        private readonly IPluginClassesService _pluginClassesService;
        private IDynamicContentPage _dynamicContentPage1;
        private IDynamicContentPage _dynamicContentPage2;
        private IDynamicContentPage _dynamicContentPage3;
        private IDynamicContentPage _dynamicContentPage10;

        #endregion Private Members

        #region Constructors

        public MockDynamicContentProvider(IPluginClassesService pluginClassesService)
        {
            _pluginClassesService = pluginClassesService ?? throw new ArgumentNullException(nameof(pluginClassesService));
        }

        #endregion Constructors

        #region IDynamicContentProvider Members

        public String RenderDynamicPage(DynamicContentTemplate contentTemplate)
        {
            throw new NotImplementedException();
        }

        public List<LookupListItem> GetActiveCustomPages()
        {
            List<LookupListItem> Result = new List<LookupListItem>();

            Result.Add(new LookupListItem(GetPage1().Id, GetPage1().Name));
            Result.Add(new LookupListItem(GetPage2().Id, GetPage2().Name));
            Result.Add(new LookupListItem(GetPage3().Id, GetPage3().Name));
            Result.Add(new LookupListItem(GetPage10().Id, GetPage10().Name));

            return Result;
        }

        public IDynamicContentPage GetCustomPage(int id)
        {
            if (id == 1)
            {
                return GetPage1();
            }

            if (id == 2)
            {
                return GetPage2();
            }

            if (id == 3)
            {
                return GetPage3();
            }

            if (id == 10)
            {
                return GetPage10();
            }

            return null;
        }

        public List<DynamicContentTemplate> Templates()
        {
            return _pluginClassesService.GetPluginClasses<DynamicContentTemplate>();
        }

        #endregion IDynamicContentProvider Members

        #region Private Methods

        private IDynamicContentPage GetPage1()
        {
            if (_dynamicContentPage1 == null)
            {
                _dynamicContentPage1 = new DynamicContentPage()
                {
                    Id = 1,
                    Name = "Custom Page 1",
                };

                HtmlTextTemplate htmlLayout1 = new HtmlTextTemplate()
                {
                    UniqueId = "1",
                    SortOrder = 0,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 12,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is <br />html over<br />three lines</p>"
                };

                _dynamicContentPage1.Content.Add(htmlLayout1);
            }

            return _dynamicContentPage1;
        }

        private IDynamicContentPage GetPage2()
        {
            if (_dynamicContentPage2 == null)
            {
                _dynamicContentPage2 = new DynamicContentPage()
                {
                    Id = 2,
                    Name = "Custom Page 2",
                };

                HtmlTextTemplate htmlLayout1 = new HtmlTextTemplate()
                {
                    UniqueId = "control-1",
                    SortOrder = 0,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 12,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is <br />html over<br />three lines</p>"
                };

                _dynamicContentPage2.Content.Add(htmlLayout1);

                HtmlTextTemplate htmlLayout2 = new HtmlTextTemplate()
                {
                    UniqueId = "control-2",
                    SortOrder = 2,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />over two lines</p>"
                };

                _dynamicContentPage2.Content.Add(htmlLayout2);
            }

            return _dynamicContentPage2;
        }

        private IDynamicContentPage GetPage3()
        {
            if (_dynamicContentPage3 == null)
            {
                _dynamicContentPage3 = new DynamicContentPage()
                {
                    Id = 3,
                    Name = "Custom Page 3",
                };

                HtmlTextTemplate htmlLayout1 = new HtmlTextTemplate()
                {
                    UniqueId = "control-1",
                    SortOrder = 0,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 12,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is <br />html over<br />three lines</p>"
                };

                _dynamicContentPage3.Content.Add(htmlLayout1);

                SpacerTemplate spacerTemplate1 = new SpacerTemplate()
                { 
                    SortOrder = 1,
                    Width = 8
                };

                _dynamicContentPage3.Content.Add(spacerTemplate1);

                HtmlTextTemplate htmlLayout2 = new HtmlTextTemplate()
                {
                    UniqueId = "control-2",
                    SortOrder = 2,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />over two lines</p>"
                };

                _dynamicContentPage3.Content.Add(htmlLayout2);
            }

            return _dynamicContentPage3;
        }

        private IDynamicContentPage GetPage10()
        {
            if (_dynamicContentPage10 == null)
            {
                _dynamicContentPage10 = new DynamicContentPage()
                {
                    Id = 10,
                    Name = "Custom Page 10",
                };

                HtmlTextTemplate htmlLayout1 = new HtmlTextTemplate()
                {
                    UniqueId = "control-1",
                    SortOrder = 0,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 12,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is <br />html over<br />three lines</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout1);

                HtmlTextTemplate htmlLayout2 = new HtmlTextTemplate()
                {
                    UniqueId = "control-2",
                    SortOrder = 2,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />Content 2</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout2);

                HtmlTextTemplate htmlLayout3 = new HtmlTextTemplate()
                {
                    UniqueId = "control-3",
                    SortOrder = 9,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />Content 3</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout3);

                HtmlTextTemplate htmlLayout4 = new HtmlTextTemplate()
                {
                    UniqueId = "control-4",
                    SortOrder = 8,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />Content 4</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout4);

                HtmlTextTemplate htmlLayout5 = new HtmlTextTemplate()
                {
                    UniqueId = "control-5",
                    SortOrder = 7,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />Content 5</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout5);

                HtmlTextTemplate htmlLayout6 = new HtmlTextTemplate()
                {
                    UniqueId = "control-6",
                    SortOrder = 6,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />Content 6</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout6);

                HtmlTextTemplate htmlLayout7 = new HtmlTextTemplate()
                {
                    UniqueId = "control-7",
                    SortOrder = 5,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />Content 7</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout7);

                HtmlTextTemplate htmlLayout8 = new HtmlTextTemplate()
                {
                    UniqueId = "control-8",
                    SortOrder = 4,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />Content 8</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout8);

                HtmlTextTemplate htmlLayout9 = new HtmlTextTemplate()
                {
                    UniqueId = "control-9",
                    SortOrder = 3,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />Content 9</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout9);

                HtmlTextTemplate htmlLayout10 = new HtmlTextTemplate()
                {
                    UniqueId = "control-10",
                    SortOrder = 20,
                    WidthType = SharedPluginFeatures.DynamicContentWidthType.Columns,
                    Width = 4,
                    HeightType = SharedPluginFeatures.DynamicContentHeightType.Automatic,
                    Data = "<p>This is html<br />Content 10</p>"
                };

                _dynamicContentPage10.Content.Add(htmlLayout10);
            }

            return _dynamicContentPage10;
        }

        #endregion Private Methods
    }
}