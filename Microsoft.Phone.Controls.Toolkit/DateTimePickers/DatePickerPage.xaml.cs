// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Microsoft.Phone.Controls.Primitives;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a page used by the DatePicker control that allows the user to choose a date (day/month/year).
    /// </summary>
    public partial class DatePickerPage : DateTimePickerPageBase
    {
        /// <summary>
        /// Initializes a new instance of the DatePickerPage control.
        /// </summary>
        public DatePickerPage()
        {
            InitializeComponent();

            // Hook up the data sources
            PrimarySelector.DataSource = new YearDataSource();
            SecondarySelector.DataSource = new MonthDataSource();
            TertiarySelector.DataSource = new DayDataSource();

            InitializeDateTimePickerPage(PrimarySelector, SecondarySelector, TertiarySelector);
        }

        /// <summary>
        /// Gets a sequence of LoopingSelector parts ordered according to culture string for date/time formatting.
        /// </summary>
        /// <returns>LoopingSelectors ordered by culture-specific priority.</returns>
        protected override IEnumerable<LoopingSelector> GetSelectorsOrderedByCulturePattern()
        {
            string pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpperInvariant();

            if (DateTimePickerBase.DateShouldFlowRTL())
            {
                char[] reversedPattern = pattern.ToCharArray();
                Array.Reverse(reversedPattern);
                pattern = new string(reversedPattern);
            }

            return GetSelectorsOrderedByCulturePattern(
                pattern,
                new char[] { 'Y', 'M', 'D' },
                new LoopingSelector[] { PrimarySelector, SecondarySelector, TertiarySelector });
        }

        /// <summary>
        /// Handles changes to the page's Orientation property.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (null == e)
            {
                throw new ArgumentNullException("e");
            }

            base.OnOrientationChanged(e);
            SystemTrayPlaceholder.Visibility = (0 != (PageOrientation.Portrait & e.Orientation)) ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        /// <summary>
        /// Sets the selectors and title flow direction.
        /// </summary>
        /// <param name="flowDirection">Flow direction to set.</param>
        internal override void SetFlowDirection(FlowDirection flowDirection)
        {
            HeaderTitle.FlowDirection = flowDirection;

            PrimarySelector.FlowDirection = flowDirection;
            SecondarySelector.FlowDirection = flowDirection;
            TertiarySelector.FlowDirection = flowDirection;
        }

        /// <summary>
        /// Added by Alex Lapuka.
        /// Allows use of page without DatePicker
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var page = e.Content as IDateTimePickerPage;
            if (page == null) return;

            string date = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("date", out date)) page.Value = DateTime.Parse(date, CultureInfo.CurrentCulture);
            else page.Value = DateTime.Now;
        }

        // костыль чтобы вернуть значение
        private void DoneButton_Click(object sender, EventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("date"))
            {
                DateTime date = ((DateTimeWrapper)PrimarySelectorPart.DataSource.SelectedItem).DateTime;
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Remove("DateTimePickerPage_rv");
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Add("DateTimePickerPage_rv", date.ToShortDateString());
            }
        }

        
    }
}
