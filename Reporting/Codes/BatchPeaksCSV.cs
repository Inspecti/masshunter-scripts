#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
// BatchPeaksCSV.cs - Export Unknowns Analysis batch peak list into a single csv file 
// Based on QuantCSV.cs by Agilent Technologies / Yoshi Tsunoi
// https://github.com/Inspecti/masshunter-scripts
//
///////////////////////////////////////////////////////////////////////////////
#endregion Copyright

using System;
using System.Text;
using System.Globalization;
using Agilent.MassSpectrometry.DataAnalysis.UnknownsAnalysisII;
using Agilent.MassSpectrometry.DataAnalysis.UnknownsAnalysisII.ReportResults;

namespace Agilent.MassSpectrometry.DataAnalysis.UnknownsAnalysisII.Report
{
  public class QuantCSV : Quantitative.ReportScript.IReportScript
  {
    private IUnknownsAnalysisDataProvider m_dataProvider;

    public QuantCSV()
    {
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
    }

    public string PreferredPageSize
    {
      get; set;
    }

    public Quantitative.ReportScript.IReportDataProvider DataProvider
    {
      get { return m_dataProvider; }
      set { m_dataProvider = value as IUnknownsAnalysisDataProvider; }
    }

    public void Process(string file)
    {
      m_dataProvider.CheckAbortSignal();

      string delimChar = ","; // Character to be used as CSV delimiter
      string[] columnHeaders = new string[] {"Datafile", "Peak Number", "Compound", "Area", "Height", "RT", "CAS", "Match Score"}; // Column headers to the first row

      using(System.IO.StreamWriter writer = new System.IO.StreamWriter(file, false, Encoding.Default))
      using(UnknownsAnalysisDataSet.TargetCompoundDataTable tdt = new UnknownsAnalysisDataSet.TargetCompoundDataTable())
      using(UnknownsAnalysisDataSet.ComponentDataTable cdt = new UnknownsAnalysisDataSet.ComponentDataTable())
      {

        // Get samples
        Agilent.MassSpectrometry.DataAnalysis.UnknownsAnalysisII.SampleRowID[] sids = m_dataProvider.TargetSamples;
	      int sampleCount = 0;

        // Write header row info
        writer.Write(String.Join(delimChar, columnHeaders));
        writer.WriteLine();

        // Iterate through each sample
	      foreach ( Agilent.MassSpectrometry.DataAnalysis.UnknownsAnalysisII.SampleRowID sid in sids )
        {
            UnknownsAnalysisDataSet.SampleDataTable sdt = new UnknownsAnalysisDataSet.SampleDataTable();
            m_dataProvider.GetTargetCompound( sid.BatchID, sid.SampleID, null, tdt );
            m_dataProvider.GetComponent( sid.BatchID, sid.SampleID, null, null, cdt );
            m_dataProvider.GetSample(sid.BatchID, sid.SampleID, sdt);	      	      

            int count = 0;
            string samplename = "";
            
            if ( sdt.Count > 0) 
            {
              samplename = sdt[sampleCount].DataFileName;
            }

            // Iterate through each component
            foreach ( UnknownsAnalysisDataSet.ComponentRow component in cdt )
            {
                if ( !component.IsPrimaryHitIDNull() )
                {
                    UnknownsAnalysisDataSet.HitDataTable hdt = new UnknownsAnalysisDataSet.HitDataTable();
                    m_dataProvider.GetHit( sid.BatchID, sid.SampleID, component.DeconvolutionMethodID, component.ComponentID, component.PrimaryHitID, hdt );
                    if ( hdt.Count > 0 )
                    {
                        UnknownsAnalysisDataSet.HitRow hit = hdt[0];

                        if ( hit.IsTargetCompoundIDNull() )
                        {
                            count++;
                            m_dataProvider.CheckAbortSignal();
                            writer.Write( string.Format( CultureInfo.InvariantCulture, "\"{0}\"", samplename ) );
                            writer.Write( delimChar );
                            writer.Write( string.Format( CultureInfo.InvariantCulture, "\"{0}\"", count ) );
                            writer.Write( delimChar );
                            writer.Write( hit.IsCompoundNameNull() ? null : string.Format( CultureInfo.InvariantCulture, "\"{0}\"", hit.CompoundName ) );
                            writer.Write( delimChar );
                            writer.Write( component.IsAreaNull() ? null : string.Format( CultureInfo.InvariantCulture, "{0:F0}", component.Area ) );
                            writer.Write( delimChar );
                            writer.Write( component.IsHeightNull() ? null : string.Format( CultureInfo.InvariantCulture, "{0:F0}", component.Height ) );
                            writer.Write( delimChar );
                            writer.Write( component.IsRetentionTimeNull() ? null : string.Format( CultureInfo.InvariantCulture, "{0:F4}", component.RetentionTime ) );
                            writer.Write( delimChar );
                            writer.Write( hit.IsCASNumberNull() ? null : string.Format( CultureInfo.InvariantCulture, "\"{0}\"", hit.CASNumber ) );
                            writer.Write( delimChar );
                            writer.Write( hit.IsLibraryMatchScoreNull() ? null : string.Format( CultureInfo.InvariantCulture, "\"{0:F1}\"", hit.LibraryMatchScore ) );
                            writer.Write( delimChar );
                            writer.WriteLine();
                        }
                    }
                }
          }
          sdt.Clear();
	        tdt.Clear();
          cdt.Clear();
        }
      writer.Flush();
      }
    }
  }
}