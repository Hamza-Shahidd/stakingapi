//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace stakingapi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class stk_users
    {
        public int id { get; set; }
        public string metamask { get; set; }
        public Nullable<double> zin_in_wallet { get; set; }
        public Nullable<double> staked_zin { get; set; }
        public Nullable<double> reward_percnetage { get; set; }
        public Nullable<double> total_stk_percentage { get; set; }
        public Nullable<double> reward_per_frequency { get; set; }
        public Nullable<double> total_reward { get; set; }
        public Nullable<System.DateTime> timestamp { get; set; }
    }
}
