using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using InventoryManagement.IRepository;
using InventoryManagement.Models;
using InventoryManagement.ViewModels;
using System;
using InventoryManagement;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace InventoryManagement.Controllers
{
    public class OrderOutController : Controller
    {
        private readonly IOrderOutRepository orderOutRepository;
        private readonly ApplicationDbContext context;

        public OrderOutController(
                              IOrderOutRepository orderOutRepository,

                              ApplicationDbContext context)
        {
            this.orderOutRepository = orderOutRepository;
            this.context = context;
        }


        //public IActionResult OrderOutIndex()
        //{
        //    var orderOuts = orderOutRepository.GetAllOrderOut();
        //    return View(orderOuts);
        //}

        [HttpGet]
        public IActionResult OrderOutIndex(string search)
        {
            // Retrieve products based on the search parameter
            var products = string.IsNullOrEmpty(search) ? orderOutRepository.GetAllOrderOut() : orderOutRepository.SearchOrderOut(search);

            // Pass the filtered products to the view
            return View(products);
        }

        [HttpGet]
        public IActionResult OrderOutCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderOutCreate(OrderOutViewModel OrderOut)
        {
            if (ModelState.IsValid)
            {
                var orderOut = new OrderOut
                {
                    Date = DateTime.Now,
                    ProductId = OrderOut.ProductId,
                    ProductSKU = OrderOut.ProductSKU,
                    Quantity = OrderOut.Quantity
                };
                orderOutRepository.Add(orderOut);
                return RedirectToAction("OrderOutIndex", "OrderOut");

            }

            return View();
        }


        [HttpGet]
        public IActionResult OrderOutEdit(int id)
        {
            var orderOut = orderOutRepository.GetById(id);
            return View(orderOut);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderOutEdit(OrderOut orderOut)
        {
            if (ModelState.IsValid)
            {
                orderOutRepository.Update(orderOut);
                return RedirectToAction("OrderOutIndex");
            }
            return View(orderOut);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderOutDelete(int id)
        {
            // Ensure that the product exists
            var orderOut = orderOutRepository.GetById(id);
            if (orderOut == null)
            {
                return NotFound();
            }

            // Delete the product
            orderOutRepository.Delete(id);

            return RedirectToAction("OrderOutIndex");
        }

        public IActionResult ExportToExcel()
        {
            // Retrieve the data you want to export (e.g., a list of products)
            var orderOuts = orderOutRepository.GetAllOrderOut();

            // Create an instance of the ExcelPackage
            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("OrderOut");

                // Set the title
                worksheet.Cells["A1"].Value = "Order Report"; // Title text
                worksheet.Cells["A1:E1"].Merge = true; // Merge cells for the title
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:E1"].Style.Font.Size = 23; // Increase the font size


                // Apply bold formatting to the header cells
                worksheet.Cells["A2:D2"].Style.Font.Bold = true;
                worksheet.Cells["A2:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:D2"].Style.Font.Size = 13; // Increase the font size

                // Add headers
                worksheet.Cells["A2"].Value = "Date";
                worksheet.Cells["B2"].Value = "ProductId";
                worksheet.Cells["C2"].Value = "ProductSKU";
                worksheet.Cells["D2"].Value = "Quantity";


                // Increase the width of the columns
                worksheet.Column(1).Width = 25; // Date column
                worksheet.Column(2).Width = 30; // Product Id column
                worksheet.Column(3).Width = 20; // Product SKU column
                worksheet.Column(4).Width = 15; // Quantity column
                


                // Add data to the worksheet
                int row = 3;
                foreach (var orderOut in orderOuts)
                {

                    worksheet.Cells["A" + row].Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss AM/PM";
                    worksheet.Cells[$"A{row}"].Value = orderOut.Date;
                    worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                    worksheet.Cells[$"B{row}"].Value = orderOut.ProductId;
                    worksheet.Cells[$"B{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                    worksheet.Cells[$"C{row}"].Value = orderOut.ProductSKU;
                    worksheet.Cells[$"C{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                    worksheet.Cells[$"D{row}"].Value = orderOut.Quantity;
                    worksheet.Cells[$"D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                    row++;
                }

                // Auto-fit columns to make the content fit properly
                //worksheet.Cells.AutoFitColumns();

                // Return the Excel file as a downloadable file
                byte[] excelData = package.GetAsByteArray();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
            }
        }



        }
}