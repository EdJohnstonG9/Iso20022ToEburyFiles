using EburyMPIsoFilesLibrary.Models.Bacs;
using EburyMPIsoFilesLibrary.Models.Ebury;
using System;
using System.Collections.Generic;
using System.Text;

namespace EburyMPIsoFilesLibrary.Services
{
    public class BacsPaymentFile : IInputPaymentFile<BacsModel>
    {
        public List<MassPaymentFileModel> MassPaymentFileList()
        {
            throw new NotImplementedException();
        }

        public int ReadPaymentsFile(string fullPathName)
        {
            throw new NotImplementedException();
        }
    }
}
