namespace RetailPOS.Application.Interfaces
{
    public interface IReceiptPrinter
    {
        string GenerateReceipt(string? customerName, System.Collections.Generic.List<(string productName, int quantity, decimal unitPrice, decimal lineTotal)> items, decimal total);
        void PrintReceipt(string receiptText);
    }
}
