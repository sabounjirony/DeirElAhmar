using System.Collections.Generic;
using System.Data;
using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Tools.Utilities;

namespace Tools.Export
{
    public class UnFormattedSpreadSheetWorker
    {
        public static bool CreateExcelDocument<T>(List<T> list, string xlsxFilePath)
        {
            var ds = new DataSet();
            ds.Tables.Add(LinqUtilities.ConvertTo(list));
            ds.Tables[0].TableName = "Requests";

            return CreateExcelDocument(ds, xlsxFilePath);
        }

        public static bool CreateExcelDocument(DataSet ds, string excelFilename)
        {
            using (var document = SpreadsheetDocument.Create(excelFilename, SpreadsheetDocumentType.Workbook))
            {
                WriteExcelFile(ds, document);
            }
            return true;
        }

        private static void WriteExcelFile(DataSet ds, SpreadsheetDocument spreadsheet)
        {

            //  Create the Excel file contents.  This function is used when creating an Excel file either writing 
            //  to a file, or writing to a MemoryStream.
            spreadsheet.AddWorkbookPart();
            spreadsheet.WorkbookPart.Workbook = new Workbook();

            //  My thanks to James Miera for the following line of code (which prevents crashes in Excel 2010)
            spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

            //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
            var workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
            var stylesheet = new Stylesheet();
            workbookStylesPart.Stylesheet = stylesheet;

            //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
            uint worksheetNumber = 1;
            foreach (DataTable dt in ds.Tables)
            {
                //  For each worksheet you want to create
                //var workSheetId = "rId" + worksheetNumber.ToString(CultureInfo.InvariantCulture);
                //var worksheetName = dt.TableName;
                var newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new Worksheet();

                // create sheet data
                newWorksheetPart.Worksheet.AppendChild(new SheetData());



                // save worksheet
                WriteDataTableToExcelWorksheet(dt, newWorksheetPart);
                newWorksheetPart.Worksheet.Save();

                // create the worksheet to workbook relation
                if (worksheetNumber == 1)
                    spreadsheet.WorkbookPart.Workbook.AppendChild(new Sheets());

                spreadsheet.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet
                                                                                          {
                                                                                              Id =
                                                                                                  spreadsheet.
                                                                                                  WorkbookPart.
                                                                                                  GetIdOfPart(
                                                                                                      newWorksheetPart),
                                                                                              SheetId =
                                                                                                  worksheetNumber,
                                                                                              Name = dt.TableName
                                                                                          });

                worksheetNumber++;
            }

            spreadsheet.WorkbookPart.Workbook.Save();
        }

        private static void WriteDataTableToExcelWorksheet(DataTable dt, WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();

            //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
            //
            //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
            //  cells of data, we'll know if to write Text values or Numeric cell values.
            var numberOfColumns = dt.Columns.Count;
            var isNumericColumn = new bool[numberOfColumns];

            var excelColumnNames = new string[numberOfColumns];
            for (var n = 0; n < numberOfColumns; n++)
                excelColumnNames[n] = GetExcelColumnName(n);

            //
            //  Create the Header row in our Excel Worksheet
            //
            uint rowIndex = 1;

            var headerRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
            sheetData.AppendChild(headerRow);

            for (var colInx = 0; colInx < numberOfColumns; colInx++)
            {
                var col = dt.Columns[colInx];
                AppendTextCell(excelColumnNames[colInx] + "1", col.ColumnName, headerRow);
                isNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32");
            }

            //
            //  Now, step through each row of data in our DataTable...
            //
            foreach (DataRow dr in dt.Rows)
            {
                // ...create a new row, and append a set of this row's data to it.
                ++rowIndex;
                var newExcelRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
                sheetData.AppendChild(newExcelRow);

                for (var colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    var cellValue = dr.ItemArray[colInx].ToString();

                    // Create cell with data
                    if (isNumericColumn[colInx])
                    {
                        //  For numeric cells, make sure our input data IS a number, then write it out to the Excel file.
                        //  If this numeric value is NULL, then don't write anything to the Excel file.
                        double cellNumericValue;
                        if (double.TryParse(cellValue, out cellNumericValue))
                        {
                            cellValue = cellNumericValue.ToString(CultureInfo.InvariantCulture);
                            AppendNumericCell(excelColumnNames[colInx] + rowIndex.ToString(CultureInfo.InvariantCulture), cellValue, newExcelRow);
                        }
                    }
                    else
                    {
                        //  For text cells, just write the input data straight out to the Excel file.
                        AppendTextCell(excelColumnNames[colInx] + rowIndex.ToString(CultureInfo.InvariantCulture), cellValue, newExcelRow);
                    }
                }
            }
        }

        private static void AppendTextCell(string cellReference, string cellStringValue, Row excelRow)
        {
            //  Add a new Excel Cell to our Row 
            var cell = new Cell { CellReference = cellReference, DataType = CellValues.String };
            var cellValue = new CellValue { Text = cellStringValue };
            cell.AppendChild(cellValue);
            excelRow.AppendChild(cell);
        }

        private static void AppendNumericCell(string cellReference, string cellStringValue, Row excelRow)
        {
            //  Add a new Excel Cell to our Row 
            var cell = new Cell { CellReference = cellReference };
            var cellValue = new CellValue { Text = cellStringValue };
            cell.AppendChild(cellValue);
            excelRow.AppendChild(cell);
        }

        private static string GetExcelColumnName(int columnIndex)
        {
            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            //
            //  eg  GetExcelColumnName(0) should return "A"
            //      GetExcelColumnName(1) should return "B"
            //      GetExcelColumnName(25) should return "Z"
            //      GetExcelColumnName(26) should return "AA"
            //      GetExcelColumnName(27) should return "AB"
            //      ..etc..
            //
            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString(CultureInfo.InvariantCulture);

            var firstChar = (char)('A' + (columnIndex / 26) - 1);
            var secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }
    }
}