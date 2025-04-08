using System;
using System.IO;
using System.Runtime.InteropServices;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;


public class Rows : IDisposable
{
    public List<string> strRows = null;
    public Rows() 
    {
        strRows = new List<string>();
    }

    public void Dispose()
    {
        strRows.Clear();
        strRows = null;
    }
}

#if UNITY_EDITOR
public class EditorCSVUtils
{
    public static void LoadXLSXFile(string _xlsxFileName)
    {

    }

    static void updateDB(List<string> _colnames, List<Rows> _rows)
    {

    }
}
#endif
