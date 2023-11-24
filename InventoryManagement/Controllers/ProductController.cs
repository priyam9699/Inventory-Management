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
    public class ProductController : Controller
    {

        private readonly IProductRepository productRepository;

        private readonly ApplicationDbContext context;
        private readonly IServiceProvider serviceProvider;
        private string htmlContent;

        public ProductController(
                              IProductRepository productRepository,

                              ApplicationDbContext context, IServiceProvider serviceProvider)
        {
           
            this.productRepository = productRepository;

            this.context = context;
            this.serviceProvider = serviceProvider;
        }

        //[HttpGet]
        //public IActionResult ProductIndex()
        //{
        //    var product = productRepository.GetAllProduct();
        //    return View(product);	
        //}

        [HttpGet]
        public IActionResult ProductIndex(string search)
        {
            // Retrieve products based on the search parameter
            var products = string.IsNullOrEmpty(search) ? productRepository.GetAllProduct() : productRepository.SearchProducts(search);

            // Pass the filtered products to the view
            return View(products);
        }



        [HttpGet]
        public IActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProductCreate(ProductViewModel product)
        {
            if (ModelState.IsValid)
            {
                var Product = new Product
                {
                    Date = DateTime.Now,
                    ProductName = product.ProductName,
                    SKU = product.SKU,
                    Price = product.Price,
                    Quantity = product.Quantity
                };
                productRepository.Add(Product);
                return RedirectToAction("ProductIndex", "Product");

            }

            return View();
        }


        [HttpGet]
        public IActionResult ProductEdit(int id)
        {
            var product = productRepository.GetById(id);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProductEdit(Product product)
        {
            if (ModelState.IsValid)
            {
                productRepository.Update(product);
                return RedirectToAction("ProductIndex");
            }
            return View(product);
        }




        

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult ProductDelete(int id)
        {
            // Ensure that the product exists
            var product = productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            // Delete the product
            productRepository.Delete(id);

            return RedirectToAction("ProductIndex");
        }

        public IActionResult ExportToExcel()
        {
            // Retrieve the data you want to export (e.g., a list of products)
            var product = productRepository.GetAllProduct();

            // Create an instance of the ExcelPackage
            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Set the title
                worksheet.Cells["A1"].Value = "Product Report"; // Title text
                worksheet.Cells["A1:F1"].Merge = true; // Merge cells for the title
                worksheet.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:F1"].Style.Font.Size = 23; // Increase the font size



                // Apply bold formatting to the header cells
                worksheet.Cells["A2:F2"].Style.Font.Bold = true;
                worksheet.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:F2"].Style.Font.Size = 13; // Increase the font size


                // Add headers
                worksheet.Cells["A2"].Value = "Date";
                worksheet.Cells["B2"].Value = "ProductName";
                worksheet.Cells["C2"].Value = "SKU";
                worksheet.Cells["D2"].Value = "Price";
                worksheet.Cells["E2"].Value = "Quantity";
                worksheet.Cells["F2"].Value = "Total Amount";


                // Increase the width of the columns
                worksheet.Column(1).Width = 25; // Date column
                worksheet.Column(2).Width = 30; // Product Name column
                worksheet.Column(3).Width = 15; // SKU column
                worksheet.Column(4).Width = 15; // Price column
                worksheet.Column(5).Width = 15; // Quantity column
                worksheet.Column(6).Width = 15; // Total column

                // Add data to the worksheet
                int row = 3;
                foreach (var products in product)
                {

                    worksheet.Cells["A" + row].Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss AM/PM";
                    worksheet.Cells[$"A{row}"].Value = products.Date;
                    worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                    worksheet.Cells[$"B{row}"].Value = products.ProductName;
                    worksheet.Cells[$"B{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align ProductName
                    worksheet.Cells[$"C{row}"].Value = products.SKU;
                    worksheet.Cells[$"C{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align SKU
                    worksheet.Cells[$"D{row}"].Value = products.Price;
                    worksheet.Cells[$"D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Price
                    worksheet.Cells[$"E{row}"].Value = products.Quantity;
                    worksheet.Cells[$"E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Quantity
                    worksheet.Cells[$"F{row}"].Value = products.Price * products.Quantity;
                    worksheet.Cells[$"F{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Quantity
                    row++;
                }



                // Add a row for the total amount
                
                
                worksheet.Cells[$"E{row}"].Formula = $"SUM(E3:E{row - 1})"; // Assuming price is in column E
                worksheet.Cells[$"E{row}"].Style.Font.Bold = true;

                // Calculate total quantity
                worksheet.Cells[$"F{row}"].Value = "Total Amount";
                worksheet.Cells[$"F{row}"].Formula = $"SUM(F3:F{row - 1})"; // Assuming Total is in column F
                worksheet.Cells[$"F{row}"].Style.Font.Bold = true;

                // Auto-fit columns to make the content fit properly
                //worksheet.Cells.AutoFitColumns();

                // Return the Excel file as a downloadable file
                byte[] excelData = package.GetAsByteArray();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
            }
        }




    }
}








    

