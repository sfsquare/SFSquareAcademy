using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Text;
using System.Data.Entity.Validation;
using SFSAcademy.Helpers;

namespace SFSAcademy
{
    public enum Role
    {
        [Display(Name = "Admin")]
        Admin,
        [Display(Name = "Student")]
        Student,
        [Display(Name = "Employee")]
        Employee,
        [Display(Name = "Parent")]
        Parent
    }
    public class UserDetails
    {
        public USER User { get; set; }
        public List<PRIVILEGE> privilage_list { get; set; }
        public List<USERS_ACCESS> useraccess { get; set; }
    }
    public class User : IValidatableObject
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        [Remote("IsUserExists", "User", ErrorMessage = "User Name already in use.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Must be between 1 to 20 characters long.")]
        [RegularExpression("^[a-zA-Z0-9_-]{1,20}$", ErrorMessage = "Must contain only letters, numbers, hyphen, and  underscores.")]
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        public int? Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Must be between 4 to 40 characters long.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string LoginErrorMessage { get; set; }

        [Required]
        public string Role { get; set; }


        public string UserType(int? UserId)
        {
            if(UserId != null)
            {
                USER CurrentUser = db.USERS.Find(UserId);
                if (CurrentUser.ADMIN_IND.Equals(true))
                {
                    return "Admin";
                }
                else if (CurrentUser.EMP_IND.Equals(true))
                {
                    var CurrentPrivilege = db.PRIVILEGES.Include(x => x.PRIVILEGES_USERS).Where(x => x.ID == UserId).ToList();
                    /*var CurrentPrivilege = (from prusr in db.PRIVILEGES_USERS
                                            join pr in db.PRIVILEGES on prusr.PRIVILEGE_ID equals pr.ID
                                            where prusr.USER_ID == UserId
                                            select pr).ToList();*/
                    foreach (var item in CurrentPrivilege)
                    {
                        if (item.NAME.Contains("HR"))
                        {
                            return "HR";
                        }
                        else if (item.PRIVILEGE_TAG.Contains("Finance"))
                        {
                            return "Finance";
                        }
                        else if (item.NAME.Contains("Inventory"))
                        {
                            return "Inventory";
                        }
                    }
                    return "Empoyee";
                }
                else if (CurrentUser.STDNT_IND.Equals(true))
                {
                    return "Student";
                }
                else if (CurrentUser.PARNT_IND.Equals(true))
                {
                    return "Parent";
                }
                else
                {
                    return "Visitor";
                }
            }
            else
            {
                return "User Type canot be determined as UserId is null";
            }           
        }

        public string UserType()
        {
            HttpContext context = HttpContext.Current;
            int UserId = Convert.ToInt32(context.Session["UserId"]);

            USER CurrentUser = db.USERS.Find(UserId);
            if (CurrentUser.ADMIN_IND.Equals(true))
            {
                return "Admin";
            }
            else if (CurrentUser.EMP_IND.Equals(true))
            {
                var CurrentPrivilege = db.PRIVILEGES.Include(x => x.PRIVILEGE_TAG).Include(x => x.PRIVILEGES_USERS).Where(x => x.ID == UserId).ToList();
                /*var CurrentPrivilege = (from prusr in db.PRIVILEGES_USERS
                                        join pr in db.PRIVILEGES on prusr.PRIVILEGE_ID equals pr.ID
                                        join pt in db.PRIVILEGE_TAG on pr.PRIVILEGE_TAG_ID equals pt.ID
                                        where prusr.USER_ID == UserId
                                        select pr).ToList();*/
                foreach (var item in CurrentPrivilege)
                {
                    if (item.NAME.Contains("HR"))
                    {
                        return "HR";
                    }
                    else if (item.PRIVILEGE_TAG.Contains("Finance"))
                    {
                        return "Finance";
                    }
                    else if (item.NAME.Contains("Inventory"))
                    {
                        return "Inventory";
                    }
                }
                return "Empoyee";
            }
            else if (CurrentUser.STDNT_IND.Equals(true))
            {
                return "Student";
            }
            else if (CurrentUser.PARNT_IND.Equals(true))
            {
                return "Parent";
            }
            else
            {
                return "Visitor";
            }

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == null)
            {
                //yield return new ValidationResult($"Classic movies must have a release year earlier than {_classicYear}.", new[] { "ReleaseDate" });
                yield return new ValidationResult($"Role must be provided for the new User.");
            }
        }

    }

    public class SelectUserAccess
    {
        public USERS_ACCESS AccessList { get; set; }
        public int? PRIVILEGE_ID { get; set; }
        public int? USRS_ID { get; set; }
        public bool Selected { get; set; }
    }

    public class UserAccessController
    {
        public string AccessListController { get; set; }
        public int? PRIVILEGE_ID { get; set; }
        public int? USRS_ID { get; set; }
        public bool Selected { get; set; }
    }

    public class SelectUserPrivilage
    {
        public PRIVILEGE PrivilageList { get; set; }
        public int? USRS_ID { get; set; }
        public bool IsActive { get; set; }
        public bool Selected { get; set; }
    }

    public class Privilege_Tags
    {
        public PRIVILEGE_TAG Privilege_Tag { get; set; }
        public int? USRS_ID { get; set; }
        public bool IsActive { get; set; }
        public bool Selected { get; set; }
    }

    public class UserMetadata
    {
        [Required]
        public string USRNAME { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string HASHED_PSWRD { get; set; }
    }

    [MetadataType(typeof(UserMetadata))]
    public partial class USER : IValidatableObject, IHasTimeStamp, IHasBeforeSave
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        private string LoginErrorMessage { get; set; }
        public IEnumerable<BATCH> Active()
        {
            var ActiveUsers = db.USERS.Include(x => x.STUDENTs).Include(x => x.EMPLOYEEs).Where(x => x.IS_DEL == false).OrderBy(x => x.USRNAME);
            return (IEnumerable<BATCH>)ActiveUsers;
        }

        public IEnumerable<BATCH> Inactive()
        {
            var ActiveUsers = db.USERS.Include(x => x.STUDENTs).Include(x => x.EMPLOYEEs).Where(x => x.IS_DEL == true).OrderBy(x => x.USRNAME);
            return (IEnumerable<BATCH>)ActiveUsers;
        }

        public string Full_Name
        {
            get { return FIRST_NAME + " " + LAST_NAME; }
        }

        private string Random_String(int SALTLength)
        {
            string allowedChars = "";
            allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "1,2,3,4,5,6,7,8,9,0,!,@,#,$,%,&,?";
            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);
            string SALT = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < SALTLength; i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                SALT += temp;
            }
            return SALT;
        }

        public void DoTimeStamp(string EntityStateVal)
        {

            if (EntityStateVal.Equals("Added"))
            {
                //add creation date_time            
                CREATED_AT = DateTime.Now;
                UPDATED_AT = DateTime.Now;
            }

            if (EntityStateVal.Equals("Modified"))
            {
                //update Updation time            
                UPDATED_AT = DateTime.Now;
            }
        }

        public void Before_Save()
        {
            if(SALT == null)
            {
                SALT = Random_String(8);
            }
            if (HASHED_PSWRD == null)
            {
                HASHED_PSWRD = SALT;
                IS_FIRST_LOGIN = true;
            }
            else
            {
                IS_FIRST_LOGIN = false;
            }
            ADMIN_IND = false;
            STDNT_IND = false;
            EMP_IND = false;
            PARNT_IND = false;
            switch (ROLE)
            {
                case "Admin":
                    ADMIN_IND = true;
                    break;
                case "Student":
                    STDNT_IND = true;
                    break;
                case "Employee":
                    EMP_IND = true;
                    break;
                case "Parent":
                    PARNT_IND = true;
                    break;
                default:  // Name ascending 
                    STDNT_IND = true;
                    break;
            }
        }

        public int Check_Reminders()
        {
            int Count = 0;
            //int UserId = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
            var EventReminder = (from EV in db.EVENTs
                                 join EDE in db.EMPLOYEE_DEPARTMENT_EVENT on EV.ID equals EDE.EV_ID
                                 join ED in db.EMPLOYEE_DEPARTMENT on EDE.EMP_DEPT_ID equals ED.ID
                                 join EP in db.EMPLOYEEs on ED.ID equals EP.EMP_DEPT_ID
                                 where EP.USRID == ID && EV.IS_DUE == true 
                                 select new { EVENT_ID = EV.ID }).ToList();

            foreach (var entity in EventReminder.ToList())
            {
                Count = Count + 1;
            }
            return Count;
        }

        public string Role_Name()
        {
            switch (ROLE)
            {
                case "Admin":
                    return "Admin";
                case "Student":
                    return "Student";
                case "Employee":
                    return "Employee";
                case "Parent":
                    return "Parent";
                default:  // Name ascending 
                    return null;
            }
        }

        public void Soft_Delete()
        {
            IS_DEL = true;
        }


        public bool IsValid(string _username, string _password)
        {

            if (_username == null)
            {
                return false;
            }
            else if (_password == null)
            {
                return false;
            }

            try
            {
                var v = db.USERS.Where(a => a.USRNAME.Equals(_username) && a.HASHED_PSWRD.Equals(_password)).FirstOrDefault();

                if (v != null)
                {
                    int value = Convert.ToInt32(v.ID);
                    System.Web.HttpContext.Current.Session["UserId"] = value;
                    if (v.STDNT_IND == true)
                    {
                        STUDENT std = db.STUDENTs.Where(x => x.USRID == v.ID).FirstOrDefault();
                        System.Web.HttpContext.Current.Session["StudentId"] = std.ID;
                    }
                    if (v.PARNT_IND == true)
                    {
                        GUARDIAN grd = db.GUARDIANs.Where(x => x.USRID == v.ID).FirstOrDefault();
                        STUDENT std = db.STUDENTs.Find(grd.WARD_ID);
                        System.Web.HttpContext.Current.Session["StudentId"] = std.ID;
                    }
                    if (v.EMP_IND == true)
                    {
                        EMPLOYEE emp = db.EMPLOYEEs.Where(x => x.USRID == v.ID).FirstOrDefault();
                        System.Web.HttpContext.Current.Session["EmployeeId"] = emp.ID;
                    }

                    //To Do... Needs tovalidate the user access rights
                    //var useraccess = db.USERS_ACCESS.Where(a => a.USRS_ID == v.ID).ToList();
                    //
                    //foreach (var access in useraccess)
                    //{
                    //    userrights.Add(new USERS_ACCESS)
                    //}
                    UserDetails CurrentUserDetails = new UserDetails();
                    CurrentUserDetails.User = v;
                    CurrentUserDetails.privilage_list = GetUserPrivilage(Convert.ToInt32(v.ID));
                    //user.privilage = GetUserPrivilage(Convert.ToInt32(v.ID));
                    System.Web.HttpContext.Current.Session["CurrentUser"] = CurrentUserDetails;
                    //userrights.user = v;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (DbEntityValidationException e)
            {
                string ErrorMessage = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, " | ", ve.ErrorMessage);
                    }
                }
                LoginErrorMessage = ErrorMessage;
                return false;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    LoginErrorMessage = e.InnerException.InnerException.Message.ToString();
                }
                if (!string.IsNullOrEmpty(e.Message.ToString()))
                {
                    LoginErrorMessage = e.Message.ToString();
                }
                return false;
            }

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(LoginErrorMessage))
            {
                //yield return new ValidationResult($"Classic movies must have a release year earlier than {_classicYear}.", new[] { "ReleaseDate" });
                yield return new ValidationResult($"*", new[] { LoginErrorMessage });
            }
        }

        public List<PRIVILEGE> GetUserPrivilage(int UserId)
        {
            var userprevilege = (from userprevl in db.PRIVILEGES_USERS
                                 join prev in db.PRIVILEGES on userprevl.PRIVILEGE_ID equals prev.ID
                                 where userprevl.USER_ID == UserId
                                 orderby userprevl.PRIVILEGE_ID
                                 select prev).ToList();

            return userprevilege;
        }
    }

}