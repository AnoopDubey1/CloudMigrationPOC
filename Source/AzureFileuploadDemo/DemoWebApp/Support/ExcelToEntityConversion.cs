using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.IO;
namespace DemoWebApp.Support
{
    public static class ExcelImporter
    {
        public static IList<T> ReadToList<T>(string path,int fromRow,int fromColumn, int toColumn = 0) where T: class
        {
            using (var excelpack = new OfficeOpenXml.ExcelPackage())
            {
                List<T> getList = new List<T>();

                using (var stream = File.OpenRead(path))
                {
                    excelpack.Load(stream);

                }
                var ws = excelpack.Workbook.Worksheets.First();
                toColumn = toColumn == 0 ? typeof(T).GetProperties().Count() : toColumn;

                for(var rownum= fromRow; rownum<=ws.Dimension.End.Row;rownum++)
                {
                    T objT = Activator.CreateInstance<T>();
                    Type myType = typeof(T);
                    PropertyInfo[] myprop =  myType.GetProperties();

                    var wsRow = ws.Cells[rownum, fromColumn, rownum, toColumn];

                    for(int i=0;i<myprop.Count();i++)
                    {
                        myprop[i].SetValue(objT, Convert.ChangeType(wsRow[rownum, fromColumn + i].Value, myprop[i].PropertyType));
                    }

                    getList.Add(objT);
                }

                return getList;
            }

        }
    }

    public class SaleOrder
    {
        public string SaleToBuyer { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNo { get; set; }
        public DateTime DueDate { get; set; }
        public string Product { get; set; }
        public string BuyerUpcCode { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
    }
}