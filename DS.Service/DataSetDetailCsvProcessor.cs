using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DS.Domain;
using DS.Domain.Interface;
using DS.Extensions;
using Elmah;
using LumenWorks.Framework.IO.Csv;


namespace DS.Service
{
    public class DataSetDetailCsvProcessor : IDataSetDetailCsvProcessor
    {
        private IDataSetSchemaService _dataSetSchemaService;
        private IRepository<DataSetDetail> _repository;

        public DataSetDetailCsvProcessor(IDataSetSchemaService dataSetSchemaService, IRepository<DataSetDetail> repository)
        {
            _dataSetSchemaService = dataSetSchemaService;
            _repository = repository;
        }
        
        
        public UploadResult ProcessCsv(string filePath, DataSetSchema schema, string title)
        {
            var resultData = new UploadResult();

            var datasetDetail = AddDataSetDetailToDatabase(schema.Id, title, new FileInfo(filePath));

            var newData = CreateDataTable(schema.Definition);//create the DataTable to save to the database later

            var errorList = new List<string>();
            var rowNumber = 1;

            using (var csv = new CsvReader(new StreamReader(filePath), true))
            {
                var fieldCount = csv.FieldCount;
                var headers = csv.GetFieldHeaders();
                var breakFromLoop = false;
                csv.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;

                while (csv.ReadNextRecord())
                {
                    var row = newData.NewRow();
                    row["DataSetDetailId"] = datasetDetail.Id;

                    for (var i = 0; i < fieldCount; i++)
                    {
                        try
                        {
                            var validateRules = schema.Definition.Column(headers[i]);
                            if (validateRules == null)
                            {
                                if (headers[i].ToLower() == "publisheruri" || headers[i].ToLower() == "publisher uri" || headers[i].ToLower() == "publisher label" || headers[i].ToLower() == "publisherlabel")//ignore default body and body name
                                    continue;

                                errorList.Add(String.Format("Column: {0} in the csv does not exist in the schema", headers[i]));
                                breakFromLoop = true;
                                break;
                            }

                            var valueToValidate = csv[i];

                            ValidateColumn(valueToValidate, i, headers, validateRules, errorList, rowNumber);

                            if (errorList.Count == 0)
                            {
                                AddColumnData(row, i, headers, valueToValidate, validateRules);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorSignal.FromCurrentContext().Raise(ex);
                            //Catch any errors and pass the message on
                            if (ex is MalformedCsvException)
                            {
                                var message = ex.Message.Substring(0, ex.Message.IndexOf("Current raw data"));
                                errorList.Add(String.Format("Invalid CSV row. Row number: {0} Column: {1} - {2}.", rowNumber, headers[i], message));
                            }
                            else
                            {
                                //Catch any errors and pass the message on
                                errorList.Add(String.Format("Column: {0} - {1}.", headers[i], ex.Message));
                            }
                        }
                    }

                    if (errorList.Count == 0)
                    {
                        newData.Rows.Add(row);
                    }

                    if (breakFromLoop)
                    {
                        break;
                    }
                    rowNumber++;
                }
            }

            if (rowNumber == 1)
            {
                errorList.Add("The selected file contains no data.");
            }

            if (errorList.Any())//delete if errors
            {
                _repository.Delete(datasetDetail);
            }
            else
            {
                datasetDetail.NumOfRows = rowNumber;
            }
            _repository.SaveChanges();

            resultData.Id = datasetDetail.Id;
            resultData.Errors = errorList;
            resultData.Data = newData;

            return resultData;
        }

        public void DeleteCsv(string filepath)
        {
            var uploadedFile = new FileInfo(filepath);
            if(uploadedFile.Exists)//remove the csv file
                uploadedFile.Delete();
        }

        private DataSetDetail AddDataSetDetailToDatabase(int schemaId, string title, FileInfo uploadedFile)
        {
            
            var datasetDetail = new DataSetDetail
            {
                Schema = _dataSetSchemaService.Repository.Query(s => s.Id == schemaId).FirstOrDefault(),
                Title = title,
                VersionNumber = 1,
                XmlFileSize = (uploadedFile.Length * 2),
                CsvFileSize = uploadedFile.Length
            };

            _repository.Add(datasetDetail);
            _repository.SaveChanges();
            return datasetDetail;
        }

        private void ValidateColumn(string valueToValidate, int i, string[] headers, DataSetSchemaColumn validateRules, ICollection<string> errorList, int rowNumber)
        {
            var errorRow = rowNumber + 1;//take header in the csv into account
            if (validateRules.IsRequired)
            {
                if (valueToValidate.Length == 0)
                {
                    errorList.Add(String.Format("Row {0}, Column {1} - is empty. This field must contain a value.", errorRow, headers[i]));

                }
            }

            if (valueToValidate.Length > 0)
            {
                switch (validateRules.Type.ToLower())
                {
                    case "text":
                        if (valueToValidate.Length > validateRules.MaxSize)
                            errorList.Add(String.Format("Row {0}, Column {1} - ({2} characters) is too big. The maximum size is {3}.", errorRow, headers[i], valueToValidate.Length, validateRules.MaxSize));
                        break;
                    case "currency":
                        var result = Regex.Match(valueToValidate, @"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,9})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,9})?))\)?$");
                        if (!result.Success)
                            errorList.Add(String.Format(@"Row {0}, Column {1} - ""{2}"" is not a valid currency value.", errorRow, headers[i], valueToValidate));

                        if ((validateRules.MaxCurrency != null) && (Convert.ToDouble(valueToValidate) > validateRules.MaxCurrency))
                            errorList.Add(String.Format(@"Row {0}, Column {1} - ""{2}"" is too big. The maximum currency amount allowed is {3}.", errorRow, headers[i], valueToValidate, StringExtensions.FormatCurrency(validateRules.MaxCurrency.ToString())));

                        if ((validateRules.MinCurrency != null) && (Convert.ToDouble(valueToValidate) < validateRules.MinCurrency))
                            errorList.Add(String.Format(@"Row {0}, Column {1} - ""{2}"" is too small. The minimum currency amount allowed is {3}.", errorRow, headers[i], valueToValidate, StringExtensions.FormatCurrency(validateRules.MinCurrency.ToString())));

                        break;
                    case "number":
                        var resultNum = Regex.Match(valueToValidate, @"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,9})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,9})?))\)?$");
                        if (!resultNum.Success)
                            errorList.Add(String.Format(@"Row {0}, Column {1} - ""{2}"" is not a valid number value.", errorRow, headers[i], valueToValidate));

                        if ((validateRules.MaxNumber != null) && (Convert.ToDouble(valueToValidate) > validateRules.MaxNumber))
                            errorList.Add(String.Format(@"Row {0}, Column {1} - ""{2}"" is too big. The maximum number allowed is {3}.", errorRow, headers[i], valueToValidate, validateRules.MaxNumber));

                        if ((validateRules.MinNumber != null) && (Convert.ToDouble(valueToValidate) < validateRules.MinNumber))
                            errorList.Add(String.Format(@"Row {0}, Column {1} - ""{2}"" is too small. The minimum number allowed is {3}.", errorRow, headers[i], valueToValidate, validateRules.MinNumber));

                        break;
                    case "datetime":

                        try //DateTime.TryParseExact is far too slow. trt/catch is much faster way to validate the date
                        {
                            var dateValue = Convert.ToDateTime(valueToValidate);
                            if ((validateRules.MaxDate != null) && (dateValue > validateRules.MaxDate))
                                errorList.Add(String.Format(@"Row {0}, Column {1} - ""{2}"" is too big. The maximum date allowed is {3}.", errorRow, headers[i], valueToValidate, validateRules.MaxDate));

                            if (validateRules.MinDate != null && dateValue < validateRules.MinDate)
                                errorList.Add(String.Format(@"Row {0}, Column {1} - ""{2}"" is too small. The minimum date allowed is {3}.", errorRow, headers[i], valueToValidate, validateRules.MinDate));

                        }
                        catch (Exception ex)
                        {
                            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                            errorList.Add(String.Format(@"Row {0}, Column {1} - ""{2}"" is not a valid value for a date.", errorRow, headers[i], valueToValidate));
                        }

                        break;
                }
            }
        }

        private void AddColumnData(DataRow row, int i, string[] headers, string valueToValidate, DataSetSchemaColumn validateRules)
        {
            switch (validateRules.Type.ToLower())
            {
                case "text":
                case "image":
                case "url":
                    row[headers[i]] = !String.IsNullOrEmpty(valueToValidate) ? valueToValidate.Trim() : "";
                    break;
                case "currency":
                    row[headers[i]] = !String.IsNullOrEmpty(valueToValidate) ? Convert.ToDouble(valueToValidate.Trim()) : 0;
                    break;
                case "number":
                    row[headers[i]] = !String.IsNullOrEmpty(valueToValidate) ? Convert.ToDouble(valueToValidate.Trim()) : SqlDouble.Null;
                    break;
                case @"lat/lng":
                    row[headers[i]] = !String.IsNullOrEmpty(valueToValidate) ? Convert.ToDouble(valueToValidate.Trim()) : 0;
                    break;
                case "datetime":
                    row[headers[i]] = !String.IsNullOrEmpty(valueToValidate) ? new SqlDateTime(Convert.ToDateTime(valueToValidate.Trim())) : SqlDateTime.Null;
                    break;
            }
        }

        private DataTable CreateDataTable(DataSetSchemaDefinition schemaDefinition)
        {
            //create the DataTable with default columns and all the ciolumns in the schema
            var newData = new DataTable(schemaDefinition.TableName);

            //add default columns
            newData.Columns.Add(new DataColumn
            {
                DataType = Type.GetType("System.Int32"),
                ColumnName = "Id",
                AutoIncrement = true
            });
            newData.Columns.Add(new DataColumn
            {
                ColumnName = "DataSetDetailId",
                DataType = Type.GetType("System.Int32")

            });

            //add schema columns
            foreach (var h in schemaDefinition.Columns)
            {
                DataColumn newCol;
                switch (h.Type.ToLower())
                {
                    case "text":
                    case "image":
                    case "url":
                        newCol = new DataColumn
                        {
                            ColumnName = h.ColumnName,
                            DataType = typeof(String)
                        };
                        break;
                    case "currency":
                        newCol = new DataColumn
                        {
                            ColumnName = h.ColumnName,
                            DataType = typeof(Double)
                        };
                        break;
                    case "number":
                        newCol = new DataColumn
                        {
                            ColumnName = h.ColumnName,
                            DataType = typeof(SqlDouble)
                        };
                        break;
                    case "datetime":
                        newCol = new DataColumn
                        {
                            ColumnName = h.ColumnName,
                            DataType = typeof(SqlDateTime)
                        };
                        break;
                    default:
                        newCol = new DataColumn
                        {
                            ColumnName = h.ColumnName,
                            DataType = typeof(String)
                        };
                        break;
                }

                newData.Columns.Add(newCol);
            }
            return newData;
        }
    }
}