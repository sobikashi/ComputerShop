//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComputerShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class ComputerItem
    {
        public int Id { get; set; }
        public int ComputerId { get; set; }
        public int ComponentId { get; set; }
    
        public virtual Computer Computer { get; set; }
        public virtual Component Component { get; set; }
    }
}