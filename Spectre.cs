using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NirondTools;

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
/// Objects Nuclide et Spectrum definitions
///</summary>
///
/// Copyright ONDRAF/NIRAS 2008,2012,2021
/// 
namespace NirondTools.MDecay
{
    /// <summary>
    /// Nuclide object
    /// </summary>
    public class Nuclide
    {
        private string mIsotop;
        private double mActivity;
        private double mlambda;
        private Int64 mHalfLive;
        private MDecay.enumDecayUnit mLiveUnit;

        /// <summary>
        /// Get/Set Isotop
        /// </summary>
        /// <remarks>Isotop can be given in different format, the object tries to normalize the isotop in the form xxx-000</remarks>
        public string Isotop
        {
            get { return mIsotop; }
            set { mIsotop = Normalize(value); }
        }
        /// <summary>
        /// Activity of the isotop
        /// </summary>
        public double Activity
        {
            get { return mActivity; }
            set { mActivity = value; }
        }

        /// <summary>
        /// exponential decay constant (lambda) (see Nuclide.xml)
        /// </summary>
        /// <see cref="https://en.wikipedia.org/wiki/Exponential_decay"/>
        public double lambda
        {
            get { return mlambda; }
            set { mlambda = value; }
        }
        /// <summary>
        /// Half Live od the isotop (see Nuclide.xml)
        /// </summary>
        public Int64 HalfLive
        {
            get { return mHalfLive; }
            set { mHalfLive = value; }
        }
        /// <summary>
        /// Isotop life unit (see Nuclide.xml)
        /// </summary>
        public enumDecayUnit LiveUnit
        {
            get { return mLiveUnit; }
            set { mLiveUnit = value; }
        }

        /// <summary>
        /// Normalize the isotop name
        /// </summary>
        /// <param name="Nucl">isotop identification</param>
        /// <returns>normalized isotop name</returns>
        private string Normalize(string Nucl)
        {
            const string letters = "_ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            const string digit = "_0123456789";
            string NormNucl = "";
            for (int i = 0; i < Nucl.Length ; i++)
            {
                if (letters.IndexOf(Nucl.Substring(i, 1)) > 0)
                {
                    NormNucl += Nucl.Substring(i, 1).ToUpper();
                    if (i<Nucl.Length-1 && (letters.IndexOf(Nucl.Substring(i+1, 1)))>0)
                    {
                        NormNucl += Nucl.Substring(i+1, 1).ToLower();
                    }
                    NormNucl += "-";
                    break;
                }
            }
            for (int i = 0; i < Nucl.Length; i++)
            {
                if (digit.IndexOf(Nucl.Substring(i, 1)) > 0)
                {
                    NormNucl += Nucl.Substring(i, 1);
                }
                else
                {
                    if (Nucl.Substring(i, 1).ToLower() == "m")
                    {
                        if (i > 0 && digit.IndexOf(Nucl.Substring(i-1, 1)) > 0)
                        {
                            NormNucl += "m";
                        }
                    }
                }
            }
            return NormNucl;
        }
    }

    /// <summary>
    /// Collection of isotops
    /// </summary>
    public class Spectrum : System.Collections.CollectionBase
    {
        public Nuclide this[ int index ]  {
            get  {
                return( (Nuclide) List[index] );
            }
            set  {
                List[index] = value;
            }
        }
        /// <summary>
        /// Add a nuclide in the collection
        /// </summary>
        /// <param name="nuclide" type="Nuclide"></param>
        /// <returns></returns>
        public int Add( Nuclide nuclide )  {
            return( List.Add( nuclide ) );
        }

        /// <summary>
        /// get the position in the list of one nuclide
        /// </summary>
        /// <param name="nuclide" type="Nuclide"></param>
        /// <returns>position in the list of one nuclide</returns>
        public int IndexOf( Nuclide nuclide )  {
            return( List.IndexOf( nuclide ) );
        }
        /// <summary>
        /// get the position in the list of one nuclide
        /// </summary>
        /// <param name="nuclideName" type="string"></param>
        /// <returns>position in the list of one nuclide</returns>
        public int IndexOf(string nuclideName)
        {
            foreach (Nuclide nuc in List)
            {
                if (nuc.Isotop == nuclideName)
                {
                    return (List.IndexOf(nuc));
                }
            }
            return (-1);
        }

        /// <summary>
        /// Insert a Nuclide at a given position in the List
        /// </summary>
        /// <param name="index" type="int"></param>
        /// <param name="nuclide" type="Nuclide"></param>
        public void Insert( int index, Nuclide nuclide )  {
            List.Insert( index, nuclide );
        }

        /// <summary>
        /// Remove a Nuclide from the List
        /// </summary>
        /// <param name="nuclide" type="Nuclide"></param>
        public void Remove( Nuclide nuclide )  {
            List.Remove( nuclide );
        }

        /// <summary>
        /// Indicates if the List contains the
        /// </summary>
        /// <param name="nuclide" type="Nuclide"></param>
        /// <returns>boolean</returns>
        public bool Contains( Nuclide nuclide )  {
            // If value is not of type Int16, this will return false.
            return( List.Contains( nuclide ) );
        }

        protected override void OnInsert( int index, Object value )  {
            // Insert additional code to be run only when inserting values.
        }

        protected override void OnRemove( int index, Object value )  {
            // Insert additional code to be run only when removing values.
        }

        protected override void OnSet( int index, Object oldValue, Object newValue )  {
            // Insert additional code to be run only when setting values.
        }

        protected override void OnValidate( Object value )  {
            if ( value.GetType() != typeof(Nuclide) )
                throw new ArgumentException( "value must be of type Nuclide.", "value" );
        }
    }
}

