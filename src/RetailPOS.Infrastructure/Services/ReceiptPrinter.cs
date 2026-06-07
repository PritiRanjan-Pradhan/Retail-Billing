using System;
using System.IO;
using RetailPOS.Application.Interfaces;

namespace RetailPOS.Infrastructure.Services
{
    public class ReceiptPrinter : IReceiptPrinter
    {
        public string GenerateReceipt(string? customerName, System.Collections.Generic.List<(string productName, int quantity, decimal unitPrice, decimal lineTotal)> items, decimal total)
        {
            var lines = new System.Text.StringBuilder();
            lines.AppendLine("RetailPOS Receipt");
            lines.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            lines.AppendLine($"Customer: {customerName ?? "Walk-in"}");
            lines.AppendLine(new string('-', 40));
            foreach (var item in items)
            {
                lines.AppendLine($"{item.productName} x{item.quantity} @ {item.unitPrice:C} = {item.lineTotal:C}");
            }
            lines.AppendLine(new string('-', 40));
            lines.AppendLine($"Total: {total:C}");
            lines.AppendLine("Thank you for your purchase!");
            return lines.ToString();
        }

        public void PrintReceipt(string receiptText)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RetailPOS_Receipt.txt");
            File.WriteAllText(path, receiptText);
        }
    }
}
