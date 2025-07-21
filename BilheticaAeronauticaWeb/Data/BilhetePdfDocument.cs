using BilheticaAeronauticaWeb.Models;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Globalization;
using System.IO;

namespace BilheticaAeronauticaWeb.Data
{
    public class BilhetePdfDocument : IDocument
    {
        private readonly BilheteViewModel _bilhete;
        private readonly byte[] _logoBytes;

        public BilhetePdfDocument(BilheteViewModel bilhete)
        {
            _bilhete = bilhete;
            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logo/logo.png");
            _logoBytes = File.Exists(logoPath) ? File.ReadAllBytes(logoPath) : null;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var qrBytes = GenerateQrCodeBytes(_bilhete);

            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A5);

                // Header
                page.Header().Row(row =>
                {
                    row.ConstantItem(60).Element(e =>
                    {
                        if (_logoBytes != null && _logoBytes.Length > 0)
                            e.Image(_logoBytes).FitArea();

                    });
                    row.RelativeItem().Text("Bilhete de Voo")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();
                });


                page.Content().Column(col =>
                {
                    col.Item().Element(e => e.LineHorizontal(1).LineColor(Colors.Blue.Medium));
                    col.Item().Height(10);

                    col.Item().Background(Colors.Grey.Lighten3).Padding(8)
                        .Text("Dados da Reserva").Bold();
                    col.Item().Height(8);

                    AddRow(col, "Nº Bilhete:", _bilhete.Id.ToString());
                    AddRow(col, "Passageiro:", _bilhete.PassageiroNome);
                    AddRow(col, "Voo:", $"{_bilhete.VooNumero} ({_bilhete.OrigemNome} → {_bilhete.DestinoNome})");
                    AddRow(col, "Data:", _bilhete.DataPartida.ToString("dd/MM/yyyy HH:mm"));
                    AddRow(col, "Lugar:", _bilhete.LugarCodigo);
                    AddRow(col, "Preço:", _bilhete.Valor.ToString("C", CultureInfo.CurrentCulture));

                    string extras = "";
                    if (_bilhete.BagagemExtra)
                        extras += "Bagagem ";
                    if (_bilhete.Refeicao)
                        extras += "Refeição ";
                    if (string.IsNullOrWhiteSpace(extras))
                        extras = "Sem extras";
                    AddRow(col, "Extras:", extras);

                    string estado = _bilhete.WasDeleted ? "Anulado" : "Ativo";
                    string corEstado = _bilhete.WasDeleted ? Colors.Red.Medium : Colors.Green.Medium;
                    col.Item().Text($"Estado: {estado}")
                        .FontSize(13)
                        .Bold()
                        .FontColor(corEstado);
                    col.Item().Height(14);

                    if (qrBytes != null && qrBytes.Length > 0)
                    {
                        col.Item().AlignCenter().Element(e =>
                        e.Height(80).Image(qrBytes).FitHeight());
                    }

                    col.Item().Height(10);

                    col.Item().Text($"Emitido: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10)
                        .Italic()
                        .AlignRight();

                });
            });
        }

        private static void AddRow(ColumnDescriptor col, string label, string value)
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Text(label).Bold();
                row.RelativeItem().Text(value);
            });
        }

        private static byte[] GenerateQrCodeBytes(BilheteViewModel bilhete)
        {
            string data = $"ID:{bilhete.Id};Passageiro:{bilhete.PassageiroNome};Voo:{bilhete.VooNumero};Lugar:{bilhete.LugarCodigo};Data:{bilhete.DataPartida:dd-MM-yyyy}";
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q))
            {
                var bitmapByteQrCode = new BitmapByteQRCode(qrCodeData);
                return bitmapByteQrCode.GetGraphic(20);
            }

        }
    }
}
