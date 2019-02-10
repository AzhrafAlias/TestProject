using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySpritzer.Models
{
    public class PickRawMtrlModel
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string JobNum { get; set; }
        public string Plant { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddThh:mm:ss}")]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }
        public string PartNum { get; set; }
        public string PartDesc { get; set; }
        public string LotNo { get; set; }
        public string TuId { get; set; }
        public decimal ActQty { get; set; }
        public decimal Qty { get; set; }
        public string UOM { get; set; }
        public decimal PickQty { get; set; }
        public string PickUOM { get; set; }
        [Display(Name = "FromWH")]
        public string FromWarehse { get; set; }
        [Display(Name = "FromBin")]
        public string FromBinNum { get; set; }
        [Display(Name = "ToWH")]
        public string ToWarehse { get; set; }
        [Display(Name = "ToBin")]
        public string ToBinNum { get; set; }
        public string PalletType { get; set; }
        public decimal PalletQty { get; set; }
        public string UserId { get; set; }
        //Input data for PalletType
        public List<PalletTypeModel> PalletTypeInfo { get; set; }
        public Byte[] SysRevID { get; set; }
        public Guid SysRowId { get; set; }

        public bool DynamicPick { get; set; }
        public decimal ConvFactor { get; set; }

        public bool isFinish { get; set; }

    }
}