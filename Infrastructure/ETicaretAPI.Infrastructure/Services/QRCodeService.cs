using ETicaretAPI.Application.Abstraction.Services;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class QRCodeService : IQRCodeService
    {
        public byte[] CreateQRCode(string text)
        {
            QRCodeGenerator codeGenerator = new();

            QRCodeData data = codeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new(data);
            byte[] byteGraphic=qrCode.GetGraphic(10, new byte[] { 95, 158, 160 }, new byte[] { 240, 240, 240 });
            return byteGraphic;
        }
    }
}
