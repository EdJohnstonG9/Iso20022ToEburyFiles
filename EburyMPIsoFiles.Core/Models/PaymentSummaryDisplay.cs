using EburyMPIsoFilesLibrary.Models.Ebury;
using System;
using System.Collections.Generic;
using System.Text;

namespace EburyMPIsoFiles.Core.Models
{

    public class PaymentSummaryDisplay
    {
        public string ItemKey { get; set; }
        public int Payments { get; set; }
        public float Amount { get; set; }
        public int HavingBene { get; set; }
        public float HavingBeneAmt { get; set; }
        public int HavingTrade { get; set; }
        public float HavingTradeAmt { get; set; }
        public int HavingPayment { get; set; }
        public float HavingPaymentAnt { get; set; }
        public PaymentSummaryDisplay(string key)
        {
            ItemKey = key;
        }
    }

    public class PaymentShortSummaryDisplay
    {
        public string ItemKey { get; set; }
        public int Payments { get; set; }
        public float Amount { get; set; }
        public PaymentShortSummaryDisplay(string key)
        {
            ItemKey = key;
        }
        private static string summaryKey(MassPaymentFileModel payment)
        {
            return payment.PaymentCurrency + payment.SettlementCurrency;
        }
        static string Total = "Total";
        public static List<PaymentShortSummaryDisplay> GetShortSummary(List<MassPaymentFileModel> payments)
        {
            List<PaymentShortSummaryDisplay> outList = new List<PaymentShortSummaryDisplay>();
            var itemTot = new PaymentShortSummaryDisplay(Total);
            outList.Add(itemTot);
            foreach(var payment in payments)
            {
                var item = outList.Find(x => x.ItemKey == summaryKey(payment));
                if (item == null)
                {
                    item = new PaymentShortSummaryDisplay(summaryKey(payment));
                    outList.Add(item);
                }
                item.Amount += (float)Math.Round(payment.PaymentAmount, 2);
                item.Payments += 1;
                itemTot.Amount += (float)Math.Round(payment.PaymentAmount, 2);
                itemTot.Payments += 1;
            }
            return outList;
        }
    }
}
