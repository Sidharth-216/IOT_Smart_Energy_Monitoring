/*using System;
using System.ComponentModel.DataAnnotations;

namespace SmartEnergy.Web.Models
{
    public class EnergyReading
    {
        [Key]
        public int ReadingId { get; set; }

        public int DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public Device? Device { get; set; }
        public float Voltage { get; set; }
        public float Current { get; set; }
        public float Power { get; set; }
        public DateTime Timestamp { get; set; }
    }
}*/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEnergy.Web.Models
{
    public class EnergyReading
    {
        [Key]
        public int ReadingId { get; set; }

        public int DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public Device? Device { get; set; }

        public float Voltage { get; set; }
        public float Current { get; set; }
        public float Power { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

