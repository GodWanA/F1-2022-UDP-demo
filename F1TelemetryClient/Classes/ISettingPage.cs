using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1TelemetryApp.Classes
{
    internal interface ISettingPage
    {
        /// <summary>
        /// Betölti a beállítások adott füléhez tatozó adatokat.
        /// </summary>
        public void LoadData();

        /// <summary>
        /// Elmenti az adott beállításon tárolt adatokat.
        /// </summary>
        public void SaveData();
    }
}
