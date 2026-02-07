using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESSMobile.Shared
{
    public class jsonRequest_Login
    {
        public string dbPath { get; set; }
        public string dbName { get; set; }
        public string dbUser { get; set; }
        public string dbPass { get; set; }
        public string Token { get; set; }
        public string userID { get; set; }
        public string userPassword { get; set; }
    }
    public class jsonRequest_Identifier
    {
        public string IdentifierID { get; set; }
    }
    public class jsonRequest_SubmitClocking
    {
        public string dbPath { get; set; }
        public string dbName { get; set; }
        public string dbUser { get; set; }
        public string dbPass { get; set; }
        public string Token { get; set; }
        public string userID { get; set; }
        public string userPassword { get; set; }
        public string empName { get; set; }
        public string DateD { get; set; }
        public string DateM { get; set; }
        public string DateY { get; set; }
        public string TimeH { get; set; }
        public string TimeM { get; set; }
        public string TimeS { get; set; }
        public string GeoLa { get; set; }
        public string GeoLo { get; set; }
        public string GeoAd { get; set; }
        public string Img64 { get; set; }
    }


    //Server Date Time
    public class ESS_ServerDateTime
    {
        [JsonProperty("serverUTC")]
        public string ServerUTC { get; set; }
        [JsonProperty("serverLocalTime")]
        public string ServerLocalTime { get; set; }
    }
    public class ESS_ClockTime
    {
        [JsonProperty("doubleClockIntervalTime")]
        public string DoubleClockIntervalTime { get; set; }
    }

    public class ESS_Approver
    {
        [JsonProperty("empid")]
        public string Empid { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("department")]
        public string Department { get; set; }
    }

    public class ESS_PushParameter
    {
        [JsonProperty("empid")]
        public string Empid { get; set; }
    }

    public class ESS_Root
    {
        [JsonProperty("master")]
        public ESS_Master ESS_Master { get; set; }

        [JsonProperty("serverDateTime")]
        public List<ESS_ServerDateTime> ESS_ServerDateTime { get; set; }

        [JsonProperty("clockTime")]
        public List<ESS_ClockTime> ESS_ClockTime { get; set; }

        [JsonProperty("approver")]
        public List<ESS_Approver> ESS_Approver { get; set; }

        [JsonProperty("checkToken")]
        public List<ESS_CheckToken> ESS_CheckToken { get; set; }

        [JsonProperty("identifier")]
        public List<ESS_Identifier> Identifier { get; set; }

        [JsonProperty("staffInfos")]
        public List<ESS_StaffInfo> ESS_StaffInfos { get; set; }

        [JsonProperty("records")]
        public List<ESS_Record> ESS_Records { get; set; }

        [JsonProperty("ttrans_records")]
        public List<ESS_Ttrans> ESS_TtransRecords { get; set; }

        [JsonProperty("leave_records")]
        public List<ESS_Leave> ESS_LeaveRecords { get; set; }

        [JsonProperty("masCodeList")]
        public List<ESS_MasCode> ESS_MasCodeList { get; set; }

        /**/
        [JsonProperty("lEntitlementList")]
        public List<ESS_LEntitlement> ESS_LEntitlementList { get; set; }

        [JsonProperty("lEmpLeaveGnt1List")]
        public List<ESS_LEmpLeaveGnt1> ESS_LEmpLeaveGnt1List { get; set; }

        [JsonProperty("earnBalanceList")]
        public List<ESS_EarnBalance> ESS_EarnBalanceList { get; set; }

        [JsonProperty("leaveEarnCalculations")]
        public List<ESS_LeaveEarnCalculation> ESS_LeaveEarnCalculations { get; set; }
        /**/

        [JsonProperty("replacementFunc")]
        public List<ESS_ReplacementFunc> ESS_ReplacementFunc { get; set; }

        [JsonProperty("leaveEmployment")]
        public List<ESS_LeaveEmployment> ESS_LeaveEmployment { get; set; }

        //[JsonProperty("cancelLeave")]
        //public List<ESS_CancelLeave> ESS_CancelLeave { get; set; }

        [JsonProperty("deleteLeave")]
        public List<ESS_DeleteLeave> ESS_DeleteLeave { get; set; }

        [JsonProperty("activityLog")]
        public List<ESS_ActivityLog> ActivityLog { get; set; }

        [JsonProperty("changePassword")]
        public List<ESS_ChangePassword> ESS_ChangePassword { get; set; }

        /**/
        public List<ESS_PushNotification> PushNotification { get; set; }
        public List<ESS_DeviceTokenList> DeviceTokenList { get; set; }
        public List<ESS_PushNotificationDeviceTokenList> PushNotificationDeviceTokenList { get; set; }

        /**/

        [JsonProperty("companyInfo")]
        public List<ESS_CompanyInfo> CompanyInfo { get; set; }

    }

    /**/
    public class ESS_Root2
    {
        [JsonProperty("master2")]
        public ESS_Master2 ESS_Master2 { get; set; }

        [JsonProperty("earnBalanceList")]
        public List<ESS_EarnBalance> ESS_EarnBalanceList { get; set; }

        [JsonProperty("leaveCancelRequestList")]
        public List<ESS_LeaveCancelRequest> ESS_LeaveCancelRequestList { get; set; }

        [JsonProperty("replacementFuncList")]
        public List<ESS_ReplacementFunc> ESS_ReplacementFuncList { get; set; }

    }

    /**/
    public class ESS_Master
    {
        [JsonProperty("userid")]
        public string Userid { get; set; }

        [JsonProperty("useremail")]
        public string Useremail { get; set; }

        [JsonProperty("usericno")]
        public string Usericno { get; set; }

        /**/
        [JsonProperty("empName")]
        public string EmpName { get; set; }
        /**/

        [JsonProperty("resultcode")]
        public string Resultcode { get; set; }

        [JsonProperty("detailmsg")]
        public string Detailmsg { get; set; }

        [JsonProperty("recordcnt")]
        public int Recordcnt { get; set; }
    }

    /**/
    public class ESS_Master2
    {
        [JsonProperty("userid")]
        public string Userid { get; set; }

        [JsonProperty("useremail")]
        public string Useremail { get; set; }

        [JsonProperty("resultcode")]
        public string Resultcode { get; set; }

        [JsonProperty("detailmsg")]
        public string Detailmsg { get; set; }

        [JsonProperty("recordcnt")]
        public int Recordcnt { get; set; }
    }

    /**/

    /**/
    public class ESS_CheckToken
    {
        public string Token { get; set; }
        public string ComID { get; set; }
    }

    public class ESS_Identifier
    {
        public string IdentifierID { get; set; }
        public string ConString { get; set; }
        public string Company { get; set; }
        public string LicenseID { get; set; }
        public string API { get; set; }
    }
    /**/
    public class ESS_StaffInfo
    {
        [JsonProperty("empid")]
        public string Empid { get; set; }

        [JsonProperty("empName")]
        public string EmpName { get; set; }

        [JsonProperty("joinDate")]
        public string JoinDate { get; set; }

        [JsonProperty("confirmDate")]
        public string ConfirmDate { get; set; }

        [JsonProperty("resignDate")]
        public string ResignDate { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("staffCount")]
        public string StaffCount { get; set; }

        //
        public string IsAdmin { get; set; }
        public string FilterDepartment { get; set; }
        public string ApproverID { get; set; }
        //

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("wg")]
        public string WG { get; set; }

        [JsonProperty("approver1")]
        public string Approver1 { get; set; }

        [JsonProperty("approver2")]
        public string Approver2 { get; set; }

        [JsonProperty("approver3")]
        public string Approver3 { get; set; }
    }

    public class ESS_Record
    {
        [JsonProperty("empid")]
        public string Empid { get; set; }

        [JsonProperty("empname")]
        public string Empname { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("icno")]
        public string ICNO { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("times")]
        public string Time { get; set; }

        [JsonProperty("clockid")]
        public string ClockID { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("latereason")]
        public string LateReason { get; set; }

        [JsonProperty("endorsed")]
        public string Endorsed { get; set; }

        [JsonProperty("shift")]
        public string Shift { get; set; }

        [JsonProperty("shiftdate")]
        public DateTime? ShiftDate { get; set; }

        [JsonProperty("shiftlock")]
        public string ShiftLock { get; set; }

    }

    public class ESS_Ttrans
    {
        [JsonProperty("empid")]
        public string Empid { get; set; }

        [JsonProperty("userid")]
        public string UserID { get; set; }

        [JsonProperty("dateInput")]
        public string DateInput { get; set; }

        [JsonProperty("date")]
        public DateTime/*string*/ Date { get; set; }

        [JsonProperty("times")]
        public string Time { get; set; }

        [JsonProperty("clockid")]
        public string ClockID { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("sms")]
        public string SMS { get; set; }

        [JsonProperty("latereason")]
        public string LateReason { get; set; }

        [JsonProperty("endorsed")]
        public string Endorsed { get; set; }

        [JsonProperty("shift")]
        public string Shift { get; set; }

        //
        [JsonProperty("shiftdateInput")]
        public string ShiftDateInput { get; set; }
        //

        [JsonProperty("shiftdate")]
        public DateTime? ShiftDate { get; set; }

        [JsonProperty("shiftlock")]
        public string ShiftLock { get; set; }

        [JsonProperty("workcode")]
        public string WorkCode { get; set; }

        [JsonProperty("temperature")]
        public string Temperature { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("attendanceimgdata")]
        public string AttendanceImgData { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        // For filter
        public string FilterYear { get; set; }
        public string FilterMonth { get; set; }
        public string FilterDate { get; set; }
        public string FilterDepartment { get; set; }

        // For filter

        public string IsAdmin { get; set; }
    }

    public class ESS_TtransGroup : List<ESS_Ttrans>
    {
        //[JsonProperty("date")]
        public DateTime Date { get; private set; }
        public ESS_TtransGroup(DateTime date, List<ESS_Ttrans> _ESS_Ttrans) : base(_ESS_Ttrans)
        {
            Date = date;
        }
    }

    public class ESS_Leave
    {
        [JsonProperty("empid")]
        public string Empid { get; set; }

        [JsonProperty("lv")]
        public string LV { get; set; }

        //
        [JsonProperty("dfromInput")]
        public string DfromInput { get; set; }
        [JsonProperty("dtoInput")]
        public string DtoInput { get; set; }
        //

        [JsonProperty("dfrom")]
        public DateTime Dfrom { get; set; }

        [JsonProperty("dto")]
        public DateTime Dto { get; set; }

        [JsonProperty("noofday")]
        public double Noofday { get; set; }

        [JsonProperty("halfday")]
        public string Halfday { get; set; }

        [JsonProperty("halftype")]
        public string Halftype { get; set; }

        [JsonProperty("entitlemt")]
        public string Entitlemt { get; set; }

        [JsonProperty("bf")]
        public string Bf { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("status")]

        public string Status { get; set; }

        //
        [JsonProperty("sdateInput")]
        public string SdateInput { get; set; }
        //

        [JsonProperty("sdate")]
        public DateTime? Sdate { get; set; }

        [JsonProperty("cdate")]
        public DateTime? Cdate { get; set; }

        [JsonProperty("img")]
        public string ImgData { get; set; }

        [JsonProperty("contlv")]
        public string Contlv { get; set; }

        [JsonProperty("endorsed")]
        public string Endorsed { get; set; }

        [JsonProperty("approver1")]
        public string Approver1 { get; set; }

        [JsonProperty("approver2")]
        public string Approver2 { get; set; }

        [JsonProperty("approver3")]
        public string Approver3 { get; set; }

        [JsonProperty("cancelreqstatus")]
        public string Cancelreqstatus { get; set; }

        [JsonProperty("cancelreq1")]
        public string Cancelreq1 { get; set; }

        [JsonProperty("cancelreq2")]
        public string Cancelreq2 { get; set; }

        [JsonProperty("cancelreq3")]
        public string Cancelreq3 { get; set; }

        [JsonProperty("attachmentimgdata")]
        public string AttachmentImgData { get; set; }

        [JsonProperty("alertMsg")]
        public string AlertMsg { get; set; }

        [JsonProperty("replacementOK")]
        public int ReplacementOK { get; set; }

        [JsonProperty("replacementRemark")]
        public string ReplacementRemark { get; set; }

        public string StatusAndCancelReqStatus => Status + "-" + Cancelreqstatus;

        // Filter
        //public DateTime? FilterDate { get; set; }
        public string FilterDate { get; set; }
        public string FilterMonth { get; set; }
        public string FilterYear { get; set; }
        public string FilterStatus { get; set; }
        public string FilterCancelReqStatus { get; set; }
        public string FilterLeaveType { get; set; }
        public string FilterDepartment { get; set; }
        public string FilterCompanyName { get; set; }

        // Filter

        // Replacement
        public string ReplacementReplaceBy { get; set; }
        public string ReplacementEarnBal { get; set; }
        // Replacement

        // Admin Leave 
        public string IsAdminLeave { get; set; }
        public int InputPageNum { get; set; }
        public int TotalPageNum { get; set; }
        public int TotalRow { get; set; }
        public string UsePagination { get; set; }
        public string WithPendingAndApprove { get; set; }
        public string WithPending { get; set; }
        public string DisplayPageNum => InputPageNum.ToString();
        // Admin Leave

        // Leave Approval
        public string ApproverID { get; set; }
        public string ApproverEmail { get; set; }
        public string ApproverAction { get; set; }
        public string ApproverDepartment { get; set; }
        public string ApprovalLevel { get; set; }
        public string EmpApprover1 { get; set; }
        public string EmpApprover2 { get; set; }
        public string EmpApprover3 { get; set; }
        // Leave Approval

        //
        public bool IsChecked { get; set; }
    }

    public class ESS_MasCode
    {
        [JsonProperty("ctype")]
        public string Ctype { get; set; }

        [JsonProperty("ccode")]
        public string Ccode { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        public string CcodeDesc => Ccode + " - " + Desc;

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("eeperc")]
        public string EEPerc { get; set; }

        [JsonProperty("eyperc")]
        public string EYPerc { get; set; }

        [JsonProperty("eeamt")]
        public string EEAMT { get; set; }

        [JsonProperty("eyamt")]
        public string EYAMT { get; set; }

        [JsonProperty("maxepf")]
        public string MAXEPF { get; set; }
    }

    public class ESS_LEntitlement
    {
        [JsonProperty("lv")]
        public string LV { get; set; }

        public string EType { get; set; }
        public string RoundingID { get; set; }
        public string Days { get; set; }
        public string Eused { get; set; }
        public string Method { get; set; }
        public string NeedApproval { get; set; }
        public int bf_Method { get; set; }
        public string bf_value { get; set; }
        public string Replacement { get; set; }
        public string ReplaceBy { get; set; }
        public string Overwritten { get; set; }
        public string Grouping { get; set; }
        public string GroupWith { get; set; }
        public string Employment { get; set; }
        public string Limited { get; set; }
        public string User { get; set; }
        public string PBMethod { get; set; }
        public string PBValue { get; set; }
        public string PBPaycode { get; set; }
        public string ContLV { get; set; }
        public string ContDay { get; set; }
        public string NonWork { get; set; }
        public string MultiApproval { get; set; }
        public string CustCutoff { get; set; }
        public string CustCutoffD { get; set; }
        public string CustCutoffM { get; set; }
        public string CutoffStartYear { get; set; }
        public string CutoffCalcBy { get; set; }
        public string YTDCalcBy { get; set; }
        public string DaysYTD { get; set; }
    }

    public class ESS_LEmpLeaveGnt1
    {
        [JsonProperty("empid")]
        public string Empid { get; set; }

        [JsonProperty("lv")]
        public string LV { get; set; }
        public string CurrentDate { get; set; }
        public string Day { get; set; }
        public string bf { get; set; }
        public string Date { get; set; }

        /**/
        public string dy { get; set; }
        public string bfv { get; set; }
        public string FullEnt_Input { get; set; }
        public string BF_Input { get; set; }
        public string EntBal_Input { get; set; }
        /**/

        // For Leave Earn Calculation Part
        /**/
        public string DD { get; set; }
        /**/

        /**/
        public string JoinDatePlaceholderToDisplay { get; set; }
        public string UserConfirmDatePlaceholderToHold { get; set; }
        public string ResignDatePlaceholderToHide { get; set; }
        /**/

        /**/

        public string MinDays { get; set; }
        public string MinDaysJoin { get; set; }
        public string MinDaysConfirm { get; set; }
        public string MinDaysResign { get; set; }
        public string MinDaysYTD { get; set; }
        /**/

        /**/
        public string YTD_Input { get; set; }
        /**/

        /**/
        public string YTDMthStart { get; set; }
        public string YTDMthEnd { get; set; }
        /**/
        // For Leave Earn Calculation Part


    }

    public class ESS_EarnBalance
    {
        // Input
        [JsonProperty("empid")]
        public string Empid { get; set; }
        [JsonProperty("lv")]
        public string LV { get; set; }

        //public string CurrentDate { get; set; }

        // Output
        //public string FullEnt { get; set; }
        //public string BF { get; set; }
        //public string EntBal { get; set; }
        //public string YTD { get; set; }
        //public string EarnDays { get; set; }
        //public string Taken1 { get; set; }
        //public string Taken2 { get; set; }
        //public string Adj { get; set; }
        //public string EarnBal { get; set; }

        // For Leave Earn Calculation Part
        public string Etype { get; set; }

        public string hideReplacement { get; set; }
        public string hideReplaceBy { get; set; }
        //public string Day { get; set; }
        //public string bf { get; set; }
        //public string Date { get; set; }

        ///**/
        //public string dy { get; set; }
        //public string bfv { get; set; }
        public string FullEnt_Input { get; set; }
        public string BF_Input { get; set; }
        public string EntBal_Input { get; set; }
        /**/

        ///**/
        //public string DD { get; set; }
        ///**/

        ///**/
        //public string DDfrom { get; set; }
        //public string DDto { get; set; }
        /**/

        ///**/
        //public string JoinDatePlaceholderToDisplay { get; set; }
        //public string UserConfirmDatePlaceholderToHold { get; set; }
        //public string ResignDatePlaceholderToHide { get; set; }
        ///**/

        ///**/

        //public string MinDays { get; set; }
        //public string MinDaysJoin { get; set; }
        //public string MinDaysConfirm { get; set; }
        //public string MinDaysResign { get; set; }
        //public string MinDaysYTD { get; set; }
        ///**/

        /**/
        //public string YTDMthCal { get; set; }
        //public string YTDMthTotal { get; set; }
        public string YTD_Input { get; set; }
        public string EarnDays_Input { get; set; }

        public string Taken1_Input { get; set; }
        public string Taken2_Input { get; set; }
        public string EarnBal_Input { get; set; }
        public string Adj_Input { get; set; }
        public string EntitleLabel { get; set; }
        /**/

        ///**/
        //public string YTDEndDays { get; set; }
        //public string YTDDayWorked { get; set; }
        //public string YTDMthToday { get; set; }
        //public string YTDMthStart { get; set; }
        //public string YTDMthEnd { get; set; }
        ///**/
        // For Leave Earn Calculation Part
    }

    public class ESS_LeaveEarnCalculation
    {

        // Input
        [JsonProperty("empid")]
        public string Empid { get; set; }
        [JsonProperty("lv")]
        public string LV { get; set; }

        public string CurrentDate { get; set; }

        // Output
        //public string FullEnt { get; set; }
        //public string BF { get; set; }
        //public string EntBal { get; set; }
        //public string YTD { get; set; }
        //public string EarnDays { get; set; }
        //public string Taken1 { get; set; }
        //public string Taken2 { get; set; }
        //public string Adj { get; set; }
        //public string EarnBal { get; set; }

        // For Leave Earn Calculation Part
        public string Etype { get; set; }
        public string Day { get; set; }
        public string bf { get; set; }
        public string Date { get; set; }

        /**/
        public string dy { get; set; }
        public string bfv { get; set; }
        public string FullEnt_Input { get; set; }
        public string BF_Input { get; set; }
        public string EntBal_Input { get; set; }
        /**/

        /**/
        public string DD { get; set; }
        /**/

        /**/
        public string JoinDatePlaceholderToDisplay { get; set; }
        public string UserConfirmDatePlaceholderToHold { get; set; }
        public string ResignDatePlaceholderToHide { get; set; }
        /**/

        /**/

        public string MinDays { get; set; }
        public string MinDaysJoin { get; set; }
        public string MinDaysConfirm { get; set; }
        public string MinDaysResign { get; set; }
        public string MinDaysYTD { get; set; }
        /**/

        /**/
        public string YTDMthCal { get; set; }
        public string YTDMthTotal { get; set; }
        public string YTD_Input { get; set; }
        public string EarnDays_Input { get; set; }

        public string Taken1_Input { get; set; }
        public string Taken2_Input { get; set; }
        public string EarnBal_Input { get; set; }
        public string Adj_Input { get; set; }
        public string EntitleLabel { get; set; }
        /**/

        /**/
        public string YTDEndDays { get; set; }
        public string YTDDayWorked { get; set; }
        public string YTDMthToday { get; set; }
        public string YTDMthStart { get; set; }
        public string YTDMthEnd { get; set; }
        /**/
        // For Leave Earn Calculation Part
    }

    /**/
    public class ESS_ReplacementFunc
    {
        [JsonProperty("numDays")]
        public string NumDays { get; set; }

        [JsonProperty("oriLV")]
        public string oriLV { get; set; }

        [JsonProperty("replaceLV")]
        public string replaceLV { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("attachmentImgData")]
        public string AttachmentImgData { get; set; }

        [JsonProperty("alertMsg")]
        public string AlertMsg { get; set; }

        /**/
        [JsonProperty("inpEarnBal")]
        public string inpEarnBal { get; set; }

        [JsonProperty("inpDateF")]
        public string inpDateF { get; set; }

        [JsonProperty("replacementOK")]
        public int ReplacementOK { get; set; }

        [JsonProperty("replacementRemark")]
        public string ReplacementRemark { get; set; }
    }

    public class ESS_LeaveEmployment
    {
        public string hideEmployment { get; set; }
        public string AlertMsg1 { get; set; }
    }

    //public class ESS_CancelLeave
    //{
    //    //public string Empid { get; set; }
    //    //public string LV { get; set; }
    //    //public DateTime dFrom { get; set; }
    //    //public DateTime dTo { get; set; }
    //    //public double NoofDay { get; set; }
    //    //public string HalfDay { get; set; }
    //    //public string HalfType { get; set; }
    //    //public DateTime? sDate { get; set; }
    //    //public string AlertMsg { get; set; }
    //    //public string Entitlemt { get; set; }

    //    //public string Bf { get; set; }

    //    //public string Remark { get; set; }

    //    //public string User { get; set; }

    //    //public string Status { get; set; }

    //    //public DateTime? Sdate { get; set; }

    //    //public DateTime? Cdate { get; set; }

    //    //public string ImgData { get; set; }

    //    //public string Contlv { get; set; }

    //    //public string Endorsed { get; set; }

    //    //public string Approver1 { get; set; }

    //    //public string Approver2 { get; set; }

    //    //public string Approver3 { get; set; }

    //    //public string Cancelreqstatus { get; set; }

    //    //public string Cancelreq1 { get; set; }

    //    //public string Cancelreq2 { get; set; }

    //    //public string Cancelreq3 { get; set; }

    //    //public string Attachmentimgdata { get; set; }

    //    [JsonProperty("empid")]
    //    public string Empid { get; set; }

    //    [JsonProperty("lv")]
    //    public string LV { get; set; }

    //    //[JsonProperty("dfrom")]
    //    //public DateTime Dfrom { get; set; }

    //    //[JsonProperty("dto")]
    //    //public DateTime Dto { get; set; }

    //    //[JsonProperty("noofday")]
    //    //public double Noofday { get; set; }

    //    //[JsonProperty("halfday")]
    //    //public string Halfday { get; set; }

    //    //[JsonProperty("halftype")]
    //    //public string Halftype { get; set; }

    //    //[JsonProperty("entitlemt")]
    //    //public string Entitlemt { get; set; }

    //    //[JsonProperty("bf")]
    //    //public string Bf { get; set; }

    //    //[JsonProperty("remark")]
    //    //public string Remark { get; set; }

    //    //[JsonProperty("user")]
    //    //public string User { get; set; }

    //    //[JsonProperty("status")]
    //    //public string Status { get; set; }

    //    //[JsonProperty("sdate")]
    //    //public DateTime? Sdate { get; set; }

    //    //[JsonProperty("cdate")]
    //    //public DateTime? Cdate { get; set; }

    //    //[JsonProperty("img")]
    //    //public string ImgData { get; set; }

    //    //[JsonProperty("contlv")]
    //    //public string Contlv { get; set; }

    //    //[JsonProperty("endorsed")]
    //    //public string Endorsed { get; set; }

    //    //[JsonProperty("approver1")]
    //    //public string Approver1 { get; set; }

    //    //[JsonProperty("approver2")]
    //    //public string Approver2 { get; set; }

    //    //[JsonProperty("approver3")]
    //    //public string Approver3 { get; set; }

    //    //[JsonProperty("cancelreqstatus")]
    //    //public string Cancelreqstatus { get; set; }

    //    //[JsonProperty("cancelreq1")]
    //    //public string Cancelreq1 { get; set; }

    //    //[JsonProperty("cancelreq2")]
    //    //public string Cancelreq2 { get; set; }

    //    //[JsonProperty("cancelreq3")]
    //    //public string Cancelreq3 { get; set; }

    //    //[JsonProperty("attachmentimgdata")]
    //    //public string AttachmentImgData { get; set; }

    //    [JsonProperty("alertMsg")]
    //    public string AlertMsg { get; set; }
    //}
    public class ESS_LeaveCancelRequest
    {
        [JsonProperty("empid")]
        public string Empid { get; set; }

        [JsonProperty("lv")]
        public string LV { get; set; }

        [JsonProperty("alertMsg")]
        public string AlertMsg { get; set; }

        [JsonProperty("dfrom")]
        public DateTime Dfrom { get; set; }

        [JsonProperty("dto")]
        public DateTime Dto { get; set; }

        [JsonProperty("noofday")]
        public double Noofday { get; set; }

        [JsonProperty("halfday")]
        public string Halfday { get; set; }

        [JsonProperty("halftype")]
        public string Halftype { get; set; }

        [JsonProperty("entitlemt")]
        public string Entitlemt { get; set; }

        [JsonProperty("bf")]
        public string Bf { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("sdate")]
        public DateTime? Sdate { get; set; }

        [JsonProperty("cdate")]
        public DateTime? Cdate { get; set; }

        [JsonProperty("img")]
        public string ImgData { get; set; }

        [JsonProperty("contlv")]
        public string Contlv { get; set; }

        [JsonProperty("endorsed")]
        public string Endorsed { get; set; }

        [JsonProperty("approver1")]
        public string Approver1 { get; set; }

        [JsonProperty("approver2")]
        public string Approver2 { get; set; }

        [JsonProperty("approver3")]
        public string Approver3 { get; set; }

        [JsonProperty("cancelreqstatus")]
        public string Cancelreqstatus { get; set; }

        [JsonProperty("cancelreq1")]
        public string Cancelreq1 { get; set; }

        [JsonProperty("cancelreq2")]
        public string Cancelreq2 { get; set; }

        [JsonProperty("cancelreq3")]
        public string Cancelreq3 { get; set; }

        [JsonProperty("attachmentimgdata")]
        public string AttachmentImgData { get; set; }
    }
    public class ESS_DeleteLeave
    {
        //public string Empid { get; set; }
        //public string LV { get; set; }
        //public DateTime dFrom { get; set; }
        //public DateTime dTo { get; set; }
        //public double NoofDay { get; set; }
        //public string HalfDay { get; set; }
        //public string HalfType { get; set; }
        //public string AlertMsg { get; set; }
        //public string Entitlemt { get; set; }

        //public string Bf { get; set; }

        //public string Remark { get; set; }

        //public string User { get; set; }

        //public string Status { get; set; }

        //public DateTime? Sdate { get; set; }

        //public DateTime? Cdate { get; set; }

        //public string ImgData { get; set; }

        //public string Contlv { get; set; }

        //public string Endorsed { get; set; }

        //public string Approver1 { get; set; }

        //public string Approver2 { get; set; }

        //public string Approver3 { get; set; }

        //public string Cancelreqstatus { get; set; }

        //public string Cancelreq1 { get; set; }

        //public string Cancelreq2 { get; set; }

        //public string Cancelreq3 { get; set; }

        //public string Attachmentimgdata { get; set; }
        [JsonProperty("empid")]
        public string Empid { get; set; }

        [JsonProperty("lv")]
        public string LV { get; set; }
        //
        [JsonProperty("dfromInput")]
        public string DfromInput { get; set; }
        [JsonProperty("dtoInput")]
        public string DtoInput { get; set; }
        //

        [JsonProperty("dfrom")]
        public DateTime Dfrom { get; set; }

        [JsonProperty("dto")]
        public DateTime Dto { get; set; }

        [JsonProperty("noofday")]
        public double Noofday { get; set; }

        [JsonProperty("halfday")]
        public string Halfday { get; set; }

        [JsonProperty("halftype")]
        public string Halftype { get; set; }

        [JsonProperty("entitlemt")]
        public string Entitlemt { get; set; }

        [JsonProperty("bf")]
        public string Bf { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("sdate")]
        public DateTime? Sdate { get; set; }

        [JsonProperty("cdate")]
        public DateTime? Cdate { get; set; }

        [JsonProperty("img")]
        public string ImgData { get; set; }

        [JsonProperty("contlv")]
        public string Contlv { get; set; }

        [JsonProperty("endorsed")]
        public string Endorsed { get; set; }

        [JsonProperty("approver1")]
        public string Approver1 { get; set; }

        [JsonProperty("approver2")]
        public string Approver2 { get; set; }

        [JsonProperty("approver3")]
        public string Approver3 { get; set; }

        [JsonProperty("cancelreqstatus")]
        public string Cancelreqstatus { get; set; }

        [JsonProperty("cancelreq1")]
        public string Cancelreq1 { get; set; }

        [JsonProperty("cancelreq2")]
        public string Cancelreq2 { get; set; }

        [JsonProperty("cancelreq3")]
        public string Cancelreq3 { get; set; }

        [JsonProperty("attachmentimgdata")]
        public string AttachmentImgData { get; set; }

        [JsonProperty("alertMsg")]
        public string AlertMsg { get; set; }
    }

    public class ESS_ActivityLog
    {
        [JsonProperty("activityName")]
        public string ActivityName { get; set; }

        [JsonProperty("activityDateTime")]
        public DateTime? ActivityDateTime { get; set; }

        [JsonProperty("activityStatus")]
        public string ActivityStatus { get; set; }

        [JsonProperty("activityUser")]
        public string ActivityUser { get; set; }
    }

    public class ESS_ChangePassword
    {
        public string id { get; set; }
        public string currentpassword { get; set; }
        public string newpassword { get; set; }
    }

    public class ESS_PushNotification
    {
        public string EmpID { get; set; }
        public string DeviceToken { get; set; }
        public int PendingStatus { get; set; }
    }

    public class ESS_DeviceTokenList
    {
        public string EmpID { get; set; }
        public string DeviceToken { get; set; }
        public /*bool*/int PendingStatus { get; set; }
    }
    public class ESS_PushNotificationDeviceTokenList
    {
        public string ApproverID { get; set; }
        public string CompanyIdentifier { get; set; }
        public string DeviceToken { get; set; }
        public string PendingStatus { get; set; }
        public /*int*/string SendingTimes { get; set; }
        public /*DateTime*/string LastLogin { get; set; }
        public string Platform { get; set; }
    }
    public class ESS_CompanyInfo
    {
        public string CompID { get; set; }
        public string CompName { get; set; }
        public string ADD1 { get; set; }
        public string ADD2 { get; set; }
        public string ADD3 { get; set; }
        public string ADD4 { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
    /**/

    /**/
    public class DB_CheckToken
    {
        public string Token { get; set; }
    }

    public class DB_CheckIdentifier
    {
        public string IdentifierID { get; set; }
    }
    /**/
    public class DB_Auth
    {
        [JsonProperty("dbpath")]
        public string dbpath { get; set; }

        [JsonProperty("dbname")]
        public string dbname { get; set; }

        [JsonProperty("dbuser")]
        public string dbuser { get; set; }

        [JsonProperty("dbpass")]
        public string dbpass { get; set; }

        public string Token { get; set; }

        [JsonProperty("userid")]
        public string userid { get; set; }

        //[JsonProperty("userpassword")]
        public string userpassword { get; set; }
    }

    public class LeaveRecordsChooseMonth
    {
        public string Month { get; set; }
    }

    public class LeaveRecordsChooseYear
    {
        public int Year { get; set; }
    }
}