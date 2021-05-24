using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EburyMPIsoFilesLibrary.Models.Airswift
{
    public class AirswiftPaymentModel
    {
        public AirswiftModel BatHdr { get; set; }
        public AirswiftModel SecPty { get; set; }
        public AirswiftModel Adv { get; set; }
    }

    public class AirswiftModel
    {
        public string RecType { get; set; }
        public string Account { get; set; }
        public string BeneName { get; set; }
        public string ColD { get; set; }
        public string IfhRef { get; set; }
        public string IfhCreated { get; set; }
        public string IfhDate { get; set; }
        public string Amount { get; set; }
        public string DueDate { get; set; }
        public string ColJ { get; set; }
        public string BatDueDate { get; set; }
        public string BatCustomerRef { get; set; }
        public string BatCcy { get; set; }
        public string BatAmount { get; set; }
        public string ColO { get; set; }
        public string ColP { get; set; }
        public string ColQ { get; set; }
        public string ColR { get; set; }
        public int? ColS { get; set; }
        public string AdvBeneEmail { get; set; }
        public string OrderingCustomer { get; set; }
        public string OrderingAddress1 { get; set; }
        public string OrderingAddress2 { get; set; }
        public string ColX { get; set; }
        public string ColY { get; set; }
        public string ColZ { get; set; }
        public string BatBeneAddress { get; set; }
        public string ColAB { get; set; }
        public string ColAC { get; set; }
        public string ColAD { get; set; }
        public string ColAE { get; set; }
        public string ColAF { get; set; }
        public string ColAG { get; set; }
        public string ColAH { get; set; }
        public string BankType { get; set; }
        public string SecBeneSwift { get; set; }
        public string SecBeneBank { get; set; }
        public string ColAL { get; set; }
        public string ColAM { get; set; }
        public string ColAN { get; set; }
        public string ColAO { get; set; }
        public string SecBankCtry { get; set; }
        public string ColAQ { get; set; }
        public string SecPaymentRef { get; set; }
        public string SecBeneBane2 { get; set; }
        public string ColAT { get; set; }
        public string ColAU { get; set; }
        public string Charges { get; set; }
    }


    //    public class ModelClassMap : ClassMap<AirswiftModel>
    //    {
    //        public ModelClassMap()
    //        {
    //            Map(m => m.RecType).Name("RecType");
    //            Map(m => m.Account).Name("Account");
    //            Map(m => m.BeneName).Name("BeneName");
    //            Map(m => m.Filler1).Name("Filler1");
    //            Map(m => m.IfhRef).Name("IfhRef");
    //            Map(m => m.IfhCreated).Name("IfhCreated");
    //            Map(m => m.IfhDate).Name("IfhDate");
    //            Map(m => m.Amount).Name("Amount");
    //            Map(m => m.DueDate).Name("DueDate");
    //            Map(m => m.Filler2).Name("Filler2");
    //            Map(m => m.BatDueDate).Name("BatDueDate");
    //            Map(m => m.BatCustomerRef).Name("BatCustomerRef");
    //            Map(m => m.BatCcy).Name("BatCcy");
    //            Map(m => m.BatAmount).Name("BatAmount");
    //            Map(m => m.ColO).Name("ColO");
    //            Map(m => m.ColP).Name("ColP");
    //            Map(m => m.ColQ).Name("ColQ");
    //            Map(m => m.ColR).Name("ColR");
    //            Map(m => m.ColS).Name("ColS");
    //            Map(m => m.AdvBeneEmail).Name("AdvBeneEmail");
    //            Map(m => m.OrderingCustomer).Name("OrderingCustomer");
    //            Map(m => m.OrderingAddress1).Name("OrderingAddress1");
    //            Map(m => m.OrderingAddress2).Name("OrderingAddress2");
    //            Map(m => m.ColX).Name("ColX");
    //            Map(m => m.ColY).Name("ColY");
    //            Map(m => m.ColZ).Name("ColZ");
    //            Map(m => m.BatBeneAddress).Name("BatBeneAddress");
    //            Map(m => m.ColAB).Name("ColAB");
    //            Map(m => m.ColAC).Name("ColAC");
    //            Map(m => m.ColAD).Name("ColAD");
    //            Map(m => m.ColAE).Name("ColAE");
    //            Map(m => m.ColAF).Name("ColAF");
    //            Map(m => m.ColAG).Name("ColAG");
    //            Map(m => m.ColAH).Name("ColAH");
    //            Map(m => m.BankType).Name("BankType");
    //            Map(m => m.SecBeneSwift).Name("SecBeneSwift");
    //            Map(m => m.SecBeneBank).Name("SecBeneBank");
    //            Map(m => m.ColAL).Name("ColAL");
    //            Map(m => m.ColAM).Name("ColAM");
    //            Map(m => m.ColAN).Name("ColAN");
    //            Map(m => m.ColAO).Name("ColAO");
    //            Map(m => m.SecBankCtry).Name("SecBankCtry");
    //            Map(m => m.ColAQ).Name("ColAQ");
    //            Map(m => m.SecPaymentRef).Name("SecPaymentRef");
    //            Map(m => m.SecBeneBane2).Name("SecBeneBane2");
    //            Map(m => m.ColAT).Name("ColAT");
    //            Map(m => m.ColAU).Name("ColAU");
    //            Map(m => m.Charges).Name("Charges");
    //        }
    //}
}
