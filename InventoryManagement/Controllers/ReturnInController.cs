using DinkToPdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using InventoryManagement.IRepository;
using InventoryManagement.Models;
using InventoryManagement.ViewModels;
using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace InventoryManagement.Controllers
{
    public class ReturnInController : Controller
    {

        private readonly IReturnInRepository returninRepository;

        private readonly ApplicationDbContext context;
        private readonly IServiceProvider serviceProvider;
        private string htmlContent;

        public ReturnInController(
                              IReturnInRepository returninRepository,

                              ApplicationDbContext context, IServiceProvider serviceProvider)
        {
           
            this.returninRepository = returninRepository;

            this.context = context;
            this.serviceProvider = serviceProvider;
        }


        //public IActionResult ReturnInIndex()
        //{
        //    var returnIns = returninRepository.GetAllReturnIn();
        //    return View(returnIns);	
        //}

        [HttpGet]
        public IActionResult ReturnInIndex(string search)
        {
            // Retrieve products based on the search parameter
            var returnIns = string.IsNullOrEmpty(search) ? returninRepository.GetAllReturnIn() : returninRepository.SearchReturnIn(search);

            // Pass the filtered products to the view
            return View(returnIns);
        }



        [HttpGet]
        public IActionResult ReturnInCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReturnInCreate(ReturnInViewModel returnin)
        {
            if (ModelState.IsValid)
            {
                var ReturnIn = new ReturnIn
                {
                    ReturnDate = DateTime.Now,
                    ProductSKU = returnin.ProductSKU,
                    
                    ProductId = returnin.ProductId,
                    Quantity = returnin.Quantity
                };
                returninRepository.Add(ReturnIn);
                return RedirectToAction("ReturnInIndex", "ReturnIn");

            }

            return View();
        }


        [HttpGet]
        public IActionResult ReturnInEdit(int id)
        {
            var returnIn = returninRepository.GetById(id);
            return View(returnIn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReturnInEdit(ReturnIn returnIn)
        {
            if (ModelState.IsValid)
            {
                returninRepository.Update(returnIn);
                return RedirectToAction("ReturnInIndex");
            }
            return View(returnIn);
        }




        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReturnInDelete(int id)
        {
            // Ensure that the product exists
            var returnIn = returninRepository.GetById(id);
            if (returnIn == null)
            {
                return NotFound();
            }

            // Delete the product
            returninRepository.Delete(id);

            return RedirectToAction("ReturnInIndex");
        }

        public IActionResult ExportToExcel()
        {
            // Retrieve the data you want to export (e.g., a list of products)
            var returnIns = returninRepository.GetAllReturnIn();

            // Create an instance of the ExcelPackage
            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Set the title
                worksheet.Cells["A1"].Value = "Product Report"; // Title text
                worksheet.Cells["A1:E1"].Merge = true; // Merge cells for the title
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:E1"].Style.Font.Size = 23; // Increase the font size


                // Apply bold formatting to the header cells
                worksheet.Cells["A2:E2"].Style.Font.Bold = true;
                worksheet.Cells["A2:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:E2"].Style.Font.Size = 13; // Increase the font size


                // Add headers
                worksheet.Cells["A2"].Value = "Date";
                worksheet.Cells["B2"].Value = "ProductName";
                worksheet.Cells["C2"].Value = "SKU";
                worksheet.Cells["D2"].Value = "Price";
                worksheet.Cells["E2"].Value = "Quantity";


                // Increase the width of the columns
                worksheet.Column(1).Width = 25; // Date column
                worksheet.Column(2).Width = 30; // Product Name column
                worksheet.Column(3).Width = 15; // SKU column
                worksheet.Column(4).Width = 15; // Price column
                worksheet.Column(5).Width = 15; // Quantity column

                // Add data to the worksheet
                int row = 3;
                foreach (var ReturnIns in returnIns)
                {

                    worksheet.Cells["A" + row].Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss AM/PM";
                    worksheet.Cells[$"A{row}"].Value = ReturnIns.ReturnDate;
                    worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                    worksheet.Cells[$"B{row}"].Value = ReturnIns.ProductSKU;
                    worksheet.Cells[$"B{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align ProductName
                    worksheet.Cells[$"C{row}"].Value = ReturnIns.ProductId;
                    worksheet.Cells[$"C{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align SKU
                    worksheet.Cells[$"E{row}"].Value = ReturnIns.Quantity;
                    worksheet.Cells[$"E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Quantity
                    row++;
                }



                // Add a row for the total amount
                worksheet.Cells[$"A{row}"].Value = "Total Amount";
                
                worksheet.Cells[$"E{row}"].Formula = $"SUM(E3:E{row - 1})"; // Assuming price is in column E
                worksheet.Cells[$"E{row}"].Style.Font.Bold = true;

                // Auto-fit columns to make the content fit properly
                //worksheet.Cells.AutoFitColumns();

                // Return the Excel file as a downloadable file
                byte[] excelData = package.GetAsByteArray();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
            }
        }




    }
}








    

