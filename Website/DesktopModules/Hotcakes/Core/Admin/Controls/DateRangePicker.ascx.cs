using System;
using System.Globalization;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class DateRangePicker : HccUserControl
    {
        private string DateFormat => CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        private readonly DateRange _range = new DateRange();

        public class RangeTypeChangedEventArgs : EventArgs { }

        public delegate void RangeTypeChangedDelegate(object sender, RangeTypeChangedEventArgs e);

        public event RangeTypeChangedDelegate RangeTypeChanged;

        public string FormItemCssClass { get; set; }

        public string LabelText
        {
            get { return lblDateRangeLabel.Text; }
            set { lblDateRangeLabel.Text = value; }
        }

        public bool Enabled
        {
            get { return lstRangeType.Enabled; }
            set { lstRangeType.Enabled = value; }
        }

        public bool HideButton
        {
            get { return !btnShow.Visible; }
            set { btnShow.Visible = !value; }
        }

        public DateRangeType RangeType
        {
            get { return (DateRangeType)int.Parse(lstRangeType.SelectedValue); }
            set
            {
                if (lstRangeType.Items.FindByValue(((int)value).ToString()) != null)
                {
                    lstRangeType.SelectedValue = ((int)value).ToString();
                }
            }
        }

        public DateTime StartDate
        {
            get
            {
                if (RangeType == DateRangeType.Custom)
                {
                    var date = DateTime.Parse(radStartDate.Text.Trim(), CultureInfo.CurrentCulture);
                    return date.ZeroOutTime();
                }
                _range.RangeType = RangeType;

                if (RangeType != DateRangeType.AllDates)
                    return _range.StartDate;
                return DateTime.MinValue;
            }
            set
            {
                radStartDate.Text = value.ToString(DateFormat, CultureInfo.CurrentCulture);
                RangeType = DateRangeType.Custom;
            }
        }

        public DateTime EndDate
        {
            get
            {
                if (RangeType == DateRangeType.Custom)
                {
                    var date = DateTime.Parse(radEndDate.Text.Trim(), CultureInfo.CurrentCulture);
                    return date.MaxOutTime();
                }
                _range.RangeType = RangeType;
                if (RangeType != DateRangeType.AllDates)
                    return _range.EndDate;
                return DateTime.MaxValue;
            }
            set
            {
                radEndDate.Text = value.ToString(DateFormat, CultureInfo.CurrentCulture);
                RangeType = DateRangeType.Custom;
            }
        }

        public DateRangePicker()
        {
            FormItemCssClass = "hcFormItemHor";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnShow.Click += btnShow_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                lstRangeType_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            RangeTypeChanged?.Invoke(this, new RangeTypeChangedEventArgs());
        }

        protected void lstRangeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RangeTypeChanged?.Invoke(this, new RangeTypeChangedEventArgs());
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (RangeType != DateRangeType.Custom)
            {
                radStartDate.Text = StartDate.ToString(DateFormat, CultureInfo.CurrentCulture);
                radEndDate.Text = EndDate.ToString(DateFormat, CultureInfo.CurrentCulture);
            }

            pnlCustom.Visible = lstRangeType.SelectedValue == ((int)DateRangeType.Custom).ToString();
        }

        public DateTime GetStartDateUtc(HotcakesApplication hccApp)
        {
            DateTime result;

            if (RangeType == DateRangeType.Custom)
            {
                var date = DateTime.Parse(radStartDate.Text.Trim(), CultureInfo.CurrentCulture);
                result = date.ZeroOutTime();
            }
            else
            {
                _range.RangeType = RangeType;
                _range.CalculateDatesFromType(DateHelper.ConvertUtcToStoreTime(hccApp));
                result = _range.StartDate;
            }

            return DateHelper.ConvertStoreTimeToUtc(hccApp, result);
        }

        public DateTime GetEndDateUtc(HotcakesApplication hccApp)
        {
            DateTime result;
            if (RangeType == DateRangeType.Custom)
            {
                var date = DateTime.Parse(radEndDate.Text.Trim(), CultureInfo.CurrentCulture);
                result = date.MaxOutTime();
            }
            else
            {
                _range.RangeType = RangeType;
                _range.CalculateDatesFromType(DateHelper.ConvertUtcToStoreTime(hccApp));
                result = _range.EndDate;
            }

            return DateHelper.ConvertStoreTimeToUtc(hccApp, result);
        }
    }
}