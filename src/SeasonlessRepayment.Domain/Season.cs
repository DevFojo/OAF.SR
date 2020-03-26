using System;
using System.Collections.Generic;

namespace SeasonlessRepayment.Domain
{
    public class Season : BaseEntity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
    }
}