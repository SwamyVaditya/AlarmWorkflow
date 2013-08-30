﻿// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Provides information about the settings, to be used in UIs or anyone who needs it.
    /// </summary>
    public sealed class SettingsDisplayConfiguration
    {
        #region Constants

        /// <summary>
        /// Defines the name of the embedded resource file representing a display configuration.
        /// </summary>
        internal const string EmbeddedResourceFileName = "settings.info.xml";

        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection of all registered identifiers.
        /// </summary>
        public List<IdentifierInfo> Identifiers { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDisplayConfiguration"/> class.
        /// </summary>
        internal SettingsDisplayConfiguration()
        {
            Identifiers = new List<IdentifierInfo>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses the given XDocument and adds its configuration to the current configuration.
        /// </summary>
        /// <param name="doc"></param>
        internal void ParseAdd(XDocument doc)
        {
            if (!doc.IsXmlValid(Properties.Resources.SettingsInfoXsd))
            {
                return;
            }

            foreach (XElement identifierE in doc.Root.Elements("Identifier"))
            {
                IdentifierInfo identifier = new IdentifierInfo();
                identifier.Name = identifierE.Attribute("Name").Value;

                identifier.DisplayText = identifierE.TryGetAttributeValue("DisplayText", identifier.Name);
                identifier.Description = identifierE.TryGetAttributeValue("Description", null);
                identifier.Order = identifierE.TryGetAttributeValue("Order", 0);
                identifier.Parent = identifierE.TryGetAttributeValue("Parent", null);

                foreach (XElement settingE in identifierE.Elements("Setting"))
                {
                    SettingInfo setting = new SettingInfo();
                    setting.Name = settingE.Attribute("Name").Value;

                    setting.Category = settingE.TryGetAttributeValue("Category", null);
                    setting.DisplayText = settingE.TryGetAttributeValue("DisplayText", setting.Name);
                    setting.Description = settingE.TryGetAttributeValue("Description", null);
                    setting.Order = settingE.TryGetAttributeValue("Order", 0);
                    setting.Editor = settingE.TryGetAttributeValue("Editor", null);
                    setting.EditorParameter = settingE.TryGetAttributeValue("EditorParameter", null);

                    identifier.Settings.Add(setting);
                }

                this.Identifiers.Add(identifier);
            }
        }

        /// <summary>
        /// Tries to find the <see cref="IdentifierInfo"/> for the section by the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IdentifierInfo GetIdentifier(string name)
        {
            return Identifiers.Find(i => i.Name == name);
        }

        /// <summary>
        /// Tries to find the <see cref="SettingInfo"/> for the setting within the given section and name.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SettingInfo GetSetting(string identifier, string name)
        {
            IdentifierInfo inf = GetIdentifier(identifier);
            if (inf != null)
            {
                return inf.Settings.Find(s => s.Name == name);
            }
            return null;
        }

        #endregion
    }
}