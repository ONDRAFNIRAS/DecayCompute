using System;
using System.Collections.Generic;
using System.Text;

/// <licence>
/// This file is part of CalcDecay.
///
///CalcDecay is free software: you can redistribute it and/or modify
///it under the terms of the GNU General Public License as published by
///the Free Software Foundation, either version 3 of the License, or
///(at your option) any later version.
///
///CalcDecay is distributed in the hope that it will be useful,
///but WITHOUT ANY WARRANTY; without even the implied warranty of
///MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
///GNU General Public License for more details.
///
///You should have received a copy of the GNU General Public License
///along with Foobar.  If not, see <https://www.gnu.org/licenses/>.
///</licence>
///<summary>
/// Utility functions
///</summary>
///
/// Copyright ONDRAF/NIRAS 2008,2012,2021
/// 
namespace MDecay.Net
{
    class Utils
    {
        public Boolean RunOnLinux()
        {
            if ((System.Environment.OSVersion.Platform.ToString() == "128") ||
                (System.Environment.OSVersion.Platform.ToString().ToLower() == "unix")) { return true; }
            else { return false; }
        }
        public string OSType()
        {
            return System.Environment.OSVersion.Platform.ToString();
        }
    }
}
