using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Data;
using DocumentFormat.OpenXml;
using Tools.Utilities;

namespace Tools.Export
{
    public class SpreadsheetWorker : IDisposable
    {
        private string _sourceFile;
        private string _outputFile;
        private MemoryStream _inMemoryStream;

        protected SpreadsheetDocument InMemoryDocument { get; private set; }

        /// <summary>
        /// Returns True if the document has been opened.
        /// </summary>
        public bool IsOpen
        {
            get { return (InMemoryDocument != null); }
        }

        #region IDisposable Members

        /// <summary>
        /// This class wraps objects that implement IDisposable, so we better comply
        /// to ensure their resources get released when ours do.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion

        /// <summary>
        /// Opens the specified Document file.
        /// This loads it in memory for later modification.
        /// </summary>
        /// <param name="sourceBytes"> </param>
        public void Open(byte[] sourceBytes)
        {
            //if the document is already Open well close it first to free up resources.
            if (IsOpen)
            {
                Close();
            }

            //populate a MemoryStream with byte array of the Document
            _inMemoryStream = new MemoryStream();
            _inMemoryStream.Write(sourceBytes, 0, (int)sourceBytes.Length);

            //Create the in-memory Document from the stream for modification
            InMemoryDocument = SpreadsheetDocument.Open(_inMemoryStream, true);
        }

        /// <summary>
        /// Opens the specified Document file.
        /// This loads it in memory for later modification.
        /// </summary>
        /// <param name="fileName"></param>
        public void Open(string fileName)
        {
            //populate a MemoryStream with byte array of the Document
            byte[] sourceBytes = File.ReadAllBytes(fileName);
            Open(sourceBytes);

            _sourceFile = fileName;
        }

        /// <summary>
        /// Validate the in-memory Document. Throws an exception if it is invalid.
        /// </summary>
        //public void Validate()
        //{
        //    if (!IsOpen)
        //    {
        //        throw new Exception("The object must be Open before calling Validate()");
        //    }

        //    if (InMemoryDocument != null)
        //    {
        //        InMemoryDocument.Validate(null);
        //    }
        //}

        public byte[] SaveAs()
        {
            byte[] outputBytes;

            if (!IsOpen)
            {
                throw new Exception("The object must be Open before calling SaveAs()");
            }

            _outputFile = string.Empty;

            //Close the in-memory document to ensure the memory stream is ready for saving
            CloseInMemoryDocument();
            try
            {
                outputBytes = _inMemoryStream.ToArray();
            }
            finally
            {
                //Now close the memory stream. 
                //The in-memory document has already been closed above 
                //and there's no point having one without the other!
                CloseInMemoryStream();
            }

            return outputBytes;

        }

        /// <summary>
        /// Takes the in-memory Document and saves it to the specified file. Overwriting if necessary.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveAs(string fileName)
        {
            if (!IsOpen)
            {
                throw new Exception("The object must be Open before calling SaveAs()");
            }

            _outputFile = fileName;
            if (!string.IsNullOrEmpty(_outputFile))
            {
                //Close the in-memory document to ensure the memory stream is ready for saving
                CloseInMemoryDocument();
                try
                {

                    //Now save the stream to file
                    using (var fileStream = new FileStream(_outputFile, FileMode.Create))
                    {
                        _inMemoryStream.WriteTo(fileStream);
                    }

                }
                finally
                {
                    //Now close the memory stream. 
                    //The in-memory document has already been closed above 
                    //and there's no point having one without the other!
                    CloseInMemoryStream();
                }
            }
        }

        /// <summary>
        /// Closes and disposes the in-memory document
        /// </summary>
        private void CloseInMemoryDocument()
        {
            if (InMemoryDocument != null)
            {
                InMemoryDocument.Close();
                InMemoryDocument.Dispose();
                InMemoryDocument = null;
            }
        }

        /// <summary>
        /// Closes and disposes the memory stream
        /// </summary>
        private void CloseInMemoryStream()
        {
            if (_inMemoryStream != null)
            {
                _inMemoryStream.Close();
                _inMemoryStream.Dispose();
                _inMemoryStream = null;
            }
        }

        public void Close()
        {
            CloseInMemoryDocument();
            CloseInMemoryStream();
        }

        public void FillData<T>(string namedRangeToFill, List<T> list)
        {
            var ds = new DataSet();
            ds.Tables.Add(LinqUtilities.ConvertTo(list));
            ds.Tables[0].TableName = "Requests";
            FillData(namedRangeToFill, ds.Tables[0]);
        }

        /// <summary>
        /// populate the namedRange part of the spreadsheet with the rows from data.
        /// </summary>
        /// <param name="namedRangeToFill"></param>
        /// <param name="dataTable"></param>
        public void FillData(string namedRangeToFill, DataTable dataTable)
        {

            //get all the reference names
            var namesTable = BuildDefinedNamesTable(InMemoryDocument.WorkbookPart);

            var matchedNameRange = namesTable.SingleOrDefault(d => d.Key == namedRangeToFill);
            if (matchedNameRange == null)
            {throw new Exception("The Named Range \"{namedRangeToFill}\" was not found in the document.");}

            var worksheetPart = GetWorkSheetPart(matchedNameRange);
            var sheetData = GetWorkSheetData(worksheetPart, matchedNameRange);
            var templateContentRow = GetContentRow(sheetData, Convert.ToInt32(matchedNameRange.EndRow));

            var rowNumber = Convert.ToInt32(matchedNameRange.EndRow);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var contentRow = CreateContentRow(rowNumber, dataRow, templateContentRow, matchedNameRange);
                templateContentRow.InsertBeforeSelf(contentRow);
                rowNumber++;
            }

            //Now remove the template row because we've finished cloning it.
            sheetData.RemoveChild(templateContentRow);
            worksheetPart.Worksheet.Save();
        }

        public void FillSingleData(string namedRangeToFill, string value)
        {
            //get all the reference names
            var namesTable = BuildDefinedNamesTable(InMemoryDocument.WorkbookPart);

            var matchedNameRange = namesTable.SingleOrDefault(d => d.Key == namedRangeToFill);
            if (matchedNameRange == null)
            { throw new Exception("The Named Range \"{namedRangeToFill}\" was not found in the document."); }

            var worksheetPart = GetWorkSheetPart(matchedNameRange);
            var theCell = worksheetPart.Worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == matchedNameRange.StartColumn + matchedNameRange.StartRow);
            if (theCell != null)
            {
                theCell.CellValue = new CellValue(value);
                theCell.DataType = new EnumValue<CellValues>(CellValues.String);
            }
            worksheetPart.Worksheet.Save();
        }

        private static Cell CreateCellFromTemplate(string header, int index, Cell templateCell, object value)
        {
            //Create new inline string cell
            var c = new Cell {StyleIndex = templateCell.StyleIndex, CellReference = header + index};

            if (value is DateTime)
            {
                var valueAsDateTime = (DateTime)value;
                var cellValue = new CellValue(valueAsDateTime.ToOADate().ToString(CultureInfo.InvariantCulture));
                c.AppendChild(cellValue);
            }
            else if (value is string || value is bool)
            {
                c.DataType = CellValues.SharedString;

                c.DataType = CellValues.InlineString;
                c.InlineString = new InlineString { Text = new Text(value.ToString()) };

                //var valueId = InsertSharedStringItem((string)value);
                //var cellValue = new CellValue(valueId.ToString());
                //c.AppendChild(cellValue);
            }

            else
            {
                var cellValue = new CellValue(value.ToString());
                c.AppendChild(cellValue);
            }

            return c;
        }

        private static Row GetContentRow(SheetData sheetData, int index)
        {
            var result = sheetData.Descendants<Row>().FirstOrDefault(c => c.RowIndex == index);
            return result;
        }

        private Row CreateContentRow(int index, DataRow dataRow, Row templateRow, DefinedNameVal namedRange)
        {

            //Create new row
            var r = new Row
                        {
                            RowIndex = (UInt32) index,
                            Spans = templateRow.Spans,
                            StyleIndex = templateRow.StyleIndex
                        };

            //First cell is a text cell, so create it and append it

            var columnIndexStart = GetColumnNumberFromColumnId(namedRange.StartColumn);
            var columnIndexEnd = GetColumnNumberFromColumnId(namedRange.EndColumn);
            //NOTE: For the purpose of this sample, it is assumed that the named range is no more than a single row.

            for (var columnIndex = columnIndexStart; columnIndex <= columnIndexEnd; columnIndex++)
            {
                Cell cell;

                var columnId = GetColumnAlpha(columnIndex);
                var templateCell = templateRow.Descendants<Cell>().ElementAtOrDefault(columnIndex - 1);
                var templateCellValue = GetValue(templateCell);

                if (templateCellValue.StartsWith("[") && templateCellValue.EndsWith("]"))
                {
                    var templateCellFieldName = templateCellValue.Substring(1, templateCellValue.Length - 2);

                    var dataValue = dataRow[templateCellFieldName];
                    if (dataValue != null)
                    {
                        cell = CreateCellFromTemplate(columnId, index, templateCell, dataValue);
                    }
                    else
                    {
                        //the named field was not found in the dataRow, so just use the templateCellValue for the new cell.
                        cell = CreateCellFromTemplate(columnId, index, templateCell, templateCellValue);
                    }
                }
                else
                {
                    //cell doesn not contain a field name, so just use the templateCellValue for the new cell.
                    cell = CreateCellFromTemplate(columnId, index, templateCell, templateCellValue);
                }

                r.AppendChild(cell);
            }

            return r;

        }

        private WorksheetPart GetWorkSheetPart(DefinedNameVal definedName)
        {
            //get worksheet based on defined name 
            string relId = InMemoryDocument.WorkbookPart.Workbook.Descendants<Sheet>().First(s => definedName.SheetName.Equals(s.Name)).Id;

            return (WorksheetPart)InMemoryDocument.WorkbookPart.GetPartById(relId);
        }

        private static SheetData GetWorkSheetData(WorksheetPart worksheetPart, DefinedNameVal definedName)
        {
            //get worksheet based on defined name 
            var result = worksheetPart.Worksheet.Descendants<SheetData>().FirstOrDefault();

            return result;
        }

        private string GetValue(Cell cell)
        {
            if (cell.ChildElements.Count == 0)
                return null;

            //get cell value
            string value = cell.CellValue.InnerText;

            //Look up real value from shared string table 
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                value = InMemoryDocument.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;

            return value;
        }

        /// <summary>
        /// [Code sampled from http://blogs.msdn.com/brian_jones/archive/2008/11/10/reading-data-from-spreadsheetml.aspx]
        /// </summary>
        /// <param name="workbookPart"></param>
        /// <returns></returns>
        private static IEnumerable<DefinedNameVal> BuildDefinedNamesTable(WorkbookPart workbookPart)
        {
            //Build a list
            var definedNames = new List<DefinedNameVal>();

            foreach (DefinedName name in workbookPart.Workbook.GetFirstChild<DefinedNames>())
            {
                //Parse defined name string...
                string key = name.Name;
                var reference = name.InnerText;

                var sheetName = reference.Split('!')[0];
                sheetName = sheetName.Trim('\'');

                //Assumption: None of my defined names are relative defined names (i.e. A1)
                var range = reference.Split('!')[1];
                var rangeArray = range.Split('$');

                var startCol = rangeArray[1];
                var startRow = rangeArray[2].TrimEnd(':');

                string endCol = null;
                string endRow = null;

                if (rangeArray.Length > 3)
                {
                    endCol = rangeArray[3];
                    endRow = rangeArray[4];
                }

                definedNames.Add(new DefinedNameVal
                                     {
                                         Key = key,
                                         SheetName = sheetName,
                                         StartColumn = startCol,
                                         StartRow = startRow,
                                         EndColumn = endCol,
                                         EndRow = endRow
                                     });
            }

            return definedNames;
        }

        /// <summary>
        /// [Sampled from http://www.codeplex.com/PowerTools]
        /// Gets the column Id for a given column number
        /// </summary>
        /// <param name="columnNumber">Column number</param>
        /// <returns>Column Id</returns>
        public static string GetColumnAlpha(int columnNumber)
        {
            const int alpha = (int)'Z' - (int)'A' + 1;
            if (columnNumber <= alpha)
                return ((char)((int)'A' + columnNumber - 1)).ToString(CultureInfo.InvariantCulture);
            else
                return
                    GetColumnAlpha(
                        (int)((columnNumber - 1) / alpha)
                    ) +
                    (
                        (char)(
                            (int)'A' + (int)((columnNumber - 1) % alpha)
                        )
                    ).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// [Sampled from http://www.codeplex.com/PowerTools]
        /// Returns the column number from the cell reference received as parameter
        /// </summary>
        /// <param name="columnId"> </param>
        /// <returns></returns>
        private static int GetColumnNumberFromColumnId(string columnId)
        {
            int columnNumber = 0;
            //Removing row number from cell reference
            int charPosition = 1;
            int charValue = 0;
            foreach (char c in columnId)
            {
                //Getting the Unicode value for the current char in cell reference
                charValue = System.Convert.ToInt32(c);
                if (charPosition < columnId.Length)
                {   //we have not reached the last character in cell reference
                    //we need to multiply the charValue (from 0 to 25) by 26 and add the result of powering 26 to current char position in cell reference
                    //65 is the Unicode value for "A" letter
                    //26 is the number of letters in English alphabet
                    columnNumber += (((charValue - 65) * 26) + (System.Convert.ToInt32(Math.Pow(26, charPosition++))));
                }
                else
                {   //This is the last character in cell reference
                    //we substract 64 instead of 65 because we want to get a one-based index for last character (instead of a zero-based index for previous characters)
                    columnNumber += (charValue - 64);
                }
            }
            return columnNumber;
        }

        /// <summary>
        /// [Sampled from http://www.codeplex.com/PowerTools]
        /// Returns the column number from the cell reference received as parameter
        /// </summary>
        /// <param name="cellReference">Cell reference to obtain the column number from</param>
        /// <param name="row">Row containing the cell to obtain the column number from</param>
        /// <returns></returns>
        private static int GetColumnNumberFromCellReference(string cellReference, int row)
        {
            var columnNumber = 0;
            //Removing row number from cell reference
            var columnReference = cellReference.Remove(cellReference.Length - row.ToString().Length);

            var charPosition = 1;
            foreach (var c in columnReference)
            {
                //Getting the Unicode value for the current char in cell reference
                var charValue = System.Convert.ToInt32(c);
                if (charPosition < columnReference.Length)
                {   //we have not reached the last character in cell reference
                    //we need to multiply the charValue (from 0 to 25) by 26 and add the result of powering 26 to current char position in cell reference
                    //65 is the Unicode value for "A" letter
                    //26 is the number of letters in English alphabet
                    columnNumber += (((charValue - 65) * 26) + (System.Convert.ToInt32(Math.Pow(26, charPosition++))));
                }
                else
                {   //This is the last character in cell reference
                    //we substract 64 instead of 65 because we want to get a one-based index for last character (instead of a zero-based index for previous characters)
                    columnNumber += (charValue - 64);
                }
            }
            return columnNumber;
        }

        /// <summary>
        /// [Code sampled from http://msdn.microsoft.com/en-us/library/cc861607%28office.14%29.aspx]
        /// Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        /// and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private int InsertSharedStringItem(string text)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (InMemoryDocument.WorkbookPart.SharedStringTablePart.SharedStringTable == null)
            {
                InMemoryDocument.WorkbookPart.SharedStringTablePart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in InMemoryDocument.WorkbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            InMemoryDocument.WorkbookPart.SharedStringTablePart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            InMemoryDocument.WorkbookPart.SharedStringTablePart.SharedStringTable.Save();

            return i;
        }

        /// <summary>
        /// [Sampled from http://blogs.msdn.com/brian_jones/archive/2008/11/10/reading-data-from-spreadsheetml.aspx]
        /// </summary>
        class DefinedNameVal
        {
            internal string Key;
            internal string SheetName;
            internal string StartColumn;
            internal string EndColumn;
            internal string StartRow;
            internal string EndRow;
        }

    }
}