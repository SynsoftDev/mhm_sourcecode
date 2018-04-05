using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using MHMDal.Models;
using SynSecurity;

namespace MHM.Api.Helpers
{
    public class ImportExcel
    {
        MHMDal.Models.MHM DB = new MHMDal.Models.MHM();

        public DataSet ImportExcel1()
        {
            //Save the uploaded Excel file.
            string filePath = HttpContext.Current.Server.MapPath("~/ExcelFiles/ExcelCaseRecordLayout-2016.xlsx");

            //Open the Excel file in Read Mode using OpenXml.
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
            {
                //Read the first Sheets from Excel file.
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

                //Get the Worksheet instance.
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

                //Fetch all the rows present in the Worksheet.
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
                List<CellFormats> formats = new List<CellFormats>(doc.WorkbookPart.WorkbookStylesPart.Stylesheet.Descendants<CellFormats>());

                //Create a new DataTable.
                DataTable dtCase = new DataTable("Case");
                dtCase.Columns.Add("CaseNumber");
                dtCase.Columns["CaseNumber"].DataType = Type.GetType("System.Int32");
                DataTable dtApplicant = new DataTable("Applicant");
                dtApplicant.Columns.Add("CaseNumber");
                dtApplicant.Columns["CaseNumber"].DataType = Type.GetType("System.Int32");

                DataTable dtBenefitCosts = new DataTable("BenefitCosts");
                dtBenefitCosts.Columns.Add("CaseNumber");
                dtBenefitCosts.Columns["CaseNumber"].DataType = Type.GetType("System.Int32");
                dtBenefitCosts.Columns.Add("BenefitName");
                dtBenefitCosts.Columns.Add("UsageCost");

                DataTable dtFamilies = new DataTable("Families");
                dtFamilies.Columns.Add("CaseNumber");
                dtFamilies.Columns["CaseNumber"].DataType = Type.GetType("System.Int32");
                dtFamilies.Columns.Add("IsPrimary");
                dtFamilies.Columns.Add("Gender");
                dtFamilies.Columns.Add("DOB");
                dtFamilies.Columns.Add("Smoking");

                DataTable dtBenefitUseDetail = new DataTable("BenefitUseDetail");
                dtBenefitUseDetail.Columns.Add("CaseNumber");
                dtBenefitUseDetail.Columns["CaseNumber"].DataType = Type.GetType("System.Int32");
                dtBenefitUseDetail.Columns.Add("FamilyNumber");
                dtBenefitUseDetail.Columns["FamilyNumber"].DataType = Type.GetType("System.Int32");
                dtBenefitUseDetail.Columns.Add("BenefitName");
                dtBenefitUseDetail.Columns.Add("UsageQty");
                dtBenefitUseDetail.Columns.Add("UsageCost");

                DataTable dtBenefitNotes = new DataTable("BenefitNotes");
                dtBenefitNotes.Columns.Add("CaseNumber");
                dtBenefitNotes.Columns["CaseNumber"].DataType = Type.GetType("System.Int32");
                dtBenefitNotes.Columns.Add("BenefitName");
                dtBenefitNotes.Columns.Add("UsageNotes");

                DataTable dtCasePlanResult = new DataTable("CasePlanResult");
                dtCasePlanResult.Columns.Add("CaseNumber");
                dtCasePlanResult.Columns["CaseNumber"].DataType = Type.GetType("System.Int32");
                dtCasePlanResult.Columns.Add("Rank");

                //Loop through the Worksheet rows.
                foreach (Row row in rows.Skip(10))
                {
                    int cellIndex = 0;
                    bool newColumn = false;
                    //int cellIndex = 0;
                    string tableName = "";
                    string ColumnName = "";
                    string BenefitName = "";

                    var CaseNumbersCells = GetRowCells(rows.Skip(9).Take(1).FirstOrDefault());

                    List<Cell> lstCells = GetRowCells(row).ToList();
                    if (lstCells.Count() < CaseNumbersCells.Count())
                    {
                        int noOfRecordMissed = CaseNumbersCells.Count() - lstCells.Count();
                        for (int i = 0; i < noOfRecordMissed; i++)
                        {
                            lstCells.Add(new Cell());
                        }
                    }
                    int index = 0;
                    int count = lstCells.Count();
                    for (index = 0; index < count; index++)
                    {
                        var CaseId = CaseNumbersCells.ElementAt(index).InnerText;
                        string tempValue = GetValue(lstCells, formats, doc, index);

                        string value = "";
                        if (index == 2)
                        {
                            BenefitName = tempValue;
                        }
                        if (index == 3)
                        {
                            tableName = tempValue;
                        }
                        else if (index == 4)
                        {
                            ColumnName = tempValue;
                        }
                        else if (index > 4)
                        {
                            value = tempValue;
                        }
                        if (tableName == "Case")
                        {
                            if (ColumnName != "")
                            {
                                if (!dtCase.Columns.Contains(ColumnName))
                                {
                                    if (!dtCase.Columns.Contains("CaseNumber"))
                                    {
                                        dtCase.Columns.Add("CaseNumber");
                                    };
                                    dtCase.Columns.Add(ColumnName);
                                    if (dtCase.Rows.Count == 0) { newColumn = true; }
                                }
                                else
                                {
                                    if (newColumn)
                                    {
                                        DataRow dr = dtCase.NewRow();
                                        dr["CaseNumber"] = int.Parse(CaseId);
                                        dr[ColumnName] = value;
                                        dtCase.Rows.Add(dr);
                                    }
                                    else
                                    {
                                        dtCase.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                        dtCase.Rows[cellIndex][ColumnName] = value;
                                        cellIndex++;
                                    }
                                }
                            }
                        }
                        else if (tableName == "Applicant")
                        {
                            if (ColumnName != "")
                            {
                                if (!dtApplicant.Columns.Contains(ColumnName))
                                {
                                    if (!dtApplicant.Columns.Contains("CaseNumber"))
                                    {
                                        dtApplicant.Columns.Add("CaseNumber");
                                    };
                                    dtApplicant.Columns.Add(ColumnName);
                                    if (dtApplicant.Rows.Count == 0) { newColumn = true; }
                                }
                                else
                                {
                                    if (newColumn)
                                    {
                                        DataRow dr = dtApplicant.NewRow();
                                        dr["CaseNumber"] = int.Parse(CaseId);
                                        dr[ColumnName] = value;
                                        dtApplicant.Rows.Add(dr);
                                    }
                                    else
                                    {
                                        dtApplicant.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                        dtApplicant.Rows[cellIndex][ColumnName] = value;
                                        cellIndex++;
                                    }
                                }
                            }
                        }
                        else if (tableName == "BenefitUseDetail : CaseID>>FamilyID(all) >> MHMMappingBenefitId")
                        {
                            if (index > 4)
                            {

                                DataRow dr = dtBenefitCosts.NewRow();
                                dr["BenefitName"] = BenefitName;
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr[ColumnName] = value;
                                dtBenefitCosts.Rows.Add(dr);

                            }
                        }
                        else if (tableName == "Family (FamilyID of Individual 1)")
                        {
                            if (index > 4)
                            {
                                if (dtFamilies.Rows.Count == 0) { newColumn = true; }

                                if (newColumn)
                                {
                                    DataRow dr = dtFamilies.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr[ColumnName] = value;
                                    dtFamilies.Rows.Add(dr);
                                }
                                else
                                {
                                    dtFamilies.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtFamilies.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }

                            }
                        }
                        else if (tableName == "BenefitUseDetail : (FamilyID of Individual 1)>>MHMMappingBenefitId")
                        {
                            if (index > 4)
                            {
                                DataRow dr = dtBenefitUseDetail.NewRow();
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr["BenefitName"] = BenefitName;
                                dr["FamilyNumber"] = 1;
                                string filterExp = "CaseNumber = 1 and BenefitName='" + BenefitName + "'";
                                var Rate = dtBenefitCosts.Select(filterExp).First()["UsageCost"];
                                dr["UsageCost"] = value != "" ? Convert.ToDecimal(Rate) * decimal.Parse(value) : 0;
                                dr[ColumnName] = value;
                                dtBenefitUseDetail.Rows.Add(dr);
                            }
                        }
                        else if (tableName == "Family (FamilyID of Individual 2)")
                        {
                            if (index > 4)
                            {
                                if (dtFamilies.Rows.Count == (count - 5) * 1) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtFamilies.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr[ColumnName] = value;
                                    dtFamilies.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = count - 5; }
                                    dtFamilies.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtFamilies.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "BenefitUseDetail : (FamilyID of Individual 2)>>MHMMappingBenefitId")
                        {
                            if (index > 4)
                            {
                                DataRow dr = dtBenefitUseDetail.NewRow();
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr["BenefitName"] = BenefitName;
                                dr["FamilyNumber"] = 2;
                                string filterExp = "CaseNumber = 2 and BenefitName='" + BenefitName + "'";
                                var Rate = dtBenefitCosts.Select(filterExp).First()["UsageCost"];
                                dr["UsageCost"] = value != "" ? Convert.ToDecimal(Rate) * decimal.Parse(value) : 0;
                                dr[ColumnName] = value;
                                dtBenefitUseDetail.Rows.Add(dr);
                            }
                        }
                        else if (tableName == "Family (FamilyID of Individual 3)")
                        {
                            if (index > 4)
                            {
                                if (dtFamilies.Rows.Count == (count - 5) * 2) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtFamilies.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr[ColumnName] = value;
                                    dtFamilies.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 2; }
                                    dtFamilies.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtFamilies.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "BenefitUseDetail : (FamilyID of Individual 3)>>MHMMappingBenefitId")
                        {
                            if (index > 4)
                            {
                                DataRow dr = dtBenefitUseDetail.NewRow();
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr["BenefitName"] = BenefitName;
                                dr["FamilyNumber"] = 3;
                                string filterExp = "CaseNumber = 3 and BenefitName='" + BenefitName + "'";
                                var Rate = dtBenefitCosts.Select(filterExp).First()["UsageCost"];
                                dr["UsageCost"] = value != "" ? Convert.ToDecimal(Rate) * decimal.Parse(value) : 0;
                                dr[ColumnName] = value;
                                dtBenefitUseDetail.Rows.Add(dr);
                            }
                        }
                        else if (tableName == "Family (FamilyID of Individual 4)")
                        {
                            if (index > 4)
                            {
                                //var test = (count - 5) * 2;
                                if (dtFamilies.Rows.Count == (count - 5) * 3) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtFamilies.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr[ColumnName] = value;
                                    dtFamilies.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 3; }
                                    dtFamilies.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtFamilies.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "BenefitUseDetail : (FamilyID of Individual 4)>>MHMMappingBenefitId")
                        {
                            if (index > 4)
                            {
                                DataRow dr = dtBenefitUseDetail.NewRow();
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr["BenefitName"] = BenefitName;
                                dr["FamilyNumber"] = 4;
                                string filterExp = "CaseNumber = 4 and BenefitName='" + BenefitName + "'";
                                var Rate = dtBenefitCosts.Select(filterExp).First()["UsageCost"];
                                dr["UsageCost"] = value != "" ? Convert.ToDecimal(Rate) * decimal.Parse(value) : 0;
                                dr[ColumnName] = value;
                                dtBenefitUseDetail.Rows.Add(dr);
                            }
                        }
                        else if (tableName == "Family (FamilyID of Individual 5)")
                        {
                            if (index > 4)
                            {
                                if (dtFamilies.Rows.Count == (count - 5) * 4) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtFamilies.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr[ColumnName] = value;
                                    dtFamilies.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 4; }
                                    dtFamilies.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtFamilies.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "BenefitUseDetail : (FamilyID of Individual 5)>>MHMMappingBenefitId")
                        {
                            if (index > 4)
                            {
                                DataRow dr = dtBenefitUseDetail.NewRow();
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr["BenefitName"] = BenefitName;
                                dr["FamilyNumber"] = 5;
                                string filterExp = "CaseNumber = 5 and BenefitName='" + BenefitName + "'";
                                var Rate = dtBenefitCosts.Select(filterExp).First()["UsageCost"];
                                dr["UsageCost"] = value != "" ? Convert.ToDecimal(Rate) * decimal.Parse(value) : 0;
                                dr[ColumnName] = value;
                                dtBenefitUseDetail.Rows.Add(dr);
                            }
                        }
                        else if (tableName == "Family (FamilyID of Individual 6)")
                        {
                            if (index > 4)
                            {
                                if (dtFamilies.Rows.Count == (count - 5) * 5) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtFamilies.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr[ColumnName] = value;
                                    dtFamilies.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 5; }
                                    dtFamilies.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtFamilies.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "BenefitUseDetail : (FamilyID of Individual 6)>>MHMMappingBenefitId")
                        {
                            if (index > 4)
                            {
                                DataRow dr = dtBenefitUseDetail.NewRow();
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr["BenefitName"] = BenefitName;
                                dr["FamilyNumber"] = 6;
                                string filterExp = "CaseNumber = 6 and BenefitName='" + BenefitName + "'";
                                var Rate = dtBenefitCosts.Select(filterExp).First()["UsageCost"];
                                dr["UsageCost"] = value != "" ? Convert.ToDecimal(Rate) * decimal.Parse(value) : 0;
                                dr[ColumnName] = value;
                                dtBenefitUseDetail.Rows.Add(dr);
                            }
                        }
                        else if (tableName == "Family (FamilyID of Individual 7)")
                        {
                            if (index > 4)
                            {
                                if (dtFamilies.Rows.Count == (count - 5) * 6) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtFamilies.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr[ColumnName] = value;
                                    dtFamilies.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 6; }
                                    dtFamilies.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtFamilies.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "BenefitUseDetail : (FamilyID of Individual 7)>>MHMMappingBenefitId")
                        {
                            if (index > 4)
                            {
                                DataRow dr = dtBenefitUseDetail.NewRow();
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr["BenefitName"] = BenefitName;
                                dr["FamilyNumber"] = 7;
                                string filterExp = "CaseNumber = 7 and BenefitName='" + BenefitName + "'";
                                var Rate = dtBenefitCosts.Select(filterExp).First()["UsageCost"];
                                dr["UsageCost"] = value != "" ? Convert.ToDecimal(Rate) * decimal.Parse(value) : 0;
                                dr[ColumnName] = value;
                                dtBenefitUseDetail.Rows.Add(dr);
                            }
                        }
                        else if (tableName == "Family (FamilyID of Individual 8)")
                        {
                            if (index > 4)
                            {
                                if (dtFamilies.Rows.Count == (count - 5) * 7) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtFamilies.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr[ColumnName] = value;
                                    dtFamilies.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 7; }
                                    dtFamilies.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtFamilies.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "BenefitUseDetail : (FamilyID of Individual 8)>>MHMMappingBenefitId")
                        {
                            if (index > 4)
                            {
                                DataRow dr = dtBenefitUseDetail.NewRow();
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr["BenefitName"] = BenefitName;
                                dr["FamilyNumber"] = 8;
                                string filterExp = "CaseNumber = 8 and BenefitName='" + BenefitName + "'";
                                var Rate = dtBenefitCosts.Select(filterExp).First()["UsageCost"];
                                dr["UsageCost"] = value != "" ? Convert.ToDecimal(Rate) * decimal.Parse(value) : 0;
                                dr[ColumnName] = value;
                                dtBenefitUseDetail.Rows.Add(dr);
                            }
                        }
                        else if (tableName == "BenefitUseDetail : CaseID >> FamilyID (all)")
                        {
                            if (index > 4)
                            {
                                DataRow dr = dtBenefitNotes.NewRow();
                                dr["CaseNumber"] = int.Parse(CaseId);
                                dr["BenefitName"] = BenefitName.Remove(0, BenefitName.IndexOf(':') + 2);
                                dr[ColumnName] = value;
                                dtBenefitNotes.Rows.Add(dr);
                            }
                        }
                        else if (tableName == "CasePlanResults :  CaseID >> CasePlanResultId (of Case Rank = 1)")
                        {
                            if (ColumnName != "")
                            {
                                if (!dtCasePlanResult.Columns.Contains(ColumnName))
                                {
                                    dtCasePlanResult.Columns.Add(ColumnName);
                                    if (dtCasePlanResult.Rows.Count == 0) { newColumn = true; }
                                }
                                else
                                {
                                    if (newColumn)
                                    {
                                        DataRow dr = dtCasePlanResult.NewRow();
                                        dr["CaseNumber"] = int.Parse(CaseId);
                                        dr["Rank"] = 1;
                                        dr[ColumnName] = value;
                                        dtCasePlanResult.Rows.Add(dr);
                                    }
                                    else
                                    {
                                        dtCasePlanResult.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                        dtCasePlanResult.Rows[cellIndex]["Rank"] = 1;
                                        dtCasePlanResult.Rows[cellIndex][ColumnName] = value;
                                        cellIndex++;
                                    }
                                }
                            }
                        }
                        else if (tableName == "CasePlanResults :  CaseID >> CasePlanResultId (of Case Rank = 2)")
                        {
                            if (index > 4)
                            {
                                if (dtCasePlanResult.Rows.Count == (count - 5) * 1) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtCasePlanResult.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr["Rank"] = 2;
                                    dr[ColumnName] = value;
                                    dtCasePlanResult.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = count - 5; }
                                    dtCasePlanResult.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtCasePlanResult.Rows[cellIndex]["Rank"] = 2;
                                    dtCasePlanResult.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "CasePlanResults :  CaseID >> CasePlanResultId (of Case Rank = 3)")
                        {
                            if (index > 4)
                            {
                                if (dtCasePlanResult.Rows.Count == (count - 5) * 2) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtCasePlanResult.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr["Rank"] = 3;
                                    dr[ColumnName] = value;
                                    dtCasePlanResult.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 2; }
                                    dtCasePlanResult.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtCasePlanResult.Rows[cellIndex]["Rank"] = 3;
                                    dtCasePlanResult.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "CasePlanResults :  CaseID >> CasePlanResultId (of Case Rank = 4)")
                        {
                            if (index > 4)
                            {
                                if (dtCasePlanResult.Rows.Count == (count - 5) * 3) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtCasePlanResult.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr["Rank"] = 4;
                                    dr[ColumnName] = value;
                                    dtCasePlanResult.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 3; }
                                    dtCasePlanResult.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtCasePlanResult.Rows[cellIndex]["Rank"] = 4;
                                    dtCasePlanResult.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "CasePlanResults :  CaseID >> CasePlanResultId (of Case Rank = 5)")
                        {
                            if (index > 4)
                            {
                                if (dtCasePlanResult.Rows.Count == (count - 5) * 4) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtCasePlanResult.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr["Rank"] = 5;
                                    dr[ColumnName] = value;
                                    dtCasePlanResult.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 4; }
                                    dtCasePlanResult.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtCasePlanResult.Rows[cellIndex]["Rank"] = 5;
                                    dtCasePlanResult.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                        else if (tableName == "CasePlanResults :  CaseID >> CasePlanResultId (of Case Rank = 5)")
                        {
                            if (index > 4)
                            {
                                if (dtCasePlanResult.Rows.Count == (count - 5) * 6) { newColumn = true; }
                                if (newColumn)
                                {
                                    DataRow dr = dtCasePlanResult.NewRow();
                                    dr["CaseNumber"] = int.Parse(CaseId);
                                    dr["Rank"] = 6;
                                    dr[ColumnName] = value;
                                    dtCasePlanResult.Rows.Add(dr);
                                }
                                else
                                {
                                    if (index == 5) { cellIndex = (count - 5) * 6; }
                                    dtCasePlanResult.Rows[cellIndex]["CaseNumber"] = int.Parse(CaseId);
                                    dtCasePlanResult.Rows[cellIndex]["Rank"] = 6;
                                    dtCasePlanResult.Rows[cellIndex][ColumnName] = value;
                                    cellIndex++;
                                }
                            }
                        }
                    }
                }

                DataRow[] result = dtFamilies.Select("Gender = ''");
                foreach (DataRow row in result)
                {
                    if (row["Gender"].ToString().Trim() == "")
                        dtFamilies.Rows.Remove(row);
                }

                result = dtBenefitUseDetail.Select("UsageQty = ''");
                foreach (DataRow row in result)
                {
                    if (row["UsageQty"].ToString().Trim() == "")
                        dtBenefitUseDetail.Rows.Remove(row);
                }

                result = dtBenefitNotes.Select("UsageNotes = ''");
                foreach (DataRow row in result)
                {
                    if (row["UsageNotes"].ToString().Trim() == "")
                        dtBenefitNotes.Rows.Remove(row);
                }

                dtCase.DefaultView.Sort = "CaseNumber";
                dtApplicant.DefaultView.Sort = "CaseNumber";
                dtBenefitCosts.DefaultView.Sort = "CaseNumber";
                dtFamilies.DefaultView.Sort = "CaseNumber";
                dtBenefitUseDetail.DefaultView.Sort = "CaseNumber, FamilyNumber";
                dtBenefitNotes.DefaultView.Sort = "CaseNumber";
                dtCasePlanResult.DefaultView.Sort = "CaseNumber, Rank";

                DataSet ds = new DataSet();
                ds.Tables.Add(dtCase);
                ds.Tables.Add(dtApplicant);
                ds.Tables.Add(dtBenefitCosts);
                ds.Tables.Add(dtFamilies);
                ds.Tables.Add(dtBenefitUseDetail);
                ds.Tables.Add(dtBenefitNotes);
                ds.Tables.Add(dtCasePlanResult);

                return ds;
            }
        }

        #region ExternalMethods
        ///<summary>returns an empty cell when a blank cell is encountered
        ///</summary>
        public static IEnumerable<Cell> GetRowCells(Row row)
        {
            int currentCount = 0;

            foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in
                row.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>())
            {

                string columnName = GetColumnName(cell.CellReference);

                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    yield return new DocumentFormat.OpenXml.Spreadsheet.Cell();
                }

                yield return cell;
                currentCount++;
            }
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Column Name (ie. B)</returns>
        public static string GetColumnName(string cellReference)
        {
            // Match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);

            return match.Value;
        }

        /// <summary>
        /// Given just the column name (no row index),
        /// it will return the zero based column index.
        /// </summary>
        /// <param name="columnName">Column Name (ie. A or AB)</param>
        /// <returns>Zero based index if the conversion was successful</returns>
        /// <exception cref="ArgumentException">thrown if the given string
        /// contains characters other than uppercase letters</exception>
        public static int ConvertColumnNameToNumber(string columnName)
        {
            Regex alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();

            char[] colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            int convertedValue = 0;
            for (int i = 0; i < colLetters.Length; i++)
            {
                char letter = colLetters[i];
                int current = i == 0 ? letter - 65 : letter - 64; // ASCII 'A' = 65
                convertedValue += current * (int)Math.Pow(26, i);
            }

            return convertedValue;
        }
        #endregion

        public string GetValue(IEnumerable<Cell> lstCell, List<CellFormats> formats, SpreadsheetDocument doc, int index)
        {
            string tempValue = "";
            if (lstCell.ElementAt(index).DataType != null && lstCell.ElementAt(index).DataType.Value == CellValues.SharedString)
            {

                if (lstCell.ElementAt(index).Count() == 0) { tempValue = ""; }
                else
                {
                    tempValue = doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(lstCell.ElementAt(index).InnerText)).InnerText;
                }
            }
            else if (lstCell.ElementAt(index).StyleIndex != null && lstCell.ElementAt(index).StyleIndex.HasValue)
            {
                // look up the style used for the cell
                int formatStyleIndex = Convert.ToInt32(lstCell.ElementAt(index).StyleIndex.Value);
                CellFormat cf = formats[0].Descendants<CellFormat>().ToList()[formatStyleIndex];

                // now you can check the NumberFormatId
                if (cf.NumberFormatId == 14 || cf.NumberFormatId == 168 || cf.NumberFormatId == 167 || cf.NumberFormatId == 164) // 21 is the “tt:mm:ss” format
                {
                    // and the culture seem to be english-US 
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("EN-US");
                    // one second is
                    double second = Convert.ToDouble("1.1574074074074073E-5", ci.NumberFormat);
                    // get what’s in the cell
                    CellValue customValue = lstCell.ElementAt(index).CellValue;
                    // convert that value to a double
                    if (lstCell.ElementAt(index).Count() == 0) { tempValue = ""; }
                    else
                    {
                        double cellSeconds = Convert.ToDouble(lstCell.ElementAt(index).InnerText, ci.NumberFormat);

                        tempValue = DateTime.FromOADate(cellSeconds).ToString();
                    }
                }
                else
                {
                    if (lstCell.ElementAt(index).Count() == 0) { tempValue = ""; }
                    else
                    {
                        tempValue = lstCell.ElementAt(index).InnerText;
                    }
                }
            }
            else
            {
                if (lstCell.ElementAt(index).Count() == 0) { tempValue = ""; }
                else
                {
                    tempValue = lstCell.ElementAt(index).InnerText;
                }
            }
            return tempValue;
        }
    }
}