using EburyMPIsoFilesLibrary.Models.Airswift;
using EburyMPIsoFilesLibrary.Models.Ebury;
using System.Collections.Generic;

namespace EburyMPIsoFilesLibrary.Services
{
    public interface IInputPaymentFile<T>
    {
        //List<T> InputPaymentList { get; set; }
        //T InputPayment { get; set; }

        List<MassPaymentFileModel> MassPaymentFileList();
        int ReadPaymentsFile(string fullPathName);
    }
}