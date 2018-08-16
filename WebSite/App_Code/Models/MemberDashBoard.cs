using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberDashBoardModel : BusinessRulesObjectModel
    {
        
        public MemberDashBoardModel()
        {
        }
        
        public MemberDashBoardModel(BusinessRules r) : 
                base(r)
        {
        }
    }
}
