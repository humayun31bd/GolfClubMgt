using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;

namespace MyCompany.Rules
{
	public partial class DueCollectionPaymentBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("DueCollectionPayment", RowKind.New)]
        public void BuildNewDueCollectionPayment()
        {
            UpdateFieldValue("PayTypeID", 1);
        }
    }
}
