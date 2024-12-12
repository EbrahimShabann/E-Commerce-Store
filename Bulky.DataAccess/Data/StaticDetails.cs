using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Data
{
	public static class StaticDetails
	{
		//Roles of Users
		public const string Customer_Role = "Customer";
		public const string Company_Role = "Company";
		public const string Employee_Role = "Employee";
		public const string Admin_Role = "Admin";


		//Status of Shipping
		public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";
		public const string StatusInProcess = "Processing";
		public const string StatusShipped = "Shipped";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";

		//Status of Payment

		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
		public const string PaymentStatusRejected = "Rejected";


		//to use session in yout project u need a keyword with keyvalue for the session

		public const string SessionCart = "SessionShoppingCart";

    }
}
