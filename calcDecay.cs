using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Configuration;
using MDecay.Net;

/// <summary>
/// Decay compute object
/// </summary>
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
///
/// Copyright ONDRAF/NIRAS 2008,2012,2021
/// 
namespace NirondTools.MDecay
{
    /// <summary>
    /// Decay period enum
    /// </summary>
    public enum enumDecayUnit
    {
        Years = 'Y',
        Days = 'D',
        Hours = 'H',
        Minutes = 'M',
        Seconds = 'S'
    };

    #region Compute object definitions
    /// <summary>
    /// ZonesCalcul : object to hold temporary decay computation
    /// </summary>
    class ZonesCalcul 
    {
        string mSymbol;
        double mActivity;
        double mLambda;
        double mBranchRatio;

        public ZonesCalcul()
        {
        }
        public ZonesCalcul(string Symbol, double Activity, double Lambda, double BranchRatio)
        {
            mSymbol = Symbol; mActivity = Activity; mLambda = Lambda; mBranchRatio = BranchRatio;
        }
        public string Symbol
        {
            get { return mSymbol; }
            set { mSymbol = value; }
        }
        public double Activity
        {
            get { return mActivity; }
            set { mActivity = value; }
        }
        public double Lambda
        {
            get { return mLambda; }
            set { mLambda = value; }
        }
        public double BranchRatio
        {
            get { return mBranchRatio; }
            set { mBranchRatio = value; }
        }

    }
    /// <summary>
    /// colCalculs : collection of Zonescalcul object
    /// </summary>
    class colCalculs : System.Collections.CollectionBase
    {
        public ZonesCalcul this[int index]
        {
            get
            {
                return ((ZonesCalcul)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }
        public int Add(ZonesCalcul value)
        {
            return (List.Add(value));
        }

        public int IndexOf(ZonesCalcul value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, ZonesCalcul value)
        {
            List.Insert(index, value);
        }

        public void Remove(ZonesCalcul value)
        {
            List.Remove(value);
        }

        public bool Contains(ZonesCalcul value)
        {
            // If value is not of type Int16, this will return false.
            return (List.Contains(value));
        }

        protected override void OnInsert(int index, Object value)
        {
            // Insert additional code to be run only when inserting values.
        }

        protected override void OnRemove(int index, Object value)
        {
            // Insert additional code to be run only when removing values.
        }

        protected override void OnSet(int index, Object oldValue, Object newValue)
        {
            // Insert additional code to be run only when setting values.
        }

        protected override void OnValidate(Object value)
        {
            if (value.GetType() != typeof(ZonesCalcul))
                throw new ArgumentException("value must be of type Nuclide.", "value");
        }
    }
    #endregion

    /// <summary>
    /// The main CalcDecay class
    /// </summary>
    public class CalcDecay
    {
        #region Private declaration
        private System.Data.DataSet dsNucleide;
        private System.Data.DataTable dtNucleide;
        private System.Data.DataTable dtNucDecay;

        private DateTime mStartDate;
        private DateTime mEndDate;
        private Spectrum mSpectrumIn;
        private Spectrum mSpectrumOut;
        private colCalculs mcolCalculs;
        private Int64 mDecayTime;
        private int miLevel;
        private long mDecaySeconds;
        private Nuclide mParentNuc;
        private enumDecayUnit mDecayUnit;
        private string msAppData = "";
        private string msDirSep;
        #endregion

        #region Public properties
        /// <summary>
        /// countNucleide gives the nucleide records count 
        /// </summary>
        public long countNucleide
        {
            get { return dtNucleide.Rows.Count; }
        }
        /// <summary>
        /// countNucDecay gives the Nucleide decay records count
        /// </summary>
        public long countNucDecay
        {
            get { return dtNucDecay.Rows.Count; }
        }
        /// <summary>
        /// startDate : get or set the start date
        /// </summary>
        /// <value>Get/Set a DateTime value</value>
        /// 
        public DateTime startDate
        {
            get { return mStartDate; }
            set { mStartDate = value; }
        }

        /// <summary>
        /// endDate : get or set the end date
        /// </summary>
        /// <value>Get/Set a DateTime value</value>
        public DateTime endDate
        {
            get { return mEndDate; }
            set
            {
                mEndDate = value;
                if (mStartDate != null && mEndDate != null)
                {
                    TimeSpan ts = mEndDate - mStartDate;
                    mDecayTime = (long)ts.TotalDays;
                    mDecayUnit = enumDecayUnit.Days;
                }
            }
        }

        /// <summary>
        /// DecayUnit : get or set the decay unit
        /// Year, Month, Day, Hour, Minute or Second
        /// </summary>
        public enumDecayUnit DecayUnit
        {
            get { return mDecayUnit; }
            set { mDecayUnit = value; }
        }
        /// <summary>
        /// DecayTime = get or set the decay unit time
        /// </summary>
        /// <value>Get/Set an integer value</value>
        public Int64 DecayTime
        {
            get { return mDecayTime; }
            set { mDecayTime = value; }
        }

        /// <summary>
        /// Get decay detail for a given Nucleide
        /// </summary>
        /// <param name="Nuc">a Nuclide object</param>
        /// <returns>returns a Nuclide object with details</returns>
        public Nuclide GetNucDetail(Nuclide Nuc)
        {
            if (dtNucleide != null)
            {
                DataRow[] drs = dtNucleide.Select("Symbol='" + Nuc.Isotop + "'");
                if (drs.Length > 0)
                {
                    Nuc.lambda = Convert.ToDouble(drs[0]["Lambda"]);
                    switch (drs[0]["Unit"].ToString())
                    {
                        case "seconds":
                            Nuc.LiveUnit = enumDecayUnit.Seconds;
                            break;
                        case "minutes":
                            Nuc.LiveUnit = enumDecayUnit.Minutes;
                            break;
                        case "hours":
                            Nuc.LiveUnit = enumDecayUnit.Hours;
                            break;
                        case "days":
                            Nuc.LiveUnit = enumDecayUnit.Days;
                            break;
                        case "years":
                            Nuc.LiveUnit = enumDecayUnit.Years;
                            break;
                    }
                    Nuc.HalfLive = Convert.ToInt64(drs[0]["Half"]);
                }
            }
            return Nuc;
        }
        #endregion

        #region Init Public functions
        /// <summary>
        /// CalcDecay constructor
        /// </summary>
        public CalcDecay()
        {
            //xmlNucleide = new XmlDocument();

            ///
            /// ONDRAF-NIRAS environment
            /// msAppData holds the path to the XML data files 
            /// 
            Utils oUtils = new Utils();
            msDirSep = @"\";
            if (oUtils.RunOnLinux())
            {
                msDirSep = "/";
            }
            if (System.Environment.UserDomainName.ToLower().StartsWith("nirond"))
            {
                /// use the shared network path
                msAppData = @"\\nironddata\data$\Decay.Net";
            }
            else
            {
                // use the local path under %ProgramData%\nirond\MDecay.Net
                msAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                msAppData += msDirSep + "nirond" + msDirSep + "MDecay.Net";
            }
            LoadData(msAppData);
        }

        public void LoadData(string sPath="")
        {
            if (sPath == "" && msAppData == "")
            {
                sPath = System.Environment.CurrentDirectory;
                msAppData = sPath;
            }
            if (!File.Exists(msAppData + msDirSep + "Nuclides.xml"))
            {
                msAppData = "." + msDirSep;
            }
            /// Read XML files into DataSet and DataTable objects
            dsNucleide = new DataSet();
            dtNucleide = new DataTable();
            dtNucDecay = new DataTable();
            try
            {
                string sNuclides = msAppData + msDirSep + "Nuclides.xml";
                string sNucDecay = msAppData + msDirSep + "NucDecay.xml";
#if DEBUG
                Console.WriteLine(sNuclides + "\r\n" + sNucDecay);
#endif
                DataSet dsNuclides = new DataSet();
                dsNuclides.ReadXml(sNuclides);
                dtNucleide = dsNuclides.Tables["Nuclide"];
                dtNucDecay.ReadXml(sNucDecay);
            }
            catch (Exception ex)
            {
                string sMess = "XML data files not available.\r\n";
                IOException exXml = new IOException(sMess + ex.Message);
                exXml.Source = "MDecay.CalcDecay";
                throw exXml;
            }
        }
        #endregion

        #region Public Compute functions

        /// <summary>
        /// Compute the decay for the default Spectrum collection `mSpectrumIn`
        /// </summary>
        /// <returns>Returns a Spectrum collection</returns>
        public Spectrum DecayCalc()
        {
            if (mSpectrumIn != null && mSpectrumIn.Count > 0)
            {
                mSpectrumOut = new Spectrum();
                Calculate();
                return mSpectrumOut;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Compute the decay for the given SpectrumIn collection
        /// </summary>
        /// <param name="SpectrumIn">a Spectrum collection</param>
        /// <returns>Returns a Spectrum collection</returns>
        public Spectrum DecayCalc(Spectrum SpectrumIn)
        {
            mSpectrumIn = SpectrumIn;
            return DecayCalc();
        }
        #endregion

        #region Private Compute functions
        /// <summary>
        /// Calculate() is the main compute procedure
        /// </summary>
        private void Calculate()
        {
            switch (mDecayUnit)
            {
                case enumDecayUnit.Seconds:
                    mDecaySeconds = mDecayTime * 1;
                    break;
                case enumDecayUnit.Minutes:
                    mDecaySeconds = mDecayTime * 60;
                    break;
                case enumDecayUnit.Hours:
                    mDecaySeconds = mDecayTime * 60 * 60;
                    break;
                case enumDecayUnit.Days:
                    mDecaySeconds = mDecayTime * 60 * 60 * 24;
                    break;
                case enumDecayUnit.Years:
                    mDecaySeconds = (long)(mDecayTime * 60 * 60 * 24 * 365.25);
                    break;
            }

            foreach (Nuclide thisNuc in mSpectrumIn)
            {
                mcolCalculs = new colCalculs();
                ZonesCalcul Constants = new ZonesCalcul();
                Constants.Symbol = thisNuc.Isotop;
                Constants.Activity = thisNuc.Activity;
                Constants.BranchRatio = 1;
                mcolCalculs.Add(Constants);
                miLevel = 0;
                mParentNuc = thisNuc;
                BrowseChain(thisNuc);
            }
        }
        /// <summary>
        /// BrowseChain runs into the decay chain of one Nuclide, and compute the resulting activity.
        /// </summary>
        /// <param name="thisNuc">the Nuclide</param>
        /// <remarks>this function is recursive</remarks>
        private void BrowseChain(Nuclide thisNuc)
        {

            ZonesCalcul Constants = mcolCalculs[miLevel];
            string sTab="";
            for (int i = 0; i < miLevel; i++) { sTab += "\t"; }
            foreach (DataRow drNuc in dtNucleide.Select("Symbol='" + thisNuc.Isotop + "'")) 
            {
                string Symbol = drNuc["Symbol"].ToString();
                string Elem = drNuc["Elem"].ToString();
                int z = (int)drNuc["Z"];
                int a = (int)drNuc["A"];
                double lambda = 0.0;
                if (miLevel > 0)
                {
                    lambda = mcolCalculs[0].Lambda;
                }
                else
                {
                    lambda = Convert.ToDouble(drNuc["Lambda"]);
                }
                double Act = mParentNuc.Activity;

                Constants.Lambda = Convert.ToDouble(drNuc["Lambda"]);
#if DEBUG
                Console.WriteLine(sTab + miLevel);
                Console.WriteLine(sTab + "Symbol:" + Constants.Symbol);
                Console.WriteLine(sTab + "Lambda:" + Constants.Lambda);
                Console.WriteLine(sTab + "Branch:" + Constants.BranchRatio);
                Console.WriteLine(sTab + "Activ :" + Constants.Activity);
#endif                
                double dblActLbd = Act / lambda; // $Result dans la routine PHP sur l'intranet
#if DEBUG
                Console.WriteLine("dblActLbd = " + Act.ToString() + " / " + lambda.ToString() + " = " + dblActLbd.ToString());
#endif
                double varSum = 0.0;
                foreach (ZonesCalcul zc in mcolCalculs)
                {
                    double varProd = 1.0;
#if DEBUG
                    Console.Write(sTab + "dblActLbd *= BranchRation : " + dblActLbd.ToString() + " * " + zc.BranchRatio + " = ");
#endif
                    if (zc.BranchRatio > 0) { dblActLbd *= zc.BranchRatio; }
#if DEBUG
                    Console.WriteLine(dblActLbd.ToString());
#endif
                    foreach (ZonesCalcul zcx in mcolCalculs) 
                    {
#if DEBUG
                        Console.WriteLine(sTab + "zc {0} zcx {1} ", zc.Symbol, zcx.Symbol);
#endif
                        if (zc.Symbol != zcx.Symbol)
                        {
#if DEBUG
                            Console.WriteLine(sTab + sTab + "zc.lambda {0} / zcx.lambda {1}", zc.Lambda, zcx.Lambda);
#endif
                            varProd *= (1 - (zc.Lambda / zcx.Lambda));
#if DEBUG
                            Console.WriteLine(sTab + sTab + "varProd=" + varProd.ToString());
#endif
                        }
                    }
                    double tt = -zc.Lambda * mDecaySeconds;
#if DEBUG
                    Console.WriteLine(sTab + "tt = -" + zc.Lambda + " * " + mDecaySeconds + " = " + tt.ToString()); 
#endif
                    varSum += zc.Lambda * (double)Math.Exp((double)tt) / varProd;
#if DEBUG
                    Console.WriteLine(sTab + "varSum += " + zc.Lambda + " * " + Math.Exp(tt) + " / " + varProd.ToString() + " = " + varSum.ToString());
#endif
                }
                dblActLbd *= varSum;
#if DEBUG
                Console.WriteLine (sTab + drNuc["Symbol"].ToString() + " " + lambda.ToString() + " " + Act.ToString() + " " + varSum.ToString() + " dblAct=" + dblActLbd.ToString());
#endif
                // store nuclide and resulting activity to mSpectrumOut
                Nuclide nucOut = new Nuclide();
                nucOut.Isotop = thisNuc.Isotop;
                nucOut.Activity = dblActLbd;
                    
                int indexOfNuc = mSpectrumOut.IndexOf(nucOut.Isotop);
                if (indexOfNuc == -1)
                {
                    mSpectrumOut.Add(GetNucDetail(nucOut));
                }
                else
                {
                    mSpectrumOut[indexOfNuc].Activity += nucOut.Activity;
                }
                
                // Search for children nuclide
                foreach (DataRow drNucChild in dtNucDecay.Select("Symbol='" + thisNuc.Isotop + "'"))
                {
                    miLevel++;
                    Nuclide childNuc = new Nuclide();
                    childNuc.Isotop = drNucChild["Daughter"].ToString();
                    childNuc.Activity = -1;
                    ZonesCalcul SubConstants = new ZonesCalcul();
                    SubConstants.Symbol = drNucChild["Daughter"].ToString();
                    SubConstants.Activity = 1;
                    SubConstants.BranchRatio = Convert.ToDouble(drNucChild["BranchRatio"]);
                    mcolCalculs.Add(SubConstants);
                    // Call BrowseChain recursively
                    BrowseChain(childNuc);
                    mcolCalculs.Remove(SubConstants);
                    miLevel--;
                }

            }
        }
        #endregion
    }
}
