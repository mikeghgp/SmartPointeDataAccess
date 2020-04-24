using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPointe.DataAccess.Models
{
    public class Property
    {

        [Required]
        public int MLSNumber { get; set; }

        [StringLength(1)]
        [Column(TypeName = "varchar(1)")]
        public string MediaDescription { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string City { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string CountyOrParish { get; set; }


        [StringLength(10)]
        [Column(TypeName = "nvarchar(10)")]
        public string PostalCode { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string StreetName { get; set; }

        [StringLength(25)]
        [Column(TypeName = "nvarchar(25)")]
        public string StreetNumber { get; set; }

        [StringLength(32)]
        [Column(TypeName = "nvarchar(32)")]
        public string StreetSuffix { get; set; }


    }
}
